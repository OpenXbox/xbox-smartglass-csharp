using System;

namespace DarkId.SmartGlass.Channels
{
    public class AuxiliaryStreamDataReceivedEventArgs : EventArgs
    {
        private readonly byte[] _data;
        public byte[] Data => _data;

        public AuxiliaryStreamDataReceivedEventArgs(byte[] data)
        {
            _data = data;
        }
    }
}