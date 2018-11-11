namespace DarkId.SmartGlass
{
    public struct GamepadState
    {
        public GamepadButtons Buttons;

        public float LeftTrigger;
        public float RightTrigger;
        public float LeftThumbstickX;
        public float LeftThumbstickY;
        public float RightThumbstickX;
        public float RightThumbstickY;

        public long Timestamp;
    }
}