using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DarkId.SmartGlass.Messaging.Session.Messages;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Messaging.Session;
using DarkId.SmartGlass.Channels.Broadcast;
using DarkId.SmartGlass.Channels.Broadcast.Messages;
using Newtonsoft.Json.Serialization;

namespace DarkId.SmartGlass.Channels.Broadcast
{
    public class GamestreamException : SmartGlassException
    {
        public GamestreamException(string message, GamestreamError result)
            : base(message, (int)result)
        {
        }

        public GamestreamError Error => (GamestreamError)HResult;
    }
}