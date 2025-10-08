Shader "Custom/RayMarchShader"
{
    Properties
    {
        _DensityTex ("Density Texture", 3D) = "" {}
        _CloudColor ("Cloud Color", Color) = (0.85,0.9,1,1)
        _Extinction ("Extinction", Float) = 1.5
        _Steps ("Ray March Steps", Int) = 96
        _DebugBounds ("Debug Bounds", Int) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0

            #include "UnityCG.cginc"

            sampler3D _DensityTex;
            float3 _GridSize;
            float3 _BoundsMin;
            float3 _BoundsSize;
            float4 _CloudColor;    // rgb used
            float  _Extinction;
            int    _Steps;
            int    _DebugBounds;

            struct appdata
            {
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float4 wp = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = wp.xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            bool RayBox(float3 ro, float3 rd, float3 bmin, float3 bmax, out float tmin, out float tmax)
            {
                float3 inv = 1.0 / rd;
                float3 t0 = (bmin - ro) * inv;
                float3 t1 = (bmax - ro) * inv;
                float3 tsmaller = min(t0, t1);
                float3 tbigger  = max(t0, t1);
                tmin = max(tsmaller.x, max(tsmaller.y, tsmaller.z));
                tmax = min(tbigger.x,  min(tbigger.y,  tbigger.z));
                return tmax > max(tmin, 0.0);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 ro = _WorldSpaceCameraPos;
                float3 rd = normalize(i.worldPos - ro);

                float t0, t1;
                if (!RayBox(ro, rd, _BoundsMin, _BoundsMin + _BoundsSize, t0, t1))
                    discard;

                t0 = max(t0, 0.0);
                float dist = t1 - t0;

                int steps = max(4, _Steps);
                float stepSize = dist / steps;
                float3 startPos = ro + rd * t0;

                float3 accum = 0;
                float transmittance = 1.0;
                float3 boxColor = _CloudColor.rgb;

                // Substitua o loop no fragment shader:
                for (int s = 0; s < steps; s++)
                {
                  float3 p = startPos + rd * (s * stepSize + stepSize * 0.5); // Offset central
                  float3 uvw = (p - _BoundsMin) / _BoundsSize;
                  uvw = saturate(uvw);

                  float density = tex3Dlod(_DensityTex, float4(uvw, 0)).r;

                  if (density > 0.001) // Threshold maior
                  {
                    // Beer-Lambert law
                    float sigma_t = density * _Extinction;
                    float absorb = exp(-sigma_t * stepSize);

                    // In-scattering (phase function simplificada)
                    float3 luminance = boxColor * density;

                    accum += transmittance * (1.0 - absorb) * luminance * stepSize;
                    transmittance *= absorb;

                    if (transmittance < 0.01) break;
                  }
                }

                float alpha = 1 - transmittance;

                // Optional debug: show bounding box edges
                if (_DebugBounds == 1)
                {
                    float3 rel = (i.worldPos - (_BoundsMin + 0.5*_BoundsSize)) / (_BoundsSize * 0.5);
                    float edge = step(0.95, max(abs(rel.x), max(abs(rel.y), abs(rel.z))));
                    accum = lerp(accum, float3(1,0,0), edge);
                    alpha = max(alpha, edge * 0.3);
                }

                return float4(accum, alpha);
            }
            ENDCG
        }
    }
    FallBack Off
}
