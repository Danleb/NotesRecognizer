__kernel void ComputeSpectrum(__global float* input,
    __global float* output)
{
    int i = get_global_id(0);
    output[i] = 64;
}