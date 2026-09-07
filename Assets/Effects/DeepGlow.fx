sampler uImage0 : register(s0);
float2 uTexelSize; // 像素大小
float uThreshold; // 发光阈值
float uIntensity; // 发光强度倍率
float uBlurRadius; // 模糊半径 (默认 1.0 即可，增大可以产生更宽的光晕，但可能产生伪影)
float uSoftKnee; // 控制平滑过渡范围
// 提取高光
// 只有亮度超过阈值的像素会被保留，用于生成光源
float4 PassPrefilter(float2 uv : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, uv);
    // 计算感知亮度
    float brightness = dot(color.rgb, float3(0.3, 0.59, 0.11));
    // 计算 Soft Knee
    float knee = uThreshold * uSoftKnee;
    float soft = brightness - uThreshold + knee;
    soft = clamp(soft, 0.0, 2.0 * knee);
    soft = (soft * soft) / (4.0 * knee + 0.00001); // 平滑曲线
    // 平滑过渡防止高光边缘出现生硬的锯齿
    float contribution = max(soft, brightness - uThreshold);
    contribution /= max(brightness, 0.00001); // 防止除以0
    // 返回提取后的高光，Alpha 设为 1.0 方便后续 Additive 混合
    return float4(color.rgb * contribution * uIntensity, 1.0);
}

// 第二种提取高光的方法，不是查看灰度，而是查看最亮的颜色通道
// 只有亮度超过阈值的像素会被保留，用于生成光源
float4 PassPrefilter2(float2 uv : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, uv);
    // 计算感知亮度
    float brightness = max(color.r, max(color.g, color.b));
        // 计算 Soft Knee
    float knee = uThreshold * uSoftKnee;
    float soft = brightness - uThreshold + knee;
    soft = clamp(soft, 0.0, 2.0 * knee);
    soft = (soft * soft) / (4.0 * knee + 0.00001); // 平滑曲线
    // 平滑过渡防止高光边缘出现生硬的锯齿
    float contribution = max(soft, brightness - uThreshold);
    contribution /= max(brightness, 0.00001); // 防止除以0
    // 返回提取后的高光，Alpha 设为 1.0 方便后续 Additive 混合
    return float4(color.rgb * contribution * uIntensity, 1.0);
}

// 4-TapKawase降采样
float4 PassDownsample(float2 uv : TEXCOORD0) : COLOR0
{
    // 偏移量，利用半个像素的偏移可以抓取周围的颜色
    float2 offset = uTexelSize * 0.5 * uBlurRadius;
    float4 color = 0;
    // 采样周围 4 个方向
    color += tex2D(uImage0, uv + float2(-offset.x, -offset.y));
    color += tex2D(uImage0, uv + float2(offset.x, -offset.y));
    color += tex2D(uImage0, uv + float2(-offset.x, offset.y));
    color += tex2D(uImage0, uv + float2(offset.x, offset.y));
    // 取平均值
    color *= 0.25;
    // 强制 Alpha 为 1，防止渲染管线中奇怪的透明度叠加黑边
    return float4(color.rgb, 1.0);
}

// 9-TapKawase升采样
float4 PassUpsample(float2 uv : TEXCOORD0) : COLOR0
{
    float2 offset = uTexelSize * uBlurRadius;
    float4 color = 0;
    color += tex2D(uImage0, uv + float2(-offset.x, offset.y)) * 1.0;
    color += tex2D(uImage0, uv + float2(0, offset.y)) * 2.0;
    color += tex2D(uImage0, uv + float2(offset.x, offset.y)) * 1.0; 
    color += tex2D(uImage0, uv + float2(-offset.x, 0)) * 2.0;
    color += tex2D(uImage0, uv) * 4.0;
    color += tex2D(uImage0, uv + float2(offset.x, 0)) * 2.0;
    color += tex2D(uImage0, uv + float2(-offset.x, -offset.y)) * 1.0;
    color += tex2D(uImage0, uv + float2(0, -offset.y)) * 2.0;
    color += tex2D(uImage0, uv + float2(offset.x, -offset.y)) * 1.0;
    // 总权重为 1+2+1 + 2+4+2 + 1+2+1 = 16
    color /= 16.0;
    return float4(color.rgb, 1.0);
}

technique DeepGlow
{
    pass Prefilter
    {
        PixelShader = compile ps_3_0 PassPrefilter();
    }
    pass Prefilter2
    {
        PixelShader = compile ps_3_0 PassPrefilter2();
    }
    pass Downsample
    {
        PixelShader = compile ps_3_0 PassDownsample();
    }
    pass Upsample
    {
        PixelShader = compile ps_3_0 PassUpsample();
    }
}