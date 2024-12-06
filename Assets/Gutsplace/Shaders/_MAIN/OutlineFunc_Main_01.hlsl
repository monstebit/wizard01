#ifndef OUTLINE_FUNC_HLSL
#define OUTLINE_FUNC_HLSL

sampler2D _CameraDepthTexture;
sampler2D _CameraNormalsTexture;
float4 _CameraColorTexture_TexelSize;

float GetDepth(float2 uv)
{
    return tex2D(_CameraDepthTexture, uv).r;
}

float3 GetNormal(float2 uv)
{
    return tex2D(_CameraNormalsTexture, uv).rgb * 2.0 - 1.0;;
}

// void Outline_float(float2 UV, float DepthThreshold, float NormalEdgeBias, float DepthEdgeStrength, float NormalEdgeStrength, out float Outline)  //  SOURCE
void Outline_float(
    float2 UV,
    float DepthThreshold,
    float NormalEdgeBias,
    float DepthEdgeStrength,
    float NormalEdgeStrength,
    float4 baseColor,
    float darkFactor,
    out float Outline)
{
    float2 texelSize = _CameraColorTexture_TexelSize.xy;

    // Получаем глубину и нормаль для текущего пикселя
    float depth = GetDepth(UV);
    float3 normal = GetNormal(UV);

    // Координаты соседних пикселей
    float2 uvs[4];
    uvs[0] = UV + float2(0.0, texelSize.y);
    uvs[1] = UV - float2(0.0, texelSize.y);
    uvs[2] = UV + float2(texelSize.x, 0.0);
    uvs[3] = UV - float2(texelSize.x, 0.0);

    // Get Depth Edge Indicator
    float depths[4];
    
    // --- Вычисление краев по глубине ---
    float depthDifference = 0.0;
    [unroll]
    for (int i = 0; i < 4; i++)
    {
        depths[i] = GetDepth(uvs[i]);
        depthDifference += depth - depths[i];
    }
    float depthEdge = step(DepthThreshold, depthDifference);

    // --- Вычисление краев по нормали ---
    float3 normals[4];
    float dotSum = 0.0;
    [unroll]
    for (int j = 0; j < 4; j++)
    {
        normals[j] = GetNormal(uvs[j]);
        float3 normalDiff = normal - normals[j];

        // Edge pixels should yield to faces closer to the bias direction.
        float normalBiasDiff = dot(normalDiff, NormalEdgeBias);
        float normalIndicator = smoothstep(-.01, .01, normalBiasDiff); // step(0, normalBiasDiff);

        /*
        // Only the shallower pixel should detect the normal edge.
        float depthDiff = depth[j] - depth;
        float depthIndicator = step(0, depthDiff * .25 + .0025);
        */
        
        dotSum += dot(normalDiff, normalDiff) * normalIndicator; // * depthIndicator;
    }
    
    float indicator = sqrt(dotSum);
    float normalEdge = step(NormalEdgeBias, indicator);

    // Refuse normal outline if the depthEdge is negative and make it a depth edge if its above the threshold
    // OutlineColor = depthDifference < 0 ? 0 : (depthEdge > 0.0 ? (DepthEdgeStrength * depthEdge) : (NormalEdgeStrength * normalEdge));    //  SOURCE
    
    //  ON TESTING
    if (depthDifference < 0)
        Outline = baseColor;
    else if (depthEdge > 0.0)
        Outline = baseColor * (1 - darkFactor) + DepthEdgeStrength * depthEdge;
    else
        Outline = baseColor + NormalEdgeStrength * normalEdge;
}

#endif