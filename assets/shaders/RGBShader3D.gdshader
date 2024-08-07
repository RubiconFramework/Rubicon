shader_type spatial;
render_mode unshaded;

uniform vec3 r:source_color = vec3(0.0, 0.0, 0.0);
uniform vec3 g:source_color = vec3(0.0, 0.0, 0.0);
uniform vec3 b:source_color = vec3(0.0, 0.0, 0.0);
uniform float mult = 1.0;
uniform sampler2D TEXTURE;

varying vec4 modulate;
void fragment() 
{
	vec4 col = texture(TEXTURE, UV);
	if (col.a > 0.0 && mult > 0.0)
	{	
		vec4 newCol = col;
		vec3 minCol = min(col.r * r + col.g * g + col.b * b, vec3(1.0));
		
		newCol.r = minCol.r;
		newCol.g = minCol.g;
		newCol.b = minCol.b;
		newCol.a = COLOR.a; // Copying Modulate.A, can't copy anything else tbh :/
		
		vec4 finalCol = mix(col, newCol, mult) * modulate;
		ALBEDO = finalCol.rgb;
		ALPHA = finalCol.a;
	}
}

void vertex()
{
	modulate = COLOR;
}
