Shader "Custom/ParticleRenderer"
{
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Particle
            {
                float3 position;
            };

            StructuredBuffer<Particle> particles;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (uint id : SV_VertexID)
            {
                v2f o;
                float3 worldPos = particles[id].position;

                // This is the most critical line.
                // It must be UnityWorldToClipPos.
                o.vertex = UnityWorldToClipPos(worldPos);
                
                o.color = float4(0.8, 0.9, 1.0, 0.5);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}