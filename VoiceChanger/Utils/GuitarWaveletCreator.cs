using System;
using System.Numerics;

namespace VoiceChanger.Utils
{
    public static class GuitarWaveletCreator
    {
        public static Complex[] CreateWavelet(float sinusoidFrequency, int sampleRate, int pointsCount, GuitarWaveletSettings settings)
        {
            var samplePeriod = 1.0 / sampleRate;
            var values = new Complex[pointsCount];

            var sigma = 2 * Math.PI * sinusoidFrequency;

            for (int i = 0; i < pointsCount; i++)
            {
                //var time = i * samplePeriod;
                var time = (-pointsCount / 2.0 + i) * samplePeriod;

                //var value = Complex.Exp(new Complex(0, 1)) * (1.0f / (time + 0.2f));
                //var value = time;
                //var value = Complex.Exp(new Complex(0, sigma * time));

                //var value = Complex.Exp(new Complex(0, sigma * time)) * (1.0f / (time + 0.2f));
                //var value = Complex.Exp(new Complex(0, sigma * time)) * (1.0f / (2 * time + 0.1f));
                //var value = Complex.Exp(new Complex(0, sigma * time)) * (1.0f / (settings.Coefficient * time + settings.Bias));

                //
                //var value = Math.Abs(Math.Atan(time * settings.Coefficient)) * Complex.Exp(new Complex(0, sigma * time)) * 100;

                var value =
                    //Math.Exp(-1.0 / 2.0 * Math.Pow(time / settings.Duration, 2)) *
                    (Complex.Exp(new Complex(0, sigma * time)))
                    ;

                if (time < -settings.Duration)
                {
                    value = 0;
                }

                if (time > settings.Duration)
                {
                    value = 0;
                }

                //if (time < settings.Duration)
                //{
                //    value = 0;
                //}

                //if(time > 0)
                //{
                //    value = 0;
                //}

                values[i] = value;
            }

            return values;
            //return values.Reverse().ToArray();
        }
    }
}
