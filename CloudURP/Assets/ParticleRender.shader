Shader "Custom/RayMarchShader"
{
    Properties
    {
        _DensityTex ("Density Texture", 3D) = "" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off // We need to see the inside of the cube

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0 // We need shader model 3.0 for this

            #include "UnityCG.cginc"

            sampler3D _DensityTex;
            float3 _GridSize;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 rayDir : TEXCOORD1;
            };

            v2f vert (float4 vertex : POSITION, float3 normal : NORMAL)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
                o.vertex = UnityObjectToClipPos(vertex);
                
                // Calculate the ray direction from the camera to this vertex
                o.rayDir = o.worldPos - _WorldSpaceCameraPos;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 rayDir = normalize(i.rayDir);
                float3 rayOrigin = _WorldSpaceCameraPos;

                // Ray-AABB (Axis-Aligned Bounding Box) intersection test
                // This finds where the ray enters and exits the cube
                float3 invDir = 1.0 / rayDir;
                float3 t0 = (unity_WorldToObject[3].xyz - rayOrigin) * invDir;
                float3 t1 = (unity_WorldToObject[3].xyz + _GridSize - rayOrigin) * invDir;
                float3 tmin3 = min(t0, t1);
                float3 tmax3 = max(t0, t1);
                float tmin = max(tmin3.x, max(tmin3.y, tmin3.z));
                float tmax = min(tmax3.x, min(tmax3.y, tmax3.z));

                if (tmin >= tmax) {
                    discard; // Ray doesn't hit the box
                }

                // --- The Ray Marching Loop ---
                float3 startPos = rayOrigin + tmin * rayDir;
                float distance = tmax - tmin;
                int numSteps = 64; // More steps = better quality, lower performance
                float stepSize = distance / numSteps;

                float4 finalColor = float4(0, 0, 0, 0);
                float transmittance = 1.0; // How much light is getting through

                for (int s = 0; s < numSteps; s++)
                {
                    float3 currentPos = startPos + s * stepSize * rayDir;
                    
                    // Convert world position to texture coordinates (0-1 range)
                    float3 uvw = currentPos / _GridSize;

                    // Sample the density from our simulation
                    float density = tex3D(_DensityTex, uvw).r * 0.1; // * density multiplier

                    if (density > 0.01)
                    {
                        // Absorb light based on density
                        transmittance *= exp(-density * stepSize);

                        // Add emissive color (simple lighting)
                        float3 cloudColor = float3(0.8, 0.9, 1.0);
                        finalColor.rgb += cloudColor * density * transmittance * stepSize;
                    }

                    // Early exit if the ray is fully absorbed
                    if (transmittance < 0.01)
                    {
                        break;
                    }
                }
                
                finalColor.a = 1.0 - transmittance;
                return finalColor;
            }
            ENDCG
        }
    }
}
