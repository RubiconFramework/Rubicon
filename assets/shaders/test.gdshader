shader_type canvas_item;

uniform float Strength : hint_range(0.0, 1.0);
uniform float pixel;
uniform vec4 modulate : source_color;

const bool allowWiggle = true;

float outCirc(float t) {
    return sqrt(-t * t + 2.0 * t);
}

float rand(vec2 co) {
    return fract(sin(dot(co.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

void fragment() {
    vec3 col;
    float amp;
    
    amp = Strength;

    for (int i = 0; i < 3; i++) {
        vec2 size = vec2(textureSize(TEXTURE, 0)) / pixel;
        vec2 uv = floor(UV * size) / size;

        if (allowWiggle) uv += vec2(sin(float(i) * amp), cos(float(i) * amp)) * amp * 0.05;
       
        vec3 texOrig = texture(TEXTURE, uv).rgb;

        uv.x += (rand(vec2(uv.y + float(i), TIME)) * 2.0 - 1.0) * amp * 0.8 * (texOrig[i] + 0.2);
        uv.y += (rand(vec2(uv.x, TIME + float(i))) * 2.0 - 1.0) * amp * 0.1 * (texOrig[i] + 0.2);

        vec3 tex = texture(TEXTURE, uv).rgb;
        tex += abs(tex[i] - texOrig[i]);
        tex *= rand(uv) * amp + 1.0;
        col[i] = tex[i];
    }
    
    vec4 origTex = texture(TEXTURE, UV);
    // Apply modulate color
    col *= modulate.rgb;
    COLOR = vec4(col, origTex.a) * modulate.a;
}
