Shader "Custom/SuperScreenURP"
{
    Properties
    {
        _GlobalSaturation ("Saturation", Range(0, 1)) = 1
        _GlobalBrightness ("Brightness", Range(0, 1)) = 1
        _SpotRadius ("Spot Radius", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Core libraries laden
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            float _GlobalSaturation;
            float _GlobalBrightness;
            float _SpotRadius;
            
            // Arrays voor spots
            float4 _SpotPositions[10];
            int _SpotCount;

            v2f vert (appdata v)
            {
                v2f o;
                // FIX WAARSCHUWING: We gebruiken .xyz om expliciet 3 coordinaten door te geven
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // Haal de UV coordinaten van het scherm op
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                
                // FIX ERROR: We gebruiken nu 'half3' omdat SampleSceneColor geen Alpha teruggeeft
                half3 col = SampleSceneColor(screenUV);

                // --- 1. SPOT LOGICA ---
                float totalSpotInfluence = 0;
                for (int k = 0; k < 10; k++)
                {
                    if (k >= _SpotCount) break;
                    
                    float dist = distance(screenUV, _SpotPositions[k].xy);
                    float spotMask = 1.0 - smoothstep(_SpotRadius * 0.5, _SpotRadius, dist);
                    totalSpotInfluence = max(totalSpotInfluence, spotMask);
                }

                // Maak grijswaarde (Luminance)
                float lum = dot(col, float3(0.299, 0.587, 0.114));
                float3 gray = float3(lum, lum, lum);

                // Pas vlekken toe (plekken worden grijs)
                // We mixen hier RGB waarden
                col = lerp(col, gray, totalSpotInfluence);

                // --- 2. GLOBAL FADE LOGICA ---
                col = lerp(gray, col, _GlobalSaturation);
                col *= _GlobalBrightness;

                // Zet het uiteindelijk om naar 4 componenten (RGB + Alpha 1)
                return half4(col, 1);
            }
            ENDHLSL
        }
    }
}