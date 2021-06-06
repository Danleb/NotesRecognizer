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
		HRESULT result = CoInitializeEx(NULL, COINIT_MULTITHREADED);
		CheckSuccess(result, "Failed to initialize COM.");

		result = CoCreateInstance(CLSID_MMDeviceEnumerator, NULL, CLSCTX_ALL, IID_IMMDeviceEnumerator, reinterpret_cast<void**>(&m_mmDeviceEnumerator));
		CheckSuccess(result, "Failed to create MMDeviceEnumerator.");

		result = m_mmDeviceEnumerator->GetDefaultAudioEndpoint(eRender, eMultimedia, &m_mmDevice);
		CheckSuccess(result, "Failed to get default audio endpoint.");

		result = m_mmDevice->Activate(IID_IAudioClient, CLSCTX_ALL, NULL, reinterpret_cast<void**>(&m_audioClient));
		CheckSuccess(result, "Failed to activate audio client.");

		WAVEFORMATEX* deviceFormat;
		result = m_audioClient->GetMixFormat(&deviceFormat);
		CheckSuccess(result, "Failed  to get mix format.");

		if (deviceFormat->wFormatTag != WAVE_FORMAT_PCM)
		{
			throw std::runtime_error("Device doesn't support PCM format.");
		}

		result = m_audioClient->Initialize(AUDCLNT_SHAREMODE_SHARED,
			0,
			10000000,
			0,
			deviceFormat,
			NULL);
		CheckSuccess(result, "Failed to initialize audioclient.");

		UINT32 bufferFrameCount;
		result = m_audioClient->GetBufferSize(&bufferFrameCount);
		CheckSuccess(result, "Failed to get buffer size");

		result = m_audioClient->GetService(IID_IAudioRenderClient, reinterpret_cast<void**>(m_audioRenderClient));
		CheckSuccess(result, "Failed to get audio render client.");

		result = m_audioRenderClient->GetBuffer(bufferFrameCount, 0);
		CheckSuccess(result, "Failed to get audio render client buffer.");



	}

	void AudioPlayer::PlayFromStart()
	{
		m_currentSamplePosition = 0;
		PlayFromCurrentPosition();
	}

	void AudioPlayer::PlayFromCurrentPosition()
	{
		while (true)
		{

		}
	}

	float AudioPlayer::GetTimePosition()
	{
		return 0;
	}

	void AudioPlayer::CheckSuccess(HRESULT result, const char* errorMessage)
	{
		if (FAILED(result))
		{
			throw std::runtime_error(errorMessage);
		}
	}

	void RenderThread()
	{

	}
}