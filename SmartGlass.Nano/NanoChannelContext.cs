using System;
using System.Linq;
using System.Collections.Generic;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano
{
    public class NanoChannelContext
    {
        private readonly Dictionary<ushort, NanoChannel> _channels;
        public ushort RemoteConnectionId { get; internal set; }
        public NanoChannelContext()
        {
            _channels = new Dictionary<ushort, NanoChannel>()
            {
                {0, NanoChannel.TcpBase}
            };
        }

        public NanoChannel GetChannel(ushort channelNumber)
        {
            NanoChannel id;
            if (!_channels.TryGetValue(channelNumber, out id))
            {
                return NanoChannel.Unknown;
            }
            return id;
        }

        public ushort GetChannelId(NanoChannel channel)
        {
            if (!_channels.ContainsValue(channel))
            {
                throw new Exception($"NanoChannel {channel} is not registered");
            }
            return _channels.First(x => x.Value == channel).Key;
        }

        public void RegisterChannel(ChannelCreate createPacket)
        {
            ushort channelId = createPacket.Header.ChannelId;
            NanoChannel channel = NanoChannelClass.GetIdByClassName(createPacket.Name);
            RegisterChannel(channelId, channel);
        }

        public void RegisterChannel(ushort channelId, NanoChannel channel)
        {
            _channels[channelId] = channel;
        }

        public bool UnregisterChannel(ChannelClose closePacket)
        {
            return UnregisterChannel(closePacket.Header.ChannelId);
        }

        public bool UnregisterChannel(ushort channelId)
        {
            return _channels.Remove(channelId);
        }
    }
}