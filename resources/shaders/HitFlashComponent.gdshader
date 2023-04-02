shader_type canvas_item;

uniform vec4 flash_colour: source_color;
uniform float flash_weight: hint_range(0.0, 1.0);

void fragment() {
	vec4 tex_col = texture(TEXTURE, UV);
	vec4 final_col = mix(tex_col, flash_colour, flash_weight) * tex_col.a;
	
	COLOR = final_col;
}
