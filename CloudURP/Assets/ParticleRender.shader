Shader "Custom/RayMarchShader"
{
    Properties
    {
        _DensityTex ("Density Texture", 3D) = "" {}
        _Intensity ("Intensity", Range(0, 10)) = 5
        _StepCount ("Step Count", Range(32, 256)) = 128
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Front  // Renderiza de trás para frente

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5

            #include "UnityCG.cginc"

            sampler3D _DensityTex;
            float4 _GridSize;
            float _Intensity;
            int _StepCount;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 objectPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.objectPos = v.vertex.xyz;
                return o;
            }

            // Ray-Box intersection
            bool RayBoxIntersection(float3 rayOrigin, float3 rayDir, float3 boxMin, float3 boxMax, out float tNear, out float tFar)
            {
                float3 invDir = 1.0 / rayDir;
                float3 t0 = (boxMin - rayOrigin) * invDir;
                float3 t1 = (boxMax - rayOrigin) * invDir;
                
                float3 tMin = min(t0, t1);
                float3 tMax = max(t0, t1);
                
                tNear = max(max(tMin.x, tMin.y), tMin.z);
                tFar = min(min(tMax.x, tMax.y), tMax.z);
                
                return tFar > tNear && tFar > 0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Configurar o raio em object space
                float3 rayOrigin = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1)).xyz;
                float3 rayDir = normalize(i.objectPos - rayOrigin);
                
                // Bounding box do cubo em object space (-0.5 a 0.5)
                float3 boxMin = float3(-0.5, -0.5, -0.5);
                float3 boxMax = float3(0.5, 0.5, 0.5);
                
                float tNear, tFar;
                if (!RayBoxIntersection(rayOrigin, rayDir, boxMin, boxMax, tNear, tFar))
                {
                    discard;
                }
                
                // Começar de dentro do box se a câmera estiver dentro
                tNear = max(tNear, 0);
                
                // Ray marching
                float stepSize = (tFar - tNear) / float(_StepCount);
                float3 currentPos = rayOrigin + rayDir * tNear;
                
                float4 finalColor = float4(0, 0, 0, 0);
                
                for (int step = 0; step < _StepCount; step++)
                {
                    // Converter object space (-0.5 a 0.5) para texture space (0 a 1)
                    float3 uvw = currentPos + 0.5;
                    
                    // Verificar se está dentro do volume
                    if (all(uvw >= 0) && all(uvw <= 1))
                    {
                        float density = tex3Dlod(_DensityTex, float4(uvw, 0)).r;
                        
                        if (density > 0.001)
                        {
                            // Cor da fumaça/nuvem
                            float3 color = float3(1, 1, 1) * density * _Intensity;
                            float alpha = density * _Intensity * stepSize * 100;
                            
                            // Composição alpha blending (front-to-back)
                            finalColor.rgb += color * alpha * (1.0 - finalColor.a);
                            finalColor.a += alpha * (1.0 - finalColor.a);
                            
                            // Early exit se já estiver opaco
                            if (finalColor.a >= 0.95)
                                break;
                        }
                    }
                    
                    currentPos += rayDir * stepSize;
                }
                
                return saturate(finalColor);
            }
            ENDCG
        }
    }
}
