shader_type canvas_item;

uniform vec2 camOffset = vec2(0.0,0.0);

void vertex() {
	VERTEX += camOffset;
}

void fragment() {
	// Called for every pixel the material is visible on.
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
