//uniform : betyr at verdien er den samme for alle vertekser.
//extern:  gjør variabelen tilgjengelig fra XNA-programmet

uniform extern float4x4 fx_WVP;		// WorldViewProjection
float fx_Blue;

//Deklarerer en del strukter:
struct gtVertex 
{
    float4 f4Position : POSITION0;
    float4 f4Color : COLOR0;
};

struct gtPixelFormat
{
    float4 f4Position : POSITION0;
    float4 f4Color : COLOR0;
};

float4 changeValueBlue() {
	float4 f4color;

	f4color.r = fx_Blue;
	f4color.g = fx_Blue;
	f4color.b = fx_Blue;
	f4color.a = 1.0f;

	return f4color;
}

struct tScreenOutput
{
    float4 f4Color : COLOR0;
};

// Multipliserer inn-verteksen med WVP-matrisen:
void vertex_shader(in gtVertex IN, out gtPixelFormat OUT)
{
     // transform vertex
    OUT.f4Position = mul(IN.f4Position, fx_WVP);
    OUT.f4Color = IN.f4Color * changeValueBlue();
}

//Tar i mot det som verteksshaderen leverer fra seg og 
//leverer fra seg en tScreenOutput-struct, uendret:
void pixel_shader(in gtPixelFormat IN, out tScreenOutput OUT)
{
    float4 fColor = IN.f4Color;
	//Gjør ingenting med fargen, lever videre:
    OUT.f4Color = fColor;
}

// Shaderen starter her:
technique simple
{
    pass p0
    {
        vertexshader = compile vs_2_0 vertex_shader(); // declare and initialize vs
        pixelshader = compile ps_2_0 pixel_shader();   // declare and initialize ps
    }
}
