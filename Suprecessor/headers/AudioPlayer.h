#include <stdint.h>
#include <thread>

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
		std::thread m_audioThread;

		void PlayFromCurrentPosition();
	};
}