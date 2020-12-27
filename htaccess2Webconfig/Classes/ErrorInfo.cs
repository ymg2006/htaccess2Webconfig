using System;

namespace WebConfig
{
    internal sealed class ErrorInfo
    {
        public ErrorInfo(string message) : this(message, false)
        {
        }

        public ErrorInfo(string message, bool isWarning)
        {
            _message = message;
            _isWarning = isWarning;
        }

        private readonly string _message;
        public bool IsWarning
        {
            get
            {
                return _isWarning;
            }
        }

        private readonly bool _isWarning;
        public string Message
        {
            get
            {
                return _message;
            }
        }

    }
}
