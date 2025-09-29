using UnityEngine;

public class FluidController : MonoBehaviour
{
    public ComputeShader computeShader;
    // 1. Assign this in the Inspector by creating a new Material
    //    and assigning the ParticleRenderer shader to it.
    public Material particleMaterial;
    public int num_particles = 1000;

    private ComputeBuffer _particleBuffer;
    private int _kernelIndex;

    struct Particle
    {
        public Vector3 position;
    }

    void Start()
    {
        _particleBuffer = new ComputeBuffer(num_particles, sizeof(float) * 3);

        Particle[] particles = new Particle[num_particles];
        for (int i = 0; i < num_particles; i++)
        {
            // Start particles inside a cube shape around the GameObject's position
            Vector3 randomPos = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            );
            particles[i] = new Particle
            {
                position = transform.position + randomPos
            };
        }
        _particleBuffer.SetData(particles);

        _kernelIndex = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(_kernelIndex, "particles", _particleBuffer);
    }

    void Update()
    {
        computeShader.SetFloat("deltaTime", Time.deltaTime);

        int threadGroups = Mathf.CeilToInt(num_particles / 64f);
        computeShader.Dispatch(_kernelIndex, threadGroups, 1, 1);
    }

    // 2. This function is called by Unity after a camera finishes rendering
    void OnRenderObject() {
        // 3. Set the particle buffer on the rendering material
        particleMaterial.SetBuffer("particles", _particleBuffer);

        // 4. Tell the material to draw itself
        particleMaterial.SetPass(0);

        // 5. Issue the magical draw command!
        // This tells the GPU: "Draw me 'num_particles' points using the currently active shader."
        Graphics.DrawProceduralNow(MeshTopology.Points, 1, num_particles);
    }

    void OnDestroy()
    {
        if (_particleBuffer != null) {
            _particleBuffer.Release();
        }
    }
}