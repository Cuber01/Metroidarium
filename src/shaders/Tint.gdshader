shader_type canvas_item;

uniform float blue_tint = -1;
uniform float red_tint = -1;
uniform float green_tint = -1;

void fragment() {
	COLOR = texture(TEXTURE, UV);
	if(red_tint >= 0.0)
	{
		COLOR.r = red_tint;
	}
	if(blue_tint >= 0.0)
	{
		COLOR.g = green_tint;
	}
	if(green_tint >= 0.0)
	{
		COLOR.b = blue_tint;
	}
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
