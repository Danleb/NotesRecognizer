#version 330

out vec4 out_color;
uniform vec4 color;

void main()
{
	out_color = color;
	//out_color = vec4(color.rgb, 1.0);
	//out_color = vec4(0.0, 1.0, 1.0, 1.0);
}