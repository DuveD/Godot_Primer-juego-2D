shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0);

// Color de la zona visible (blanco)
uniform vec4 inner_color : source_color = vec4(0.9, 0.9, 0.85, 1.0); // #FFFFFF

// Color de la cortina (negro)
uniform vec4 outer_color : source_color = vec4(0.0, 0.0, 0.0, 1.0); // #000000

void fragment() {
    float y = UV.y;

    // Border avanza desde 0 a 0.5 (top/bottom hacia centro)
    float border = mix(0.0, 0.5, progress);

    float isInside = step(border, y) * step(y, 1.0 - border);

    // Mezcla entre negro y blanco según la posición
    vec4 final_color = mix(outer_color, inner_color, isInside);

    COLOR = final_color;
}
