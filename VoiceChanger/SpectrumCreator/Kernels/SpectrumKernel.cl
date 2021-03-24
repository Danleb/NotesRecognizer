__kernel void ComputeSpectrum(
	__global float* input,
	__global float* output)
{
	int i = get_global_id(0);

	for (int i = 0; i < 10; ++i)
	{

	}


	output[i] = 64;
}