__kernel void ComputeSpectrum(__global const float* input0,
    __global float* out)
{
    const size_t idx = get_global_id(0);
    out[idx] = 50;
}