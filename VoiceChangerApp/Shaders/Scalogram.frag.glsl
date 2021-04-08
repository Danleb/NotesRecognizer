#version 430

in vec2 texCoordV;
out vec4 out_color;

//x
uniform int startSignalIndex;
uniform int endSignalIndex;
uniform int signalsCountInRow;
//y
uniform int startFrequencyElementIndex;
uniform int endFrequencyElementIndex;

//uniform float maxValue;
//uniform vec2 mouseUV;

layout(std430, binding = 0) buffer Input0 {
	float data[];
} ScalogramBuffer;

const vec4 background = vec4(0.1, 0.1, 0.1, 1.0);
const vec4 colorFrom = vec4(0.1, 0.1, 0.1, 1.0);
const vec4 colorTo = vec4(1.0, 1.0, 1.0, 1.0);

int getSignalIndex(float u)
{
	int width = endSignalIndex - startSignalIndex;
	float signalWidth = 1.0 / width;
	int signalIndex = int(floor(u / signalWidth));
	return signalIndex;
}

int getFrequencyIndex(float v)
{
	int height = endFrequencyElementIndex - startFrequencyElementIndex;
	float frequencyHeight = 1.0 / height;
	int frequencyIndex = int(floor(v / frequencyHeight));
	return frequencyIndex;
}

void main()
{
	int currentSignalRowIndex = getSignalIndex(texCoordV.x);
	int currentFrequencyIndex = getFrequencyIndex(texCoordV.y);
	int valueIndex = currentFrequencyIndex * signalsCountInRow + currentSignalRowIndex;
	float ratio = ScalogramBuffer.data[valueIndex];
	vec4 color = mix(colorFrom, colorTo, ratio);
	out_color = color;
}