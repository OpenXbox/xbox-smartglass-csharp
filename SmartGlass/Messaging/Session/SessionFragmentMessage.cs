using System.Linq;
using SmartGlass.Connection;
using SmartGlass.Common;
using Newtonsoft.Json;

namespace SmartGlass.Messaging.Session
{
    [MessageType(MessageType.SessionMessage)]
    internal class SessionFragmentMessage : MessageBase<SessionFragmentMessageHeader>, ICryptoMessage
    {
        private static readonly int payloadSizeAlignment = 16;

        [JsonIgnore]
        public CryptoContext Crypto { get; set; }

        public bool InvalidSignature { get; set; }

        public byte[] Fragment { get; set; }

        protected override void DeserializePayload(BEReader reader)
        {
            Fragment = reader.ReadToEnd();
        }

        protected override void SerializePayload(BEWriter writer)
        {
            writer.Write(Fragment);
        }

        public override void Deserialize(BEReader reader)
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

            var messageBodyReader = new BEReader(messageBody);

            Header.Deserialize(messageBodyReader);

            var payload = messageBodyReader.ReadToEnd();
            var payloadReader = new BEReader(payload);
            DeserializePayload(payloadReader);

            var decrypted = Crypto.DecryptWithoutPadding(Fragment, initVector);

            Fragment = decrypted.Take(Header.PayloadLength).ToArray();
        }

        public override void Serialize(BEWriter writer)
        {
            var messageWriter = new BEWriter();

            base.Serialize(messageWriter);

            var message = messageWriter.ToArray();

            var initVectorSource = message.Take(16).ToArray();
            var initVector = Crypto.CreateDerivedInitVector(initVectorSource);

            var fragmentWriter = new BEWriter();
            fragmentWriter.WriteWithPaddingAlignment(
                Fragment,
                payloadSizeAlignment);

            var encryptedFragment = Crypto.EncryptWithoutPadding(fragmentWriter.ToArray(), initVector);

            Header.Serialize(writer);
            writer.Write(encryptedFragment);

            var signature = Crypto.CalculateMessageSignature(writer.ToArray());
            writer.Write(signature);
        }
    }
}