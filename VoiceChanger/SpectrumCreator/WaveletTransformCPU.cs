using System;
using System.Numerics;
using VoiceChanger.FormatParser;

namespace VoiceChanger.SpectrumCreator
{
    public class WaveletTransformCPU
    {
        public AudioContainer AudioContainer { get; }

        public WaveletTransformCPU(AudioContainer audioContainer)
        {
            AudioContainer = audioContainer;
        }

        public static float[] CreateScalogram(float frequency, Complex[] signalFT, int sampleRate, int cyclesCount, double sigma)
        {
            var pointsCount = signalFT.Length;
            var wavelet = GenerateMorletWavelet(frequency, sampleRate, cyclesCount, pointsCount, sigma);
            var kernelFFT = new FastFourierTransformCPU(wavelet).CreateTransformZeroPadded();

            var pointwiseMultiplication = new Complex[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                pointwiseMultiplication[i] = signalFT[i] * kernelFFT[i];
            }

            var inverseFFT = new FastFourierTransformCPU(pointwiseMultiplication).CreateTransform(false);

            var amplitudes = new float[inverseFFT.Length];
            for (int i = 0; i < amplitudes.Length; i++)
            {
                //amplitudes[i] = (float)inverseFFT[i].Imaginary;
                var value = inverseFFT[i];
                //amplitudes[i] = (float)Math.Sqrt(value.Real * value.Real + value.Imaginary * value.Imaginary);
                //amplitudes[i] = (float)value.Real;
                amplitudes[i] = (float)value.Magnitude;
                //amplitudes[i] = (float)value.Imaginary;
            }

            return amplitudes;
        }

        public float[] CreateScalogram(float sinusoidFrequency)
        {
            var signalFFT = new FastFourierTransformCPU(AudioContainer.Samples).CreateTransformZeroPadded();
            var waveletCyclesCount = 5;
            var sigma = 6;
            return CreateScalogram(sinusoidFrequency, signalFFT, AudioContainer.SampleRate, waveletCyclesCount, sigma);
        }

        public static Complex[] GenerateMorletWavelet(float sinusoidFrequency, int sampleRate, int cycles, int pointsCount, double sigma)
        {
            //var gaussianWindowScale = 1.0f;
            //gaussianWindowScale *= sinusoidFrequency;
            //const int cyclesCountInIdentityGaussian = 3;
            //gaussianWindowScale *= (cycles / cyclesCountInIdentityGaussian);

            var samplePeriod = 1.0 / sampleRate;
            //var seconds = samplePeriod * pointsCount;

            var values = new Complex[pointsCount];

            sigma = 2 * Math.PI * sinusoidFrequency;

            for (int i = 0; i < pointsCount; i++)
            {
                //var time = -seconds / 2.0 + samplePeriod * i;
                var time = (-pointsCount / 2.0 + i) * samplePeriod;

                var kSigma = Math.Exp(-1.0 / 2.0 * Math.Pow(sigma, 2));

                var cSigma = Math.Pow(
                    (1 + Math.Exp(-Math.Pow(sigma, 2) - 2 * Math.Exp(-3.0 / 4.0 * Math.Pow(sigma, 2))))
                    , -0.5);

                var value =
                    cSigma *
                    Math.Pow(Math.PI, -1.0 / 4.0) *
                    Math.Exp(-1.0 / 2.0 * Math.Pow(time, 2)) *
                    (Complex.Exp(new Complex(0, sigma * time)) - kSigma);

                values[i] = value;
            }

            return values;
        }
    }
}
