using System.IO;

namespace DarkId.SmartGlass.Common
{
    interface ISerializableLE
    {
         void Deserialize(LEReader reader);
         void Serialize(LEWriter writer);
    }
}