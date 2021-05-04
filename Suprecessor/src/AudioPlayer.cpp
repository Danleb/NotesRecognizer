#include <Audioclient.h>
#include <audioendpoints.h>
#include <audioenginebaseapo.h>
#include <AudioEngineEndpoint.h>
#include <audiopolicy.h>
#include <AudioSessionTypes.h>
#include <devicetopology.h>
#include <endpointvolume.h>
#include <mmdeviceapi.h>
#include <exception>
#include <winerror.h>

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

	void AudioPlayer::Initialize()
	{
		HRESULT result = CoCreateInstance(
			CLSID_MMDeviceEnumerator, NULL,
			CLSCTX_ALL, IID_IMMDeviceEnumerator,
			(void**)&m_mmDeviceEnumerator);
		CheckSuccess(result, "Failed to create MMDeviceEnumerator");
		
		m_mmDeviceEnumerator->EnumAudioEndpoints(eRender, fab)

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

	void CheckSuccess(HRESULT result, const char* errorMessage)
	{
		if (FAILED(result))
		{
			throw std::runtime_error(errorMessage);
		}
	}
}