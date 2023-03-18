using Nop.Plugin.Misc.FluidApi.ModelBinders;
namespace Nop.Plugin.Misc.FluidApi.Models.ManufacturersParameters
{
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(typeof(ParametersModelBinder<ManufacturersCountParametersModel>))]
    public class ManufacturersCountParametersModel : BaseManufacturersParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}