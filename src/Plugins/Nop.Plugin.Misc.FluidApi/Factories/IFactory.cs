namespace Nop.Plugin.Misc.FluidApi.Factories
{
    public interface IFactory<T>
    {
        Task<T> Initialize();
    }
}