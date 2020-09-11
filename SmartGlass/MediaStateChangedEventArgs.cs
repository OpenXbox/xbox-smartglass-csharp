using System;

namespace SmartGlass
{
    public class MediaStateChangedEventArgs : EventArgs
    {
        public MediaState State { get; private set;  }

        public MediaStateChangedEventArgs(MediaState state)
        {
            State = state;
        }
    }
}
