using System;

namespace VoiceChangerApp.Models
{
    public interface IErrorModel
    {
        void RaiseError(Exception e);
        void RaiseError(string e);
    }
}
