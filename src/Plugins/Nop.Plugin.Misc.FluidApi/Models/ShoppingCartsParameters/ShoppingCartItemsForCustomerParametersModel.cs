using Nop.Plugin.Misc.FluidApi.ModelBinders;

namespace Nop.Plugin.Misc.FluidApi.Models.ShoppingCartsParameters
{
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(typeof(ParametersModelBinder<ShoppingCartItemsForCustomerParametersModel>))]
    public class ShoppingCartItemsForCustomerParametersModel : BaseShoppingCartItemsParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}