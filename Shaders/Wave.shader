shader_type canvas_item;

uniform float sinConst = 0.2;
uniform float timeConst = 2.0;
uniform vec4 color : hint_color = vec4(1, 1, 1, 1);

void fragment() {
	vec2 uvOffset;
	uvOffset.x = cos(TIME * timeConst + UV.x + UV.y) * sinConst;
	uvOffset.y = sin(TIME * timeConst + UV.x * 10f + UV.y) * sinConst;
	
	COLOR = texture(TEXTURE, UV + uvOffset);
	COLOR.rgb = color.rgb;
}
