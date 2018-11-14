using System.IO;

namespace SmartGlass.Common
{
    public interface ISerializableLE
    {
        void Deserialize(BinaryReader reader);
        void Serialize(BinaryWriter writer);
    }
}