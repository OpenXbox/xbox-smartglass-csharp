namespace SmartGlass.Common
{
    /// <summary>
    /// Serializable (Big endian)
    /// </summary>
    public interface ISerializable
    {
        void Deserialize(BEReader reader);
        void Serialize(BEWriter writer);
    }
}
