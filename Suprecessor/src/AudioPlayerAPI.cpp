#include <stdint.h>
#include <iostream>
#include <unordered_map>

#include "AudioPlayer.h"
#include "AudioPlayerAPI.h"

AudioPlayerId nextId = 0;
std::unordered_map<AudioPlayerId, suprecessor::AudioPlayer*> audioPlayers;

int DllExport TestAdd(int a, int b)
{
	return a + b;
}

AudioPlayerId GenerateUniqueId()
{
	return ++nextId;
}

int32_t CreateAudioPlayer(suprecessor::AudioContainer audioContainer)
{
	try
	{
		auto audioPlayer = new suprecessor::AudioPlayer(audioContainer);
		auto id = GenerateUniqueId();
		audioPlayers[id] = audioPlayer;
		return id;
	}
	catch (const std::exception& exception)
	{
		std::cerr << exception.what() << std::endl;
		return -1;
	}

	return -1;
}