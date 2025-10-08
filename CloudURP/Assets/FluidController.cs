using UnityEngine;

public class FluidController : MonoBehaviour
{
    [Header("Simulation Settings")]
    public ComputeShader computeShader;
    public Vector3Int gridSize = new Vector3Int(128, 128, 128);

    [Header("Rendering")]
    public Material rayMarchMaterial;

    [Header("Debug")]
    public bool addSourceContinuously = true;
    public KeyCode addSourceKey = KeyCode.Space;

    private RenderTexture densityRead, densityWrite;
    private RenderTexture velocityRead, velocityWrite;

    private int advectKernel;
    private int diffuseKernel;
    private int initKernel = -1;
    private int addSourceKernel = -1;

    void Start()
    {
        Debug.Log("🚀 Iniciando Fluid Simulation...");

        if (computeShader == null)
        {
            Debug.LogError("❌ ComputeShader está NULL!");
            enabled = false;
            return;
        }

        CreateTextures();
        FindKernels();
        InitializeDensity();

        Debug.Log("✅ Fluid Simulation iniciada!");
    }

    void CreateTextures()
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

        desc.colorFormat = RenderTextureFormat.RFloat;
        densityRead = new RenderTexture(desc);
        densityWrite = new RenderTexture(desc);
        densityRead.Create();
        densityWrite.Create();

        desc.colorFormat = RenderTextureFormat.ARGBFloat;
        velocityRead = new RenderTexture(desc);
        velocityWrite = new RenderTexture(desc);
        velocityRead.Create();
        velocityWrite.Create();

        Debug.Log($"✅ Texturas criadas: {gridSize.x}x{gridSize.y}x{gridSize.z}");
    }

    void FindKernels()
    {
        advectKernel = computeShader.FindKernel("AdvectKernel");
        diffuseKernel = computeShader.FindKernel("DiffuseKernel");
        
        try { initKernel = computeShader.FindKernel("InitKernel"); }
        catch { Debug.LogWarning("⚠️ InitKernel não encontrado"); }
        
        try { addSourceKernel = computeShader.FindKernel("AddSourceKernel"); }
        catch { Debug.LogWarning("⚠️ AddSourceKernel não encontrado"); }
    }

    void InitializeDensity()
    {
        if (initKernel >= 0)
        {
            Debug.Log("📦 Inicializando densidade com esfera central...");
            
            Vector3 center = new Vector3(gridSize.x / 2f, gridSize.y / 2f, gridSize.z / 2f);
            
            computeShader.SetTexture(initKernel, "densityWrite", densityRead);
            computeShader.SetVector("sourcePos", center);
            computeShader.SetFloat("sourceRadius", 30f);
            computeShader.SetInts("gridSize", new int[] { gridSize.x, gridSize.y, gridSize.z });
            
            computeShader.Dispatch(initKernel, gridSize.x / 8, gridSize.y / 8, gridSize.z / 8);
            
            Debug.Log($"✅ Densidade inicializada no centro: {center}");
        }
        else
        {
            Debug.LogError("❌ Não foi possível inicializar - InitKernel não encontrado!");
        }
    }

    void Update()
    {
        // Adicionar densidade ao pressionar tecla OU continuamente
        if ((Input.GetKey(addSourceKey) || addSourceContinuously) && addSourceKernel >= 0)
        {
            Vector3 sourcePos = new Vector3(gridSize.x / 2f, gridSize.y / 4f, gridSize.z / 2f);
            AddDensitySource(sourcePos, 25f, 0.5f * Time.deltaTime);
        }

        // Simulação
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetInts("gridSize", new int[] { gridSize.x, gridSize.y, gridSize.z });

        // Advection
        computeShader.SetTexture(advectKernel, "velocityRead", velocityRead);
        computeShader.SetTexture(advectKernel, "densityRead", densityRead);
        computeShader.SetTexture(advectKernel, "densityWrite", densityWrite);
        computeShader.Dispatch(advectKernel, gridSize.x / 8, gridSize.y / 8, gridSize.z / 8);
        Swap(ref densityRead, ref densityWrite);

        // Diffusion (reduzido para melhor performance)
        float diffusionRate = 0.05f;
        if (diffusionRate > 0 && Time.deltaTime > 0)
        {
            float alpha = (gridSize.x * gridSize.x) / (diffusionRate * Time.deltaTime);
            float rBeta = 1.0f / (6.0f + alpha);
            computeShader.SetFloat("alpha", alpha);
            computeShader.SetFloat("rBeta", rBeta);
            computeShader.SetTexture(diffuseKernel, "initialBuffer", densityRead);

            for (int i = 0; i < 5; i++)
            {
                computeShader.SetTexture(diffuseKernel, "bufferRead", densityRead);
                computeShader.SetTexture(diffuseKernel, "bufferWrite", densityWrite);
                computeShader.Dispatch(diffuseKernel, gridSize.x / 8, gridSize.y / 8, gridSize.z / 8);
                Swap(ref densityRead, ref densityWrite);
            }
        }

        // Atualizar material
        if (rayMarchMaterial != null)
        {
            rayMarchMaterial.SetTexture("_DensityTex", densityRead);
            rayMarchMaterial.SetVector("_GridSize", new Vector4(gridSize.x, gridSize.y, gridSize.z, 0));
        }
        else
        {
            Debug.LogWarning("⚠️ Ray March Material está NULL!");
        }
    }

    void AddDensitySource(Vector3 position, float radius, float amount)
    {
        if (addSourceKernel < 0) return;

        computeShader.SetTexture(addSourceKernel, "densityWrite", densityRead);
        computeShader.SetVector("sourcePos", position);
        computeShader.SetFloat("sourceRadius", radius);
        computeShader.SetFloat("sourceAmount", amount);
        computeShader.Dispatch(addSourceKernel, gridSize.x / 8, gridSize.y / 8, gridSize.z / 8);
    }

    void Swap(ref RenderTexture a, ref RenderTexture b)
    {
        RenderTexture temp = a;
        a = b;
        b = temp;
    }

    void OnDestroy()
    {
        if (densityRead != null) densityRead.Release();
        if (densityWrite != null) densityWrite.Release();
        if (velocityRead != null) velocityRead.Release();
        if (velocityWrite != null) velocityWrite.Release();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 200));
        GUILayout.Box("=== FLUID SIMULATION ===");
        GUILayout.Label($"Grid: {gridSize}");
        GUILayout.Label($"Density Texture: {(densityRead != null && densityRead.IsCreated() ? "✅" : "❌")}");
        GUILayout.Label($"Material: {(rayMarchMaterial != null ? "✅" : "❌")}");
        GUILayout.Label($"\nPressione {addSourceKey} para adicionar densidade");
        GUILayout.Label($"Adição contínua: {(addSourceContinuously ? "ON" : "OFF")}");
        GUILayout.EndArea();
    }
}
