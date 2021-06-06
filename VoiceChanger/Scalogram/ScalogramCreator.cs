using Microsoft.Extensions.Logging;

namespace VoiceChanger.Scalogram
{
    public abstract class ScalogramCreator
    {
        private readonly ILogger _logger;

        public bool IsFinished { get; protected set; }

        public float ProgressRatio { get; protected set; }

    }
}
