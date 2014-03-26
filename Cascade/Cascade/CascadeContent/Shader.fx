float4x4 World;
float4x4 View;
float4x4 Projection;

float depth = 0;
float alpha =1;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 Pos : TEXCOORD0;
	float4 Color : COLOR0;
};

struct mrt { float4 col0 : COLOR0; float4 col1 : COLOR1; };

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
	float4 projPosition = mul(viewPosition, Projection);
    output.Position = projPosition;
	output.Pos = projPosition;
	output.Color = input.Color * alpha;
    return output;
}

mrt PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    mrt m = (mrt)0;
	m.col0 = input.Color;
	float z = input.Pos.z;
	m.col1= float4( depth, 0, 0, 1);
	//mrt.col
    return m;
}

technique Normal
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
