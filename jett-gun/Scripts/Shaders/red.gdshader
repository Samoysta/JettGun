shader_type canvas_item;

uniform float blink_speed = 8.0; // saniyedeki blink sayısı
uniform bool blinking = false;

void fragment() {
    vec4 tex = texture(TEXTURE, UV);

    if (blinking) {
        float t = sin(TIME * blink_speed);

        if (t > 0.0) {
            tex.rgb = vec3(1.0, 0.0, 0.0); // TAM KIRMIZI
        }
    }

    COLOR = vec4(tex.rgb, tex.a);
}
