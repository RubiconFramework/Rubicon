shader_type canvas_item;

uniform vec4 black:source_color = vec4(1.0, 0.0, 0.0, 1.0);
uniform vec4 white:source_color = vec4(0.0, 1.0, 0.0, 1.0);
uniform float value = 0.5;

// This is a port of HoloFunk's bar shader to Godot, written by one of my best
// buddies, firubii. Don't expect me to know what all of this does.
void fragment() 
{
	vec4 col = texture(TEXTURE, UV);
	
	float lerpWeight = value <= col.r ? 1.0 : 0.0;
	vec4 colorLerp = mix(black, white, lerpWeight);
	
	float endAlpha = colorLerp.a * col.a;
	COLOR = colorLerp;
	COLOR.a = endAlpha;
}