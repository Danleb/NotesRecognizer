#version 430

in vec2 texCoordV;
out vec4 out_color;

uniform int startFrequencyElementIndex;
uniform int endFrequencyElementIndex;
uniform float minAmplitude;
uniform float maxAmplitude;
uniform vec2 mousePosition;
uniform mat4 MVP;

const vec4 black = vec4(0.0, 0.0, 0.0, 1.0);
const vec4 white = vec4(1.0, 1.0, 1.0, 1.0);
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
	float pixelRelativeAmplitude = mix(minAmplitude, maxAmplitude, texCoordV.y);

	vec4 v = vec4(mousePosition, 0.0, 1.0);
	vec4 mouseUV = v * MVP;
	float x = (mouseUV.x + 1.0) / 2.0;
	float y = (mouseUV.y + 1.0) / 2.0;
	int mousePointerFrequencyIndex = getFrequencyIndex(x);
	float mouseRelativeAmplitude = mix(minAmplitude, maxAmplitude, y);

	//vec2 v = (texCoordV + 1.0) / 2.0;
	//out_color = vec4(v, 0.0, 1.0);	
	//out_color = vec4(texCoordV, 0.0, 1.0);

	//todo anyway drawing 1px width;

	/*if (x > texCoordV.x)
	{
		out_color = highlight;
	}
	else
	{
		out_color = white;
	}*/

	if (pixelRelativeAmplitude <= amplitude)
	{
		if (mousePointerFrequencyIndex == currentFrequencyIndex && mouseRelativeAmplitude <= amplitude)
		{
			out_color = highlight;
		}
		else
		{
			out_color = white;
		}
	}
	else
	{
		out_color = black;
	}
}