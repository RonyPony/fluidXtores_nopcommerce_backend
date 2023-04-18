using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Plugin.Misc.ApiFlex.DTO.Categories;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class NewsLetterSubscriptoonDtoMappings
    {
        public static NewsLetterSubscriptionDto ToDto(this NewsLetterSubscription newsLetterSubscription)
        {
            return newsLetterSubscription.MapTo<NewsLetterSubscription, NewsLetterSubscriptionDto>();
        }

        public static NewsLetterSubscription ToEntity(this NewsLetterSubscriptionDto newsLetterSubscriptionDto)
        {
            return newsLetterSubscriptionDto.MapTo<NewsLetterSubscriptionDto, NewsLetterSubscription>();
        }
    }
}
