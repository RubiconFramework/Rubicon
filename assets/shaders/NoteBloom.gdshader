shader_type canvas_item;

void fragment() {
	COLOR *= 1.0 + (abs(sin(TIME * 2.0)) * 1.5);
}
