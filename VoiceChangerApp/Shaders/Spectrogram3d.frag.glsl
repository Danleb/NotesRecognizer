#version 430

in vec2 texCoordV;
out vec4 out_color;

const vec4 background = vec4(0.1, 0.1, 0.1, 1.0);
const vec4 colorFrom = vec4(0.1, 0.1, 0.1, 1.0);
const vec4 colorTo = vec4(1.0, 1.0, 1.0, 1.0);

uniform int startFrequencyElementIndex;
uniform int endFrequencyElementIndex;
uniform float startTime;
uniform float endTime;
uniform vec2 mouseUV;

void main()
{




	out_color = background;
}