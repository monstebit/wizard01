#ifndef OUTLINE_FUNC_HLSL
#define OUTLINE_FUNC_HLSL

sampler2D _CameraDepthTexture;
sampler2D _NormalTex;
float4 _CameraColorTexture_TexelSize;

float GetDepth(float2 uv)
{
    return tex2D(_CameraDepthTexture, uv).r;
}

float3 GetNormal(float2 uv)
{
    return tex2D(_NormalTex, uv).rgb * 2.0 - 1.0;
}

void Outline_float(float2 UV, float DepthThreshold, float NormalEdgeBias, float DepthEdgeStrength, float NormalEdgeStrength, out float Outline)
{
    float2 texelSize = _CameraColorTexture_TexelSize.xy;

    float depth = GetDepth(UV);
    float3 normal = GetNormal(UV);

    float2 uvs[4];
    uvs[0] = UV + float2(0.0, texelSize.y);
    uvs[1] = UV - float2(0.0, texelSize.y);
    uvs[2] = UV + float2(texelSize.x, 0.0);
    uvs[3] = UV - float2(texelSize.x, 0.0);

    // Get Depth Edge Indicator
    float depthDifference = 0.0;
    [unroll]
    for (int i = 0; i < 4; i++)
    {
        float neighborDepth = GetDepth(uvs[i]);
        depthDifference += abs(depth - neighborDepth);
    }
    float depthEdge = step(DepthThreshold, depthDifference);

    // Get Normal Edge Indicator
    float dotSum = 0.0;
    [unroll]
    for (int i = 0; i < 4; i++)
    {
        float3 neighborNormal = GetNormal(uvs[i]);
        float3 normalDiff = normal - neighborNormal;
        dotSum += dot(normalDiff, normalDiff);
    }
    float normalEdge = step(NormalEdgeBias, sqrt(dotSum));

    // Combine edges
    Outline = max(DepthEdgeStrength * depthEdge, NormalEdgeStrength * normalEdge);
}

#endif