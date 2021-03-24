using System;
using VoiceChanger.FormatParser;

namespace VoiceChanger.SpectrumCreator
{
    public abstract class SpectrumCreator : IDisposable
    {
        protected bool _disposed = false;

        public SpectrumCreator(AudioContainer audioContainer)
        {
            AudioContainer = audioContainer;
        }

        ~SpectrumCreator()
        {
            Dispose(false);
        }

        public AudioContainer AudioContainer { get; }

        public SpectrumContainer SpectrumContainer { get; protected set; }

        public abstract unsafe SpectrumContainer CreateSpectrum(SpectrumCreatorSettings settings);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            _disposed = true;
        }
    }
}
