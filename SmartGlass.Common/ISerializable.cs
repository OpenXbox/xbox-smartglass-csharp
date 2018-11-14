namespace SmartGlass.Common
{
    public interface ISerializable
    {
        void Deserialize(BEReader reader);
        void Serialize(BEWriter writer);
    }
}