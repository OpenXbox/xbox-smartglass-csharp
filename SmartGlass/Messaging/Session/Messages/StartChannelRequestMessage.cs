using System;
using System.Collections.Generic;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.StartChannelRequest)]
    internal record StartChannelRequestMessage : SessionMessageBase
    {
        private static readonly Dictionary<ServiceType, byte[]> _serviceUuids =
            new Dictionary<ServiceType, byte[]>()
            {
                {
                    ServiceType.SystemBroadcast,
                    new byte[] { 0xb6, 0xa1, 0x17, 0xd8, 0xf5, 0xe2, 0x45, 0xd7, 0x86, 0x2e, 0x8f, 0xd8, 0xe3, 0x15, 0x64, 0x76 }
                },
                {
                    ServiceType.SystemInputTVRemote,
                    new byte[] { 0xd4, 0x51, 0xe3, 0xb3, 0x60, 0xbb, 0x4c, 0x71, 0xb3, 0xdb, 0xf9, 0x94, 0xb1, 0xac, 0xa3, 0xa7 }
                },
                {
                    ServiceType.SystemInput,
                    new byte[] { 0xfa, 0x20, 0xb8, 0xca, 0x66, 0xfb, 0x46, 0xe0, 0xad, 0xb6, 0x0b, 0x97, 0x8a, 0x59, 0xd3, 0x5f }
                },
                {
                    ServiceType.SystemMedia,
                    new byte[] { 0x48, 0xa9, 0xca, 0x24, 0xeb, 0x6d, 0x4e, 0x12, 0x8c, 0x43, 0xd5, 0x74, 0x69, 0xed, 0xd3, 0xcd }
                },
                {
                    ServiceType.SystemText,
                    new byte[] { 0x7a, 0xf3, 0xe6, 0xa2, 0x48, 0x8b, 0x40, 0xcb, 0xa9, 0x31, 0x79, 0xc0, 0x4b, 0x7d, 0xa3, 0xa0 }
                },
                {
                    ServiceType.None,
                    new byte[16]
                }
            };

        public StartChannelRequestMessage()
        {
            Header.RequestAcknowledge = true;
        }

        public static IReadOnlyDictionary<ServiceType, byte[]> ServiceUuids => _serviceUuids;

        public uint ChannelRequestId { get; set; }
        public uint TitleId { get; set; }
        public ServiceType ServiceType { get; set; }
        public uint ActivityId { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(ChannelRequestId);
            writer.WriteBE(TitleId);

            var uuidBytes = ServiceUuids[ServiceType];
            writer.Write(uuidBytes);

            writer.WriteBE(ActivityId);
        }
    }
}
