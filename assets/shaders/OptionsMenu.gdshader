shader_type canvas_item;

uniform sampler2D noise_tex;
uniform float rain_strength: hint_range(0.0, 2.0) = 1.0;
uniform float distortion_scale: hint_range(0.1, 50.0) = 10.0;
uniform float distortion_speed: hint_range(0.0, 20.0) = 5.0;
uniform float rain_direction: hint_range(0.0, 360.0) = 90.0;

void fragment() {
    float rain_dir_rad = radians(rain_direction);
    vec2 rain_direction_vec = vec2(cos(rain_dir_rad), sin(rain_dir_rad));
    vec2 noise_uv = UV * distortion_scale + rain_direction_vec * TIME * distortion_speed;
    noise_uv = fract(noise_uv);
    vec2 noise_vector = (texture(noise_tex, noise_uv).rg * 2.0 - 1.0) * rain_strength;
    vec2 distorted_uv = UV + noise_vector / distortion_scale;
    distorted_uv = clamp(distorted_uv, 0.0, 1.0);
    vec4 tex_color = texture(TEXTURE, distorted_uv);
    COLOR = tex_color * texture(TEXTURE, UV);
}
