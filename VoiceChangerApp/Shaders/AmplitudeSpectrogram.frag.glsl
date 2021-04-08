#version 430

in vec2 texCoordV;
out vec4 out_color;

uniform int startFrequencyElementIndex;
uniform int endFrequencyElementIndex;
uniform float minAmplitude;
uniform float maxAmplitude;
uniform vec2 mouseUV;
uniform mat4 MVP;

const vec4 background = vec4(0.1, 0.1, 0.1, 1.0);
const vec4 diagram = vec4(0.9, 0.9, 0.9, 1.0);
const vec4 highlight = vec4(0.0, 0.7, 0.0, 1.0);

layout(std430, binding = 0) buffer Input0 {
	float data[];
} AmplitudesBuffer;

int getFrequencyIndex(float u)
{
	int width = endFrequencyElementIndex - startFrequencyElementIndex;
	float frequencyWidth = 1.0 / int(width);
	int frequencyIndex = int(floor(u / frequencyWidth));
	return frequencyIndex;
}

void main()
{
	int currentFrequencyIndex = getFrequencyIndex(texCoordV.x);
	float amplitude = AmplitudesBuffer.data[currentFrequencyIndex];
	//int frequenciesInOneTexel = 
	/*for (int i = currentFrequencyIndex + 1; i < currentFrequencyIndex + ; ++i)
	{
		float currentAmplitude = AmplitudesBuffer.data[i];
		if (currentAmplitude > amplitude)
		{
			amplitude = currentAmplitude;
		}
	}*/

	float pixelRelativeAmplitude = mix(minAmplitude, maxAmplitude, texCoordV.y);

	int mousePointerFrequencyIndex = getFrequencyIndex(mouseUV.x);
	float mouseRelativeAmplitude = mix(minAmplitude, maxAmplitude, mouseUV.y);

	if (pixelRelativeAmplitude <= amplitude)
	{
		if (mousePointerFrequencyIndex == currentFrequencyIndex && mouseRelativeAmplitude <= amplitude)
		{
			out_color = highlight;
		}
		else
		{
			out_color = diagram;
		}
	}
	else
	{
		out_color = background;
	}
}