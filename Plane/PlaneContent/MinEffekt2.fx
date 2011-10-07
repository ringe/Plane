float4x4 World;
float4x4 View;
float4x4 Projection;

float fx_Red;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Color = input.Color;

    return output;
}

float4 changeRed(float4 inn) {
	float4 f4color;

	f4color.r = fx_Red;
	f4color.g = inn.g;
	f4color.b = inn.b;
	f4color.a = 1.0f;

	return f4color;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return input.Color * changeRed(input.Color);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
