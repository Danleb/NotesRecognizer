#version 330

in vec2 position;
in vec2 texCoord;

out vec2 texCoordV;

uniform mat4 MVP;

void main()
{
	texCoordV = texCoord;
	vec4 v = vec4(position, 0.0, 1.0);
	gl_Position = MVP * v;
}