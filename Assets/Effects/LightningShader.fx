#define PI 3.1415926535

texture MainTex;
sampler sampler_Main = sampler_state {
    Texture = <MainTex>;
    AddressU = CLAMP;
    AddressV = CLAMP;
    Filter = LINEAR;
};

float uTime;
float2 uResolution;
float4 uColor = float4(1,1,1,1);
float uNoiseScale = 1.0;
float uSpeed = 1.0;
float uThickness = 0.05;
float uGlow = 2.0;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float rand(float2 n) {
    return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453);
}

float noise(float2 p) {
    float2 i = floor(p);
    float2 f = frac(p);
    float2 u = f * f * (3.0 - 2.0 * f);
    return lerp(
        lerp(rand(i), rand(i + float2(1,0)), u.x),
        lerp(rand(i + float2(0,1)), rand(i + float2(1,1)), u.x),
        u.y
    );
}

float lightning(float2 uv, float t) {
    float n = noise(uv * float2(2.0, 1.0) * uNoiseScale + t * uSpeed) * 2.0 - 1.0;
    n *= noise(uv * float2(5.0, 1.0) + t * uSpeed * 1.5) * 0.5;
    n *= noise(uv * float2(10.0, 1.0) + t * uSpeed * 2.0) * 0.25;

    float d = abs(uv.x + n);
    float arc = 1.0 - smoothstep(0.0, uThickness, d);
    arc = pow(arc, uGlow);
    return arc;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv_center = input.TextureCoordinates - float2(0.5, 0.5);
    float arc = lightning(uv_center, uTime);
    float4 color = uColor * arc;
    color.a = arc;
    return color;
}

technique Tech {
    pass P0 {
        PixelShader = compile ps_3_0 MainPS();
    }
}