using Nop.Plugin.Misc.ApiFlex.ModelBinders;

namespace Nop.Plugin.Misc.ApiFlex.Models.ProductsParameters
{
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(typeof(ParametersModelBinder<ProductsCountParametersModel>))]
    public class ProductsCountParametersModel : BaseProductsParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}