namespace Nop.Plugin.Misc.FluidApi.Factories
{
    public interface IFactory<T>
    {
        object Initialize();
        Task<T> InitializeAsync();
    }
}