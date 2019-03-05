using System;
using System.Linq;
using System.Collections.Generic;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano
{
    /// <summary>
    /// Nano channel context.
    /// </summary>
    public class NanoChannelContext
    {
        private readonly Dictionary<ushort, NanoChannel> _channels;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Nano.NanoChannelContext"/> class.
        /// </summary>
        public NanoChannelContext()
        {
            _channels = new Dictionary<ushort, NanoChannel>()
            {
                {0, NanoChannel.TcpBase}
            };
        }

        /// <summary>
        /// Gets the channel identifier.
        /// </summary>
        /// <returns>The channel identifier.</returns>
        /// <param name="channelNumber">Channel number.</param>
        public NanoChannel GetChannel(ushort channelNumber)
        {
            NanoChannel id;
            if (!_channels.TryGetValue(channelNumber, out id))
            {
                return NanoChannel.Unknown;
            }
            return id;
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <returns>The channel.</returns>
        /// <param name="channel">Channel identifier.</param>
        public ushort GetChannelId(NanoChannel channel)
        {
            if (!_channels.ContainsValue(channel))
            {
                throw new Exception($"NanoChannel {channel} is not registered");
            }
            return _channels.First(x => x.Value == channel).Key;
        }

        /// <summary>
        /// Registers the channel via ChannelCreate packet.
        /// </summary>
        /// <param name="createPacket">ChannelCreate packet.</param>
        public void RegisterChannel(ChannelCreate createPacket)
        {
            ushort channelId = createPacket.Header.ChannelId;
            NanoChannel channel = NanoChannelClass.GetIdByClassName(createPacket.Name);
            RegisterChannel(channelId, channel);
        }

        /// <summary>
        /// Registers the channel via channel number and channel Id.
        /// </summary>
        /// <param name="channelNumber">Channel number.</param>
        /// <param name="channelId">Channel identifier.</param>
        public void RegisterChannel(ushort channelNumber, NanoChannel channelId)
        {
            _channels[channelNumber] = channelId;
        }

        /// <summary>
        /// Unregisters the channel via ChannelClose packet.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if channel was unregistered successfully, <c>false</c> otherwise.
        /// </returns>
        /// <param name="closePacket">ChannelClose packet.</param>
        public bool UnregisterChannel(ChannelClose closePacket)
        {
            return UnregisterChannel(closePacket.Header.ChannelId);
        }

        /// <summary>
        /// Unregisters the channel via channel number
        /// </summary>
        /// <returns>
        /// <c>true</c>, if channel was unregistered successfully, <c>false</c> otherwise.
        /// </returns>
        /// <param name="channelNumber">Channel number.</param>
        public bool UnregisterChannel(ushort channelNumber)
        {
            return _channels.Remove(channelNumber);
        }
    }
}
