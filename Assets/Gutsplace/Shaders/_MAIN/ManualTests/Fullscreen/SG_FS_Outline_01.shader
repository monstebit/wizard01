Shader "Custom/Outline_01"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DepthThreshold ("Depth Threshold", Float) = 0.0005
        _NormalEdgeBias ("Normal Edge Bias", Float) = 0.01
        _OutlineThickness ("Outline Thickness", Float) = 0.6
        _DarkenFactor ("Outline Darken Factor", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler2D _NormalTex;
            float4 _CameraColorTexture_TexelSize;
            float _DepthThreshold;
            float _NormalEdgeBias;
            float _OutlineThickness;
            float _DarkenFactor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float GetDepth(float2 uv)
            {
                return tex2D(_CameraDepthTexture, uv).r;
            }
            
            float3 GetNormal(float2 uv)
            {
                return tex2D(_NormalTex, uv).rgb * 2.0 - 1.0;
            }
            
            void Outline_Float(float2 UV, out float Outline, out float3 OutlineColor, float3 baseColor)
            {
                float2 texelSize = _CameraColorTexture_TexelSize.xy * _OutlineThickness;
                float depth = GetDepth(UV);
                float3 normal = GetNormal(UV);

                float2 uvs[4] = {
                    UV + float2(0.0, texelSize.y),
                    UV - float2(0.0, texelSize.y),
                    UV + float2(texelSize.x, 0.0),
                    UV - float2(texelSize.x, 0.0)
                };

                float depthDifference = 0.0;
                float normalDifference = 0.0;

                [unroll]
                for (int i = 0; i < 4; i++)
                {
                    float neighborDepth = GetDepth(uvs[i]);
                    float3 neighborNormal = GetNormal(uvs[i]);

                    depthDifference = max(depthDifference, abs(depth - neighborDepth));
                    normalDifference += dot(normal - neighborNormal, normal - neighborNormal);
                }

                float depthEdge = step(_DepthThreshold, depthDifference);
                float normalEdge = step(_NormalEdgeBias, normalDifference);
                
                Outline = depthEdge + normalEdge;

                if (depthEdge > 0.0)
                {
                    OutlineColor = baseColor * (1.0 - _DarkenFactor); // Затеняем основной цвет объекта
                }
                else if (normalEdge > 0.0)
                {
                    OutlineColor = baseColor * (1.0 + _DarkenFactor); // Затеняем основной цвет объекта
                }
                else
                {
                    OutlineColor = float3(0, 0, 0); // Без обводки
                }
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float outline;
                float3 outlineColor;
                fixed4 col = tex2D(_MainTex, i.uv);

                // Обновляем вызов функции Outline_Float
                Outline_Float(i.uv, outline, outlineColor, col.rgb);

                // Применение цвета контура в зависимости от того, как он был определён
                if (outline > 0.0)
                {
                    col.rgb = lerp(col.rgb, outlineColor, smoothstep(0.0, 1.0, outline * _OutlineThickness));
                }

                return col;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}