using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Misc.FluidApi.ModelBinders;

namespace Nop.Plugin.Misc.FluidApi.Models.SpecificationAttributes
{
    // JsonProperty is used only for swagger
    [ModelBinder(typeof(ParametersModelBinder<SpecifcationAttributesCountParametersModel>))]
    public class SpecifcationAttributesCountParametersModel
    {
        public SpecifcationAttributesCountParametersModel()
        {

        }
    }
}