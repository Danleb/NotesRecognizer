#version 330

layout(location = 0) in vec2 vertexPosition;
uniform mat4 MVP;

void main()
{
	vec4 v = vec4(vertexPosition, 1, 1);
	gl_Position = MVP * v;
}