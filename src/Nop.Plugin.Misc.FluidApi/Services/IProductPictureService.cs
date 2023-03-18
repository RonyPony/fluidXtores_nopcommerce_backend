using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Misc.FluidApi.Services
{
    public interface IProductPictureService
    {
        ProductPicture GetProductPictureByPictureId(int pictureId);
    }
}
