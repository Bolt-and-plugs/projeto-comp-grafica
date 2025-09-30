using UnityEngine;
using UnityEngine.Rendering;

public class FluidController : MonoBehaviour
{
    [Header("Simulation Settings")]
    public ComputeShader computeShader;
    public Vector3Int gridSize = new Vector3Int(128, 128, 128);

    [Header("Rendering")]
    public Material rayMarchMaterial;

    // Simulation data is stored in 3D Render Textures
    private RenderTexture densityRead, densityWrite;
    private RenderTexture velocityRead, velocityWrite;

    // Kernel indices for the compute shader
    private int advectKernel;
    private int diffuseKernel;

    void Start()
    {
        RenderTextureDescriptor desc = new RenderTextureDescriptor
        {
            dimension = TextureDimension.Tex3D,
            width = gridSize.x,
            height = gridSize.y,
            volumeDepth = gridSize.z,
            enableRandomWrite = true,
            msaaSamples = 1
        };

        // Create the density buffers (single float channel is enough)
        desc.colorFormat = RenderTextureFormat.RFloat;
        densityRead = new RenderTexture(desc);
        densityWrite = new RenderTexture(desc);
        densityRead.Create();
        densityWrite.Create();

        // Create the velocity buffers
        desc.colorFormat = RenderTextureFormat.ARGBFloat; // R, G, B for X, Y, Z velocity
        velocityRead = new RenderTexture(desc);
        velocityWrite = new RenderTexture(desc);
        velocityRead.Create();
        velocityWrite.Create();

        // 2. Find and store the kernel indices
        advectKernel = computeShader.FindKernel("AdvectKernel");
        diffuseKernel = computeShader.FindKernel("DiffuseKernel");

        // dispatch some random intialization for testing (with no extra kernel)


    }

    void Update()
    {
      if (Input.GetKey(KeyCode.Space)) // Hold Space to add density
      {
        computeShader.SetVector("injectPos", new Vector3(gridSize.x / 2f, gridSize.y / 2f, gridSize.z / 2f));
        computeShader.SetFloat("injectRadius", 32f);
        computeShader.SetFloat("injectValue", 1.0f);
        computeShader.SetTexture(injectKernel, "densityWrite", densityRead);
        computeShader.Dispatch(injectKernel, gridSize.x / 8, gridSize.y / 8, gridSize.z / 8);
    }
        // --- SIMULATION PASSES ---
        // You will expand this section with more passes for a full fluid simulation

        // Pass global variables to the compute shader
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetInts("gridSize", new int[] { gridSize.x, gridSize.y, gridSize.z });
        
        // --- Advection Pass ---
        computeShader.SetTexture(advectKernel, "velocityRead", velocityRead);
        computeShader.SetTexture(advectKernel, "densityRead", densityRead);
        computeShader.SetTexture(advectKernel, "densityWrite", densityWrite);
        computeShader.Dispatch(advectKernel, gridSize.x / 8, gridSize.y / 8, gridSize.z / 8);
        Swap(ref densityRead, ref densityWrite);

        // --- Diffusion Pass ---
        float diffusionRate = 0.1f; // Adjust this value to control diffusion speed
        if (diffusionRate > 0 && Time.deltaTime > 0)
        {
            float alpha = (gridSize.x * gridSize.x) / (diffusionRate * Time.deltaTime);
            float rBeta = 1.0f / (6.0f + alpha);
            computeShader.SetFloat("alpha", alpha);
            computeShader.SetFloat("rBeta", rBeta);

            computeShader.SetTexture(diffuseKernel, "initialBuffer", densityRead);

            for (int i = 0; i < 20; i++) // More iterations = more accurate diffusion
            {
                computeShader.SetTexture(diffuseKernel, "bufferRead", densityRead);
                computeShader.SetTexture(diffuseKernel, "bufferWrite", densityWrite);
                computeShader.Dispatch(diffuseKernel, gridSize.x / 8, gridSize.y / 8, gridSize.z / 8);
                Swap(ref densityRead, ref densityWrite);
            }
        }

        // --- RENDER ---
        // 3. Pass the final simulation data to the ray marching material
        if (rayMarchMaterial != null)
        {
            rayMarchMaterial.SetTexture("_DensityTex", densityRead);
            rayMarchMaterial.SetVector("_GridSize", (Vector3)gridSize);
        }
    }

    // Helper function to swap RenderTextures for ping-ponging
    void Swap(ref RenderTexture a, ref RenderTexture b)
    {
        RenderTexture temp = a;
        a = b;
        b = temp;
    }

    // 4. Proper cleanup of GPU resources
    void OnDestroy()
    {
        if (densityRead != null) densityRead.Release();
        if (densityWrite != null) densityWrite.Release();
        if (velocityRead != null) velocityRead.Release();
        if (velocityWrite != null) velocityWrite.Release();
    }
}
