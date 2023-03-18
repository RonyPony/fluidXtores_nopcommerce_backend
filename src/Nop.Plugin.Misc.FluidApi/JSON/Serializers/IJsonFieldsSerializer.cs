namespace Nop.Plugin.Misc.FluidApi.JSON.Serializers
{
    using Nop.Plugin.Misc.FluidApi.DTO;

    public interface IJsonFieldsSerializer
    {
        string Serialize(ISerializableObject objectToSerialize, string fields);
    }
}
