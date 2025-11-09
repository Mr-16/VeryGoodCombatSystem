shader_type canvas_item;

// ============ 🔧 可调参数 ============
uniform vec2 texture_size = vec2(64.0, 64.0);
uniform vec4 base_color = vec4(1.0, 0.0, 0.0, 1.0);
uniform float color_wave_strength = 1;
uniform float flash_speed = 5.0;
uniform float flash_strength = 0.5;
uniform float scale_speed = 3.0;
uniform float scale_range = 0.5;
uniform float edge_softness = 4.0; // 🔸 边缘柔和度(1~4，越大越柔)
uniform float mix_strength = 0.5;  // 🔸 纹理与自定义颜色混合比例(0=只用纹理,1=只用shader颜色)

// ============ 🔹 缩放动画 ============
void vertex() {
    float scale = 1.0 + (sin(TIME * scale_speed) * 0.5 + 0.5) * scale_range;
    vec2 center = texture_size * 0.5;
    VERTEX -= center;
    VERTEX *= scale;
    VERTEX += center;
}

// ============ 🔸 火焰视觉效果 ============
void fragment() {
    // 1️⃣ 圆形渐变遮罩
    float dist = distance(UV, vec2(0.5, 0.5));
    float circle_gradient = 1.0 - dist * 1.414;
    circle_gradient = clamp(circle_gradient, 0.0, 1.0);

    // 柔光边缘（指数控制边缘模糊程度）
    circle_gradient = pow(circle_gradient, edge_softness);

    // 2️⃣ 闪烁亮度
    float flash = 1.0 + sin(TIME * flash_speed) * flash_strength;
    float final_brightness = circle_gradient * flash;

    // 3️⃣ 颜色波动
    float color_wave = (sin(TIME) * 0.5 + 0.5) * color_wave_strength;
    vec4 final_color = vec4(
        base_color.r,
        base_color.g + color_wave,
        base_color.b + color_wave * 0.5,
        base_color.a
    ) * final_brightness;

    // 4️⃣ 混合纹理（有贴图时叠加）
    vec4 tex = texture(TEXTURE, UV);
    vec4 mixed_color = mix(tex, final_color, mix_strength);

    // 5️⃣ 使用圆形遮罩控制透明度
    mixed_color.a *= circle_gradient;

    COLOR = mixed_color;
}
