#include <stdint.h>

#define DllExport __declspec( dllexport )

using AudioPlayerId = int32_t;

extern "C"
{
	int DllExport TestAdd(int a, int b);

	AudioPlayerId DllExport CreateAudioPlayer(char* buffer, int32_t size, int32_t sample_rate, int32_t value_size);
	
	//bool DllExport IsPlaying();
	//void DllExport Play(,);
	//void DllExport Stop();
}