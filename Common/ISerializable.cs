namespace SmartGlass.Common
{
    interface ISerializable
    {
         void Deserialize(BEReader reader);
         void Serialize(BEWriter writer);
    }
}