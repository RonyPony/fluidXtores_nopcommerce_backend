namespace Nop.Plugin.Misc.FluidApi.Factories
{
    public interface Factory<T>
    {
        Task<T> Initialize();
    }
}