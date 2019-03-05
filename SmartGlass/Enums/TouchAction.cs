using System;

namespace SmartGlass
{
    /// <summary>
    /// Touch action.
    /// Used by InputChannel.
    /// </summary>
    public enum TouchAction : ushort
    {
        Down = 1,
        Move,
        Up,
        Cancel
    }
}
