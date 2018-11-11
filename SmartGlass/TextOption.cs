using System;

namespace SmartGlass
{
    [Flags]
    public enum TextOption
    {
        Default = 0x0000,
        AcceptsReturn = 0x0001,
        Password = 0x0002,
        MultiLine = 0x0004,
        SpellCheckEnabled = 0x0008,
        PredictionEnabled = 0x0010,
        RTL = 0x0020,
        Dismiss = 0x4000
    }
}
