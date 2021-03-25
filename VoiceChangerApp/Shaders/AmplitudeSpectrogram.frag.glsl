#version 430

in vec2 texCoordV;
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
	float frequencyWidth = 1.0 / int(width);
	int currentFrequencyIndex = int(floor(texCoordV.x / frequencyWidth));
	float amplitude = AmplitudesBuffer.data[currentFrequencyIndex];
	float pixelRelativeAmplitude = mix(minAmplitude, maxAmplitude, texCoordV.y);

	//vec2 v = (texCoordV + 1.0) / 2.0;
	//out_color = vec4(v, 0.0, 1.0);	
	//out_color = vec4(texCoordV, 0.0, 1.0);

	if (pixelRelativeAmplitude <= amplitude)
	{
		out_color = white;
	}
	else
	{
		out_color = black;
	}
}