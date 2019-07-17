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

        protected abstract void SerializeProtectedPayload(EndianWriter writer);

        protected abstract void DeserializeProtectedPayload(EndianReader reader);

        public override void Serialize(EndianWriter writer)
        {
            var protectedPayloadWriter = new EndianWriter();

            SerializeProtectedPayload(protectedPayloadWriter);

            // legth is pefore padding
            Header.ProtectedPayloadLength = (ushort)protectedPayloadWriter.Length;

            // padding is before encryption
            byte[] padding = Padding.CreatePaddingData(
               PaddingType.PKCS7,
               protectedPayloadWriter.Length,
               alignment: payloadSizeAlignment
            );

            protectedPayloadWriter.Write(padding);

            // encrypt without adding padding to the encrypted value
            var encryptedPayload = Crypto.EncryptWithoutPadding(protectedPayloadWriter.ToBytes(), InitVector);

            base.Serialize(writer);
            writer.Write(encryptedPayload);

            var signature = Crypto.CalculateMessageSignature(writer.ToBytes());
            writer.Write(signature);
        }

        public override void Deserialize(EndianReader reader)
        {
            var message = reader.ReadToEnd();

            var messageBody = message.Take(message.Length - 32).ToArray();
            var signature = message.Skip(message.Length - 32).ToArray();

            if (!signature.SequenceEqual(Crypto.CalculateMessageSignature(messageBody)))
            {
                throw new InvalidDataException("Invalid message signature.");
            }

            var messageReader = new EndianReader(messageBody);

            base.Deserialize(messageReader);

            var protectedPayload = messageReader.ReadToEnd();
            var unencryptedPayload = protectedPayload.Length > 0 ?
                Crypto.Decrypt(protectedPayload, InitVector) : new byte[] { };

            var protectedPayloadReader = new EndianReader(unencryptedPayload);

            DeserializeProtectedPayload(protectedPayloadReader);
        }
    }
}
