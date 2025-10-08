using UnityEngine;
using UnityEngine.Rendering;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// FluidController: orchestrates 3D density simulation + ray-march rendering
// Attach to a GameObject (e.g. the volume cube) and assign:
// - computeShader = FludSim.compute
// - rayMarchMaterial = material using Custom/RayMarchShader
// - volumeRenderer = MeshRenderer of the cube
public class FluidController : MonoBehaviour
{
    [Header("Simulation Settings")]
    public ComputeShader computeShader;
    public Vector3Int gridSize = new Vector3Int(128, 128, 128);

    [Tooltip("Constant diffusion rate (small value ~0.02 - 0.1).")]
    public float diffusionRate = 0.05f;
    [Tooltip("Jacobi iterations for diffusion (more = smoother but slower).")]
    public int diffusionIterations = 15;

    [Header("Velocity Init / Sources")]
    public Vector3 initialVelocity = new Vector3(0, 0.25f, 0);

    [Header("Procedural Injection (Hold Space)")]
    public float injectRadius = 20f;
    public float injectValue = 1.0f;

    [Header("Generic Source (Prebaked)")]
    public bool addConstantSource = true;
    public float sourceScale = 1.0f;

    [Header("Rendering")]
    public Material rayMarchMaterial;
    public MeshRenderer volumeRenderer;
    public int rayMarchSteps = 96;
    public float extinction = 1.5f;
    public Color cloudColor = new Color(0.85f, 0.9f, 1.0f);
    public bool debugBounds = false;

    // Internal RenderTextures
    private RenderTexture densityA, densityB;
    private RenderTexture velocity;

    // Optional source density texture
    private RenderTexture densitySource;

    // Kernels
    private int kInitVelocity = -1;
    private int kInject = -1;
    private int kAddSource = -1;
    private int kAdvect = -1;
    private int kDiffuse = -1;

    // Cached bounds
    private Vector3 boundsMin;
    private Vector3 boundsSize;

    // ------------------------------------------------------
    // Unity Lifecycle
    // ------------------------------------------------------
    void Awake()
    {
        if (!computeShader)
        {
            Debug.LogError("ComputeShader not assigned.", this);
            enabled = false;
            return;
        }

        Allocate3DTexture(ref densityA, RenderTextureFormat.RFloat);
        Allocate3DTexture(ref densityB, RenderTextureFormat.RFloat);
        Allocate3DTexture(ref velocity, RenderTextureFormat.ARGBFloat);

        FindKernels();

        // Initialize velocity
        if (kInitVelocity >= 0)
        {
            computeShader.SetVector("initialVelocity", initialVelocity);
            computeShader.SetTexture(kInitVelocity, "velocityWrite", velocity);
            DispatchFull(kInitVelocity);
        }

        // Build constant source (one-time sphere) if requested
        if (addConstantSource && kAddSource >= 0)
        {
            CreateSourceTexture();
        }

        UpdateBoundsFromTransform();
        PushStaticRenderParams();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0) return;

        // Set global simulation params
        computeShader.SetVector("gridSize", new Vector4(gridSize.x, gridSize.y, gridSize.z, 0));
        computeShader.SetFloat("deltaTime", dt);


        //  source (add_source stage)
        if (kAddSource >= 0 && addConstantSource && densitySource)
        {
            computeShader.SetFloat("sourceScale", sourceScale);
            computeShader.SetTexture(kAddSource, "densityWrite", densityA);
            computeShader.SetTexture(kAddSource, "sourceDensity", densitySource);
            DispatchFull(kAddSource);
        }

        // real-time injection (space bar)
        bool injectPressed =
#if ENABLE_INPUT_SYSTEM
            (Keyboard.current != null && Keyboard.current.spaceKey.isPressed);
#else
            Input.GetKey(KeyCode.Space);
#endif
        if (kInject >= 0 && injectPressed)
        {
            computeShader.SetVector("injectPos", new Vector3(gridSize.x * 0.5f, gridSize.y * 0.5f, gridSize.z * 0.5f));
            computeShader.SetFloat("injectRadius", injectRadius);
            computeShader.SetFloat("injectValue", injectValue);
            computeShader.SetTexture(kInject, "densityWrite", densityA);
            DispatchFull(kInject);
        }

        // 3. Advection: densityA -> densityB
        if (kAdvect >= 0)
        {
            computeShader.SetTexture(kAdvect, "velocityRead", velocity);
            computeShader.SetTexture(kAdvect, "densityRead", densityA);
            computeShader.SetTexture(kAdvect, "densityWrite", densityB);
            DispatchFull(kAdvect);
            Swap(ref densityA, ref densityB);
        }

        // 4. Diffusion (Jacobi) on density
        if (kDiffuse >= 0 && diffusionRate > 0)
        {
          float dx = 1.0f; // grid cell size
          float alphaVal = (dx * dx) / (diffusionRate * dt);
          float rBetaVal = 1.0f / (6.0f + alphaVal);

          computeShader.SetFloat("alpha", alphaVal);
          computeShader.SetFloat("rBeta", rBetaVal);

            for (int i = 0; i < diffusionIterations; i++)
            {
                computeShader.SetTexture(kDiffuse, "bufferRead", densityA);
                computeShader.SetTexture(kDiffuse, "initialBuffer", densityA);
                computeShader.SetTexture(kDiffuse, "bufferWrite", densityB);
                DispatchFull(kDiffuse);
                Swap(ref densityA, ref densityB);
            }
        }

        // 5. Update material
        if (rayMarchMaterial)
        {
            rayMarchMaterial.SetTexture("_DensityTex", densityA);
            rayMarchMaterial.SetVector("_GridSize", (Vector3)gridSize);
            rayMarchMaterial.SetVector("_BoundsMin", boundsMin);
            rayMarchMaterial.SetVector("_BoundsSize", boundsSize);
            rayMarchMaterial.SetFloat("_Extinction", extinction);
            rayMarchMaterial.SetColor("_CloudColor", cloudColor);
            rayMarchMaterial.SetInt("_Steps", Mathf.Max(4, rayMarchSteps));
            rayMarchMaterial.SetInt("_DebugBounds", debugBounds ? 1 : 0);
        }
    }

    void OnDestroy()
    {
        ReleaseRT(densityA);
        ReleaseRT(densityB);
        ReleaseRT(velocity);
        ReleaseRT(densitySource);
    }

    // ------------------------------------------------------
    // Allocation / Setup
    // ------------------------------------------------------
    void Allocate3DTexture(ref RenderTexture rt, RenderTextureFormat fmt)
    {
        var desc = new RenderTextureDescriptor(
            gridSize.x,
            gridSize.y,
            fmt,
            0
        );
        desc.dimension         = TextureDimension.Tex3D;
        desc.volumeDepth       = gridSize.z;
        desc.enableRandomWrite = true;
        desc.msaaSamples       = 1;
        desc.mipCount          = 1;
        desc.autoGenerateMips  = false;
        desc.depthBufferBits   = 0;

        rt = new RenderTexture(desc);
        rt.wrapMode   = TextureWrapMode.Clamp;
        rt.filterMode = FilterMode.Bilinear;
        rt.enableRandomWrite = true;
        rt.Create();
    }

    void FindKernels()
    {
        kInitVelocity = SafeFind("InitVelocityKernel");
        kInject       = SafeFind("InjectKernel");
        kAddSource    = SafeFind("AddSourceKernel");
        kAdvect       = SafeFind("AdvectKernel");
        kDiffuse      = SafeFind("DiffuseKernel");
    }

    int SafeFind(string kernel)
    {
        try
        {
            int k = computeShader.FindKernel(kernel);
            return k;
        }
        catch
        {
            Debug.LogWarning($"Kernel '{kernel}' not found in compute shader.", this);
            return -1;
        }
    }

    void UpdateBoundsFromTransform()
    {
        var t = volumeRenderer ? volumeRenderer.transform : transform;
        // Assuming unit cube mesh (size 1 centered) scaled uniformly
        Vector3 center = t.position;
        Vector3 size   = t.localScale;
        boundsMin  = center - size * 0.5f;
        boundsSize = size;
    }

    void PushStaticRenderParams()
    {
        if (rayMarchMaterial)
        {
            rayMarchMaterial.SetVector("_BoundsMin", boundsMin);
            rayMarchMaterial.SetVector("_BoundsSize", boundsSize);
        }
    }

    // One-time build of a sphere density source using the InjectKernel
    void CreateSourceTexture()
    {
        var desc = new RenderTextureDescriptor(gridSize.x, gridSize.y, RenderTextureFormat.RFloat, 0)
        {
            dimension = TextureDimension.Tex3D,
            volumeDepth = gridSize.z,
            enableRandomWrite = true,
            msaaSamples = 1,
            mipCount = 1
        };
        densitySource = new RenderTexture(desc);
        densitySource.wrapMode = TextureWrapMode.Clamp;
        densitySource.filterMode = FilterMode.Bilinear;
        densitySource.enableRandomWrite = true;
        densitySource.Create();

        if (kInject >= 0)
        {
            computeShader.SetVector("injectPos",
                new Vector3(gridSize.x * 0.5f, gridSize.y * 0.5f, gridSize.z * 0.5f));
            computeShader.SetFloat("injectRadius", Mathf.Min(gridSize.x, gridSize.y, gridSize.z) * 0.25f);
            computeShader.SetFloat("injectValue", 1.0f);
            computeShader.SetTexture(kInject, "densityWrite", densitySource);
            DispatchFull(kInject);
        }
    }

    // ------------------------------------------------------
    // Utility
    // ------------------------------------------------------
    void DispatchFull(int kernel)
    {
        if (kernel < 0) return;
        int gx = Mathf.CeilToInt(gridSize.x / 8.0f);
        int gy = Mathf.CeilToInt(gridSize.y / 8.0f);
        int gz = Mathf.CeilToInt(gridSize.z / 8.0f);
        computeShader.Dispatch(kernel, gx, gy, gz);
    }

    void Swap(ref RenderTexture a, ref RenderTexture b)
    {
        var tmp = a; a = b; b = tmp;
    }

    void ReleaseRT(RenderTexture rt)
    {
        if (rt != null)
        {
            rt.Release();
        }
    }
}
