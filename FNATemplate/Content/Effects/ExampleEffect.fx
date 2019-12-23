
// A simple example effect for FNA-Template.


sampler TextureSampler : register(s0);

float4 PixelShaderFunction(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 tex = tex2D(TextureSampler, texCoord);
	
	return tex * float4(texCoord.x, texCoord.y, 1, 1);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
