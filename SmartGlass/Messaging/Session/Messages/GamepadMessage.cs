using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Gamepad)]
    internal record GamepadMessage : SessionMessageBase
    {
        public GamepadState State { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(State.Timestamp);

            writer.WriteBE((ushort)State.Buttons);

            writer.WriteBE(State.LeftTrigger);
            writer.WriteBE(State.RightTrigger);

            writer.WriteBE(State.LeftThumbstickX);
            writer.WriteBE(State.LeftThumbstickY);

            writer.WriteBE(State.RightThumbstickX);
            writer.WriteBE(State.RightThumbstickY);
        }
    }
}