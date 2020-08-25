using System.Linq;
using SmartGlass.Connection;
using SmartGlass.Common;
using System;
using System.Text.Json.Serialization;

namespace SmartGlass.Messaging.Session
{
    /// <summary>
    /// Session fragment message.
    /// </summary>
    [MessageType(MessageType.SessionMessage)]
    internal class SessionFragmentMessage : MessageBase<SessionFragmentMessageHeader>, ICryptoMessage
    {
        private static readonly int payloadSizeAlignment = 16;

        [JsonIgnore]
        public CryptoContext Crypto { get; set; }

        public bool InvalidSignature { get; set; }

        public byte[] Fragment { get; set; }

        protected override void DeserializePayload(EndianReader reader)
        {
            Fragment = reader.ReadToEnd();
        }

        protected override void SerializePayload(EndianWriter writer)
        {
            writer.Write(Fragment);
        }

        public override void Deserialize(EndianReader reader)
        {
            var message = reader.ReadToEnd();

            var messageBody = message.Take(message.Length - 32).ToArray();
            var signature = message.Skip(message.Length - 32).ToArray();

            if (!signature.SequenceEqual(Crypto.CalculateMessageSignature(messageBody)))
            {
                InvalidSignature = true;
                return;
            }

            var initVectorSource = message.Take(16).ToArray();
            var initVector = Crypto.CreateDerivedInitVector(initVectorSource);

            var messageBodyReader = new EndianReader(messageBody);

            Header.Deserialize(messageBodyReader);

            var payload = messageBodyReader.ReadToEnd();
            var payloadReader = new EndianReader(payload);
            DeserializePayload(payloadReader);

            var decrypted = Crypto.DecryptWithoutPadding(Fragment, initVector);

            Fragment = decrypted.Take(Header.PayloadLength).ToArray();
        }

        public override void Serialize(EndianWriter writer)
        {
            var messageWriter = new EndianWriter();

            base.Serialize(messageWriter);

            var message = messageWriter.ToBytes();

            var initVectorSource = message.Take(16).ToArray();
            var initVector = Crypto.CreateDerivedInitVector(initVectorSource);

            var fragmentWriter = new EndianWriter();

            byte[] padding = Padding.CreatePaddingData(
                PaddingType.PKCS7,
                Fragment,
                alignment: payloadSizeAlignment
            );

            fragmentWriter.Write(Fragment);
            fragmentWriter.Write(padding);

            var encryptedFragment = Crypto.EncryptWithoutPadding(fragmentWriter.ToBytes(), initVector);

            Header.Serialize(writer);
            writer.Write(encryptedFragment);

            var signature = Crypto.CalculateMessageSignature(writer.ToBytes());
            writer.Write(signature);
        }
    }
}
