using Nop.Plugin.Misc.FluidApi.ModelBinders;
namespace Nop.Plugin.Misc.FluidApi.Models.CategoriesParameters
{
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(typeof(ParametersModelBinder<CategoriesCountParametersModel>))]
    public class CategoriesCountParametersModel : BaseCategoriesParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}