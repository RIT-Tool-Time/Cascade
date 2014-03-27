
float aspect = 16.0/9.0;
float blurLevel = 0.01;
static float blurStep = 2.0 / 18.0;
SamplerState gSampler : register(s0)
{
	
};

Texture2D<float4> depthTexture : register(t1);

SamplerState depthSampler : register(s1) = sampler_state
{
	Texture = (depthTexture);
};

struct mrt { float4 col0 : COLOR0; float4 col1 : COLOR1; };

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

float4 BokehShader(float4 pos: POSITION, float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float blur = tex2D(depthSampler, tex).r;
	[branch]if (blur > 0.03)
	{
		float4 col = (float4)0;
		float4 col2 = (float4)0;
		float colMul = 0;
		float times = 0;
		float dist = 0;
		float b = 0;
		float2 pos2;
		for (float i = -1; i <= 1; i+=blurStep)
		{
			for (float o = -1; o <= 1; o+=blurStep)
			{
				dist = (i * i) + (o * o);
				[branch]if (dist < 0.6)
				{
					pos2 = float2(i, o * aspect) ;
					float b = tex2Dlod(depthSampler, float4(tex + (pos2 * blur * blurLevel), 0, 0)).r;
					
					col2 = tex2Dlod(gSampler, float4(tex + (pos2 * blurLevel * b), 0, 0));
					colMul = (pow(col2.r + col2.g + col2.b, 1) * (dist)) + 1;
					//colMul = dist + 0.3;
					col += col2 * colMul;
					times+= colMul;
				}
			}
		}
		return col / times;
	}
	else return tex2D(gSampler, tex);
	
}

technique Normal
{
	pass p1
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 NormalShader();
	}
}
technique Bokeh
{
	pass p1
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 BokehShader();
	}
}
