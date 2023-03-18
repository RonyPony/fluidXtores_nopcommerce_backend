using System.Collections.Generic;
using Newtonsoft.Json;
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;
using Nop.Plugin.Misc.FluidApi.ModelBinders;

namespace Nop.Plugin.Misc.FluidApi.Models.ProductsParameters
{
    using Microsoft.AspNetCore.Mvc;

    // JsonProperty is used only for swagger
    [ModelBinder(typeof(ParametersModelBinder<TaxCategoriesParametersModel>))]
    public class TaxCategoriesParametersModel : BaseProductsParametersModel
    {       
    }
}