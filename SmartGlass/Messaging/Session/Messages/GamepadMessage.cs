using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Gamepad)]
    internal class GamepadMessage : SessionMessageBase
    {
        public GamepadState State { get; set; }

        public override void Deserialize(BEReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(State.Timestamp);

            writer.Write((ushort)State.Buttons);

            writer.Write(State.LeftTrigger);
            writer.Write(State.RightTrigger);

            writer.Write(State.LeftThumbstickX);
            writer.Write(State.LeftThumbstickY);

            writer.Write(State.RightThumbstickX);
            writer.Write(State.RightThumbstickY);
        }
    }
}