using System;
using System.Collections.Generic;
using SmartGlass.Common;
using SmartGlass.Connection;

namespace SmartGlass.Messaging.Connection
{
    /// <summary>
    /// Connect request message.
    /// Sent from client to console. Console will respond with ConnectResponse.
    /// </summary>
    [MessageType(MessageType.ConnectRequest)]
    internal class ConnectRequestMessage : ProtectedMessageBase<ConnectionMessageHeader>
    {
        public Guid DeviceId { get; set; }
        public PublicKeyType PublicKeyType => Crypto.PublicKeyType;
        public byte[] PublicKey => Crypto.PublicKey;
        public string UserHash { get; set; }
        public string Authorization { get; set; }
        public uint SequenceNumber { get; set; }
        public uint SequenceBegin { get; set; }
        public uint SequenceEnd { get; set; }

        protected override void DeserializePayload(EndianReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void DeserializeProtectedPayload(EndianReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void SerializePayload(EndianWriter writer)
        {
            writer.Write(DeviceId.ToByteArray());

            writer.WriteBE((ushort)PublicKeyType);
            writer.Write(PublicKey);

            writer.Write(InitVector);
        }

        protected override void SerializeProtectedPayload(EndianWriter writer)
        {
            writer.WriteUInt16BEPrefixed(UserHash);
            writer.WriteUInt16BEPrefixed(Authorization);

            writer.WriteBE(SequenceNumber);
            writer.WriteBE(SequenceBegin);
            writer.WriteBE(SequenceEnd);
        }

        /// <summary>
        /// Generates an anonymous connect request.
        /// </summary>
        /// <param name="deviceId">Client device id</param>
        /// <param name="initialSequenceNumber">Initial sequence number</param>
        /// <returns>Generated connect request</returns>
        public static ConnectRequestMessage GenerateAnonymousRequest(
            Guid deviceId, CryptoContext crypto, int initialSequenceNumber = 0)
        {
            return new ConnectRequestMessage()
            {
                InitVector = SmartGlass.Connection.CryptoContext.GenerateRandomInitVector(),
                DeviceId = deviceId,
                UserHash = "",
                Authorization = "",

                SequenceNumber = (uint)initialSequenceNumber,
                SequenceBegin = (uint)(initialSequenceNumber + 1),
                SequenceEnd = (uint)(initialSequenceNumber + 1)
            };
        }

        /// <summary>
        /// Generates the connect request.
        /// Either returns a single ConnectRequest if no authorization data is
        /// provided or more-than-one if authorization data is provided.
        /// </summary>
        /// <returns>An Enumerable of connect requests.</returns>
        /// <param name="deviceId">Client device Id.</param>
        /// <param name="xboxLiveUserHash">Xbox live user hash.</param>
        /// <param name="xboxLiveAuthorization">Xbox live authorization token (XToken).</param>
        /// <param name="initialSequenceNumber">Initial sequence number.</param>
        public static IEnumerable<ConnectRequestMessage> GenerateConnectRequest(
            Guid deviceId, CryptoContext crypto,
            string xboxLiveUserHash, string xboxLiveAuthorization,
            int initialSequenceNumber = 0)
        {
            var requests = new List<ConnectRequestMessage>();

            /* 
             * Get size of anonymous ConnectRequest to calculate
             * available space for authentication data
             */
            var anonymousRequest = GenerateAnonymousRequest(deviceId, crypto, initialSequenceNumber);
            EndianWriter bw = new EndianWriter();
            ((ICryptoMessage)anonymousRequest).Crypto = crypto;
            anonymousRequest.Serialize(bw);
            var connectRequestSize = bw.ToBytes().Length;

            if (String.IsNullOrEmpty(xboxLiveAuthorization)
                || String.IsNullOrEmpty(xboxLiveUserHash))
            {
                // Incomplete authoritzation data
                // Return anonymous connect request
                requests.Add(anonymousRequest);
                return requests;
            }

            var authenticationStringLength = xboxLiveUserHash.Length
                                             + xboxLiveAuthorization.Length;
            var maxStringSize = 1024 - connectRequestSize;
            var fragmentCount = authenticationStringLength / maxStringSize;
            if ((authenticationStringLength % maxStringSize) > 0)
                fragmentCount++;

            if (fragmentCount <= 1)
                throw new InvalidOperationException(
                    "Authentication data too small to fragment");

            var xtokenPosition = 0;
            for (int fragment = 0; fragment < fragmentCount; fragment++)
            {
                var availableBytes = maxStringSize;
                var currentUserhash = String.Empty;
                var currentXToken = String.Empty;

                if (fragment == 0)
                {
                    // Userhash fits in first fragment
                    currentUserhash = xboxLiveUserHash;
                    availableBytes -= currentUserhash.Length;
                }

                var xtokenCopyLength = availableBytes > (xboxLiveAuthorization.Length - xtokenPosition)
                                       ? xboxLiveAuthorization.Length - xtokenPosition
                                       : availableBytes;

                currentXToken = xboxLiveAuthorization.Substring(xtokenPosition, xtokenCopyLength);
                xtokenPosition += currentXToken.Length;

                var requestFragment = new ConnectRequestMessage()
                {
                    InitVector = SmartGlass.Connection.CryptoContext.GenerateRandomInitVector(),
                    DeviceId = deviceId,
                    UserHash = currentUserhash,
                    Authorization = currentXToken,

                    SequenceNumber = (uint)(initialSequenceNumber + fragment),
                    SequenceBegin = (uint)initialSequenceNumber,
                    SequenceEnd = (uint)(initialSequenceNumber + fragmentCount)
                };
                requests.Add(requestFragment);
            }

            return requests;
        }
    }
}
