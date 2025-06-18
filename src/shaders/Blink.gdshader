shader_type canvas_item;

uniform vec3 blink_color = vec3(1.0,1.0,1.0);
uniform float blink_speed = 2.0f;
uniform bool run = true;

void fragment() 
{
	if(run)
	{
		// Fracture of TIME means we go from 0.(0) to 0.(9) meaning it's a 0-9 counter
		float timer = fract(TIME * blink_speed);
		// If it's below 0.5 blink is 0, otherwise 1
		float blink = step(0.5, timer);
		vec4 tex_color = texture(TEXTURE, UV);
		if(blink == 1.0)
		{
			COLOR = vec4(blink_color, tex_color.a);
		}
	}
}