using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Subjects;

namespace VoiceChangerApp.Models
{
    public class ErrorModel
    {
        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        #region Events

        public readonly Subject<Exception> OnError = new();

        #endregion

        public void RaiseError(Exception e)
        {
            _logger.LogError(e.ToString());
            OnError.OnNext(e);
        }
    }
}
