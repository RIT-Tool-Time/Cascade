SamplerState gSampler : register(s0);
Texture2D<float4> depthTexture : register(t1);
SamplerState depthSampler : register(s1) = sampler_state
{
	Texture=(depthTexture);
};

Texture2D<float4> depthCoordTexture : register(t2);
SamplerState depthCoordSampler : register(s2) = sampler_state
{
	Texture=(depthCoordTexture);
};
float4x4 MatrixTransform : register(vs, c0);
void SpriteVertexShader(inout float4 color    : COLOR0,  
                        inout float2 texCoord : TEXCOORD0,  
                        inout float4 position : SV_Position)  
{  
    position = mul(position, MatrixTransform);  
}  

float4 NormalShader(float4 pos: POSITION, float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	return tex2D(gSampler, tex) * color;
}

float4 NormalDepthShader(float4 pos: POSITION, float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
    float2 dtex = tex2D(depthCoordSampler, tex);
	return tex2D(gSampler, tex) * (1 - tex2D(depthSampler, dtex).r);
}

technique Normal
{
pass p1
	{
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 NormalShader();
	}
}

technique NormalDepth
{
pass p1
	{
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 NormalDepthShader();
	}
}
