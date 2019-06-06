using System;

namespace SafenetSign
{
    public sealed class Logger
    {
        private readonly bool _isVerbose;

        public Logger(bool isVerbose)
        {
            _isVerbose = isVerbose;
        }

        public void WriteLine(string message, bool isVerbose)
        {
            if (isVerbose && !_isVerbose)
            {
                return;
            }

            Console.WriteLine(message);
        }
    }
}