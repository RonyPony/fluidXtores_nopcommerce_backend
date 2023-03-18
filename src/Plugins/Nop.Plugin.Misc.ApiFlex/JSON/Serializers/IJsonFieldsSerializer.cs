namespace Nop.Plugin.Misc.ApiFlex.JSON.Serializers
{
    using Nop.Plugin.Misc.ApiFlex.DTO;

    public interface IJsonFieldsSerializer
    {
        string Serialize(ISerializableObject objectToSerialize, string fields);
    }
}
