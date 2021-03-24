#version 430

in vec2 texCoord;
out vec4 out_color;

uniform int startFrequencyElementIndex;
uniform int endFrequencyElementIndex;
uniform float minAmplitude;
uniform float maxAmplitude;

const vec4 black = vec4(0.0, 0.0, 0.0, 1.0);
const vec4 white = vec4(1.0, 1.0, 1.0, 1.0);

layout(std430, binding = 0) buffer Input0 {
	float data[];
} AmplitudesBuffer;

void main()
{
	int width = endFrequencyElementIndex - startFrequencyElementIndex;
	float frequencyWidth = 1.0 / (int)width;
	int currentFrequencyIndex = floor(texCoord.x / frequencyWidth);
	float amplitude = AmplitudesBuffer.data[currentFrequencyIndex];
	float pixelRelativeAmplitude = mix(minAmplitude, maxAmplitude, texCoord.y);

	if (pixelRelativeAmplitude <= amplitude)
	{
		out_color = white;
	}
	else
	{
		out_color = black;
	}
}