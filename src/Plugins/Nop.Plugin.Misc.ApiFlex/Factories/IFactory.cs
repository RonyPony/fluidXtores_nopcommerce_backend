using System.Threading.Tasks;
namespace Nop.Plugin.Misc.ApiFlex.Factories
{
    public interface Factory<T>
    {
        Task<T> Initialize();
    }
}