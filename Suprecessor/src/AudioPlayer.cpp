#include <Audioclient.h>
#include <audioendpoints.h>
#include <audioenginebaseapo.h>
#include <AudioEngineEndpoint.h>
#include <audiopolicy.h>
#include <AudioSessionTypes.h>
#include <devicetopology.h>
#include <endpointvolume.h>
#include <mmdeviceapi.h>

#include "AudioPlayer.h"

const CLSID CLSID_MMDeviceEnumerator = __uuidof(MMDeviceEnumerator);
const IID IID_IMMDeviceEnumerator = __uuidof(IMMDeviceEnumerator);
const IID IID_IAudioClient = __uuidof(IAudioClient);
const IID IID_IAudioRenderClient = __uuidof(IAudioRenderClient);

namespace suprecessor
{
	AudioPlayer::AudioPlayer(AudioContainer audioContainer) : m_audioContainer{ audioContainer }
	{

	}

	AudioPlayer::~AudioPlayer()
	{

	}

	void AudioPlayer::PlayFromStart()
	{
		m_currentSamplePosition = 0;
		PlayFromCurrentPosition();
	}

	void AudioPlayer::PlayFromCurrentPosition()
	{

	}

	float AudioPlayer::GetTimePosition()
	{
		return 0;
	}
}