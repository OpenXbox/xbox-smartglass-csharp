using System.IO;

namespace SmartGlass.Common
{
    /// <summary>
    /// Serializable (Little endian)
    /// </summary>
    public interface ISerializableLE
    {
        void Deserialize(BinaryReader reader);
        void Serialize(BinaryWriter writer);
    }
}
