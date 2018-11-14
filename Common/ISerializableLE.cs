using System.IO;

namespace SmartGlass.Common
{
    interface ISerializableLE
    {
        void Deserialize(BinaryReader reader);
        void Serialize(BinaryWriter writer);
    }
}