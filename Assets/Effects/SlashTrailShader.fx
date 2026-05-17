// 用于Alpha裁切的材质
sampler AlphaTexture : register(s0);
// 用于底部的噪波
sampler NoiseTexture : register(s1);
// 用于溶解效果
sampler DissolveTexture : register(s2);
// UV的坐标偏移
float2 UVOffset;
// 噪波材质的缩放
float2 NoiseTextureScale;
// 溶解的阈值：强度1为完全溶解，0为全部显示
float DissolveThreshold;
// 溶解材质的缩放
float2 DissolveTextureScale;
// 溶解材质的坐标偏移
float2 DissolveTexturUVOffset;
// 是否使用Alpha裁切用材质的透明度
bool UseAlphaMult;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 SlashTrailShader(VertexShaderOutput input) : COLOR
{
    // 进行alpha裁切
    float4 baseColor = tex2D(AlphaTexture, input.TextureCoordinates);
    if (baseColor.r == 0)
        discard;
    // 溶解材质
    float2 DissolveTextureUV = input.TextureCoordinates * DissolveTextureScale;
    DissolveTextureUV = frac(DissolveTextureUV);
    float4 DissolveColor = tex2D(DissolveTexture, DissolveTextureUV + DissolveTexturUVOffset);
    if (DissolveColor.r < DissolveThreshold)
        discard;
    // 溶解后贴上噪波材质
    float2 NoiseTextureUV = input.TextureCoordinates * NoiseTextureScale;
    NoiseTextureUV = frac(NoiseTextureUV);
    float4 NoiseColor = tex2D(NoiseTexture, NoiseTextureUV + UVOffset);
    // 输出
    if (UseAlphaMult) return NoiseColor * input.Color * baseColor.r;
    else return NoiseColor * input.Color;
}

technique SpriteDrawing
{
    pass LAPSlashTrailShaderPass
    {
        PixelShader = compile ps_3_0 SlashTrailShader();
    }
};