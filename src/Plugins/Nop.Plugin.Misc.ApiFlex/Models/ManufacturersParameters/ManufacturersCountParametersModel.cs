using Nop.Plugin.Misc.ApiFlex.ModelBinders;
namespace Nop.Plugin.Misc.ApiFlex.Models.ManufacturersParameters
{
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(typeof(ParametersModelBinder<ManufacturersCountParametersModel>))]
    public class ManufacturersCountParametersModel : BaseManufacturersParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}