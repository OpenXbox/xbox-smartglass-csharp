using System;

namespace SmartGlass.Channels
{
    /// <summary>
    /// Auxiliary stream data event arguments.
    /// </summary>
    public class AuxiliaryStreamDataReceivedEventArgs : EventArgs
    {
        private readonly byte[] _data;
        /// <summary>
        /// Gets the decrypted data chunk.
        /// </summary>
        /// <value>Decrypted data chunk.</value>
        public byte[] Data => _data;

        public AuxiliaryStreamDataReceivedEventArgs(byte[] data)
        {
            _data = data;
        }
    }
}
