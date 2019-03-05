using System.IO;
using System.Linq;
using SmartGlass.Connection;
using SmartGlass.Common;

namespace SmartGlass.Messaging
{
    /// <summary>
    /// Protected message base.
    /// </summary>
    internal abstract class ProtectedMessageBase<THeader> : MessageBase<THeader>, ICryptoMessage
        where THeader : IProtectedMessageHeader, new()
    {
        public CryptoContext Crypto { get; set; }

        public byte[] InitVector { get; set; }

        protected abstract void SerializeProtectedPayload(BEWriter writer);

        protected abstract void DeserializeProtectedPayload(BEReader reader);

        public override void Serialize(BEWriter writer)
        {
            var protectedPayloadWriter = new BEWriter();

            SerializeProtectedPayload(protectedPayloadWriter);

            var protectedPayload = protectedPayloadWriter.ToBytes();
            Header.ProtectedPayloadLength = (ushort)protectedPayload.Length;

            var encryptedPayload = protectedPayload.Length > 0 ?
                Crypto.Encrypt(protectedPayload, InitVector) : new byte[] { };

            base.Serialize(writer);
            writer.Write(encryptedPayload);

            var signature = Crypto.CalculateMessageSignature(writer.ToBytes());
            writer.Write(signature);
        }

        public override void Deserialize(BEReader reader)
        {
            var message = reader.ReadToEnd();

            var messageBody = message.Take(message.Length - 32).ToArray();
            var signature = message.Skip(message.Length - 32).ToArray();

            if (!signature.SequenceEqual(Crypto.CalculateMessageSignature(messageBody)))
            {
                throw new InvalidDataException("Invalid message signature.");
            }

            var messageReader = new BEReader(messageBody);

            base.Deserialize(messageReader);

            var protectedPayload = messageReader.ReadToEnd();
            var unencryptedPayload = protectedPayload.Length > 0 ?
                Crypto.Decrypt(protectedPayload, InitVector) : new byte[] { };

            var protectedPayloadReader = new BEReader(unencryptedPayload);

            DeserializeProtectedPayload(protectedPayloadReader);
        }
    }
}
