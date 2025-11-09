shader_type canvas_item;

uniform vec4 base_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float trail_strength : hint_range(0.0, 1.0) = 0.7;
uniform float trail_length : hint_range(0.0, 1.0) = 0.3;
uniform vec4 trail_color : source_color = vec4(0.2, 0.6, 1.0, 1.0);
uniform float fade_speed : hint_range(0.1, 5.0) = 1.0;

uniform bool flash_mode = false;
uniform vec2 move_dir = vec2(1.0, 0.0);
uniform float flash_time = 0.0;

void fragment() {
    // 改：用自定义 base_color 而不是 COLOR
    vec4 base = base_color;

    float t = clamp(1.0 - flash_time * fade_speed, 0.0, 1.0);

    vec4 trail = trail_color;
    trail.a *= trail_strength * t;

    vec4 result = mix(base, base + trail * trail.a, float(flash_mode));
    COLOR = result;
}
