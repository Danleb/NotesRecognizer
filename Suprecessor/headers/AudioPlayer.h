#include <Audioclient.h>
#include <mmdeviceapi.h>
#include <stdint.h>
#include <thread>
#include <Windows.h>

namespace suprecessor
{
	struct AudioContainer
	{
		char* data;
		int32_t dataSize;
		int32_t sampleRate;
		int32_t channelCount;
		int32_t sampleSize;
	};

	class AudioPlayer
	{
	public:
		AudioPlayer(AudioContainer audioContainer);
		virtual ~AudioPlayer();

		void Initialize();
		void PlayFromStart();
		void Play(float timeStart);
		bool IsPlaying();
		void Stop();
		void Continue();
		float GetTimePosition();

	protected:


	private:
		AudioContainer m_audioContainer;
		int32_t m_currentSamplePosition = 0;
		std::thread m_renderThread;
		IMMDeviceEnumerator* m_mmDeviceEnumerator = NULL;
		IMMDevice* m_mmDevice = NULL;
		IAudioClient* m_audioClient = NULL;
		IAudioRenderClient* m_audioRenderClient = NULL;

		void PlayFromCurrentPosition();
		void CheckSuccess(HRESULT result, const char* errorMessage);
	};
}