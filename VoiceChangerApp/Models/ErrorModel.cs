using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Subjects;

namespace VoiceChangerApp.Models
{
    public class ErrorModel : IErrorModel
    {
        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        #region Events

        public readonly Subject<Exception> OnError = new();
        public readonly Subject<string> OnErrorDescription = new();

        #endregion

        public void RaiseError(Exception e)
        {
            _logger.LogError(e.ToString());
            OnError.OnNext(e);
            OnErrorDescription.OnNext(e.ToString());
        }

        public void RaiseError(string e)
        {
            _logger.LogError(e);
            OnErrorDescription.OnNext(e);
        }
    }
}
