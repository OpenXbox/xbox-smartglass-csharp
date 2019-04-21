using System;
using SmartGlass.Common;
using SmartGlass.Connection;
using SmartGlass.Messaging.Session;
using SmartGlass.Messaging.Session.Messages;

namespace SmartGlass.Analysis
{
    /// <summary>
    /// MessageAnalzer provides an easy way to parse SmartGlass
    /// messages in form of raw bytes. It needs to be initialized
    /// with a crypto blob (shared secret).
    /// </summary>
    public class MessageAnalyzer
    {
        private readonly CryptoContext _crypto;

        /// <summary>
        /// Initialize an instance of MessageAnalzer with a
        /// shared secret blob.
        /// <seealso cref="SmartGlass.Connection.CryptoContext.CryptoBlob"/>
        /// </summary>
        /// <param name="cryptoBlob">Shared secret blob</param>
        public MessageAnalyzer(byte[] cryptoBlob)
        {
            _crypto = new CryptoContext(cryptoBlob);
        }


        /// <summary>
        /// Read a message in form of raw bytes and return its decrypted,
        /// parsed and human-readable information.
        /// </summary>
        /// <param name="encryptedMessage">
        /// Encrypted SmartGlass messages as bytearray
        /// </param>
        /// <returns>Parsed MessageInfo</returns>
        public MessageInfo ReadMessage(byte[] encryptedMessage)
        {
            var message = new SessionFragmentMessage();
            message.Crypto = _crypto;

            message.Deserialize(new EndianReader(encryptedMessage));

            if (message.InvalidSignature)
            {
                throw new Exception("Invalid message signature.");
            }

            var info = new MessageInfo()
            {
                RequestAcknowledge = message.Header.RequestAcknowledge,
                Version = message.Header.Version,
                ChannelId = message.Header.ChannelId,
                MessageType = message.Header.SessionMessageType.ToString(),
                Data = message.Fragment
            };

            if (message.Header.SessionMessageType == SessionMessageType.Json)
            {
                var jsonMessage = new JsonMessage();
                jsonMessage.Deserialize(new EndianReader(message.Fragment));

                info.Json = jsonMessage.Json;
            }

            return info;
        }
    }
}
