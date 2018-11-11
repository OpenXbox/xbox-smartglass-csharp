using System.IO;

namespace SmartGlass.Common
{
    interface ISerializableLE
    {
         void Deserialize(LEReader reader);
         void Serialize(LEWriter writer);
    }
}