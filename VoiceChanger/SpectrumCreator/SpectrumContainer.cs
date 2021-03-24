using System;
using System.Collections;
using System.Collections.Generic;

namespace VoiceChanger.SpectrumCreator
{
    public class SpectrumContainer : IEnumerable<SpectrumSlice>
    {
        public SpectrumContainer(List<SpectrumSlice> spectrumSlices)
        {
            SpectrumSlices = spectrumSlices;
        }

        private List<SpectrumSlice> SpectrumSlices { get; set; }

        public SpectrumSlice GetSliceForSignal(int signalIndex)
        {
            return SpectrumSlices[signalIndex];
        }

        IEnumerator<SpectrumSlice> IEnumerable<SpectrumSlice>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return null;
        }
    }
}
