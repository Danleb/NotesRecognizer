#version 330

in vec2 position;
in vec2 texCoord;

out vec2 texCoordV;

void main()
{
	texCoordV = texCoord;
	gl_Position = position;
}