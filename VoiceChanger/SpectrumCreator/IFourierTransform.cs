using System.Numerics;

namespace VoiceChanger.SpectrumCreator
{
    public interface IFourierTransform
    {
        Complex[] CreateTransformZeroPadded(bool forward = true);
    }
}
