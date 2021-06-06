#include <iostream>
#include <stdint.h>
#define _USE_MATH_DEFINES
#include <math.h>

#include "AudioPlayer.h"
#include "AudioPlayerAPI.h"

int main()
{
	int sampleRate = 44100;
	int sampleSize = sizeof(int16_t);
	int channelCount = 1;
	float duration = 2;
	float frequency = 440;
	int valuesCount = static_cast<int>(channelCount * sampleRate * duration);
	int dataSize = valuesCount * sampleSize;
	const int16_t digitMultiplier = 1;

	char* data = new char[dataSize];
	int16_t* values = reinterpret_cast<int16_t*>(data);
	for (size_t i = 0; i < valuesCount; ++i)
	{
		float time = static_cast<float>(i) / sampleRate;
		float value = std::sin(time * frequency * 2 * M_PI);
		int16_t digitValue = static_cast<int16_t>(value) * digitMultiplier;
		values[i] = digitValue;
	}

	suprecessor::AudioContainer audioContainer;
	audioContainer.data = data;
	audioContainer.dataSize = dataSize;
	audioContainer.sampleRate = sampleRate;
	audioContainer.channelCount = channelCount;
	audioContainer.sampleSize = sampleSize;

	suprecessor::AudioPlayer player(audioContainer);
	player.Initialize();
	player.PlayFromStart();

	delete[] data;
}