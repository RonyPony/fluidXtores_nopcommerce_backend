
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.FluidApi.DTO.Images;
using Nop.Plugin.Misc.FluidApi.DTO.Products;

namespace Nop.Plugin.Misc.FluidApi.DTO.ProductImages
{
    public class ProductPicturesRootObjectDto : ISerializableObject
    {
        public ProductPicturesRootObjectDto()
        {
            //Image = new ProductPicture();
        }

        [JsonProperty("image")]
        public ImageMappingDto Image { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "image";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (ImageMappingDto);
        }
    }
}