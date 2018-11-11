using System;

namespace DarkId.SmartGlass
{
    public class ConsoleStatusChangedEventArgs : EventArgs
    {
        private readonly ConsoleStatus _status;
        public ConsoleStatus Status => _status;

        public ConsoleStatusChangedEventArgs(ConsoleStatus status)
        {
            _status = status;
        }
    }
}