shader_type canvas_item;

uniform vec3 r:source_color = vec3(0.0, 0.0, 0.0);
uniform vec3 g:source_color = vec3(0.0, 0.0, 0.0);
uniform vec3 b:source_color = vec3(0.0, 0.0, 0.0);
uniform float mult = 1.0;

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
		newCol.a = COLOR.a;
		
		vec4 finalCol = mix(col, newCol, mult);
		finalCol.r *= modulate.r;
		finalCol.g *= modulate.g;
		finalCol.b *= modulate.b;
		finalCol.a = COLOR.a;
		
		COLOR = finalCol;
	}
}

void vertex()
{
	modulate = COLOR;
}
