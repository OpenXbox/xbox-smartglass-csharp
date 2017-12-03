using System.Collections.Generic;
using System.Linq;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.StartChannelRequest)]
    internal class StartChannelRequestMessage : SessionMessageBase
    {
        private static readonly Dictionary<ServiceType, byte[]> _serviceUuids =
            new Dictionary<ServiceType, byte[]>()
            {
                {
                    ServiceType.SystemBroadcast,
                    new byte[] {
                        0xd8, 0x17, 0xa1, 0xb6, 0xe2, 0xf5, 0xd7, 0x45, 0x86, 0x2e, 0x8f, 0xd8, 0xe3, 0x15, 0x64, 0x76 }
                },
                {
                    ServiceType.SystemInputTVRemote,
                    new byte[] {
                        0xb3, 0xe3, 0x51, 0xd4, 0xbb, 0x60, 0x71, 0x4c, 0xb3, 0xdb, 0xf9, 0x94, 0xb1, 0xac, 0xa3, 0xa7 }
                },
                {
                    ServiceType.SystemInput,
                    new byte[] {
                        0xca, 0xb8, 0x20, 0xfa, 0xfb, 0x66, 0xe0, 0x46, 0xad, 0xb6, 0x0b, 0x97, 0x8a, 0x59, 0xd3, 0x5f }
                },
                {
                    ServiceType.SystemMedia,
                    new byte[] {
                        0x24, 0xca, 0xa9, 0x48, 0x6d, 0xeb, 0x12, 0x4e, 0x8c, 0x43, 0xd5, 0x74, 0x69, 0xed, 0xd3, 0xcd }
                },
                {
                    ServiceType.SystemText,
                    new byte[] {
                        0xa2, 0xe6, 0xf3, 0x7a, 0x8b, 0x48, 0xcb, 0x40, 0xa9, 0x31, 0x79, 0xc0, 0x4b, 0x7d, 0xa3, 0xa0 }
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

        public override void Deserialize(BEReader reader)
        {
            ChannelRequestId = reader.ReadUInt32();
            TitleId = reader.ReadUInt32();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(ChannelRequestId);
            writer.Write(TitleId);

            if (ServiceType != ServiceType.None)
            {
                // TODO: Change arrays above to be in the correct order already.
                var uuidBytes = ServiceUuids[ServiceType];

                writer.Write(uuidBytes.Take(4).Reverse().ToArray());
                writer.Write(uuidBytes.Skip(4).Take(2).Reverse().ToArray());
                writer.Write(uuidBytes.Skip(6).Take(2).Reverse().ToArray());
                writer.Write(uuidBytes.Skip(8).ToArray());
            }
            else
            {
                writer.Write(new byte[16]);
            }

            writer.Write(ActivityId);
        }
    }
}