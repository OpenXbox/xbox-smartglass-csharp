using System;
using SmartGlass.Common;
using SmartGlass.Connection;
using SmartGlass.Messaging.Session;
using SmartGlass.Messaging.Session.Messages;

namespace SmartGlass.Analysis
{
    public class MessageAnalyzer
    {
        private readonly CryptoContext _crypto;

        public MessageAnalyzer(byte[] cryptoBlob)
        {
            _crypto = new CryptoContext(cryptoBlob);
        }


        public MessageInfo ReadMessage(byte[] encryptedMessage)
        {
            var message = new SessionFragmentMessage();
            message.Crypto = _crypto;

            message.Deserialize(new BEReader(encryptedMessage));

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
                jsonMessage.Deserialize(new BEReader(message.Fragment));

                info.Json = jsonMessage.Json;
            }

            return info;
        }
    }
}