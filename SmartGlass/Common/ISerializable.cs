using System.IO;

namespace SmartGlass.Common
{
    /// <summary>
    /// Serializable interface
    /// </summary>
    public interface ISerializable
    {
        void Deserialize(EndianReader reader);
        void Serialize(EndianWriter writer);
    }
}
