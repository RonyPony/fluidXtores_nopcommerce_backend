using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Misc.FluidApi.DTO.Base;
using Nop.Plugin.Misc.FluidApi.DTO.Customers;
using Nop.Plugin.Misc.FluidApi.DTO.Orders;
using Nop.Plugin.Misc.FluidApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;

namespace Nop.Plugin.Misc.FluidApi.Validators
{
    public abstract class BaseDtoValidator<T> : AbstractValidator<T> where T : BaseDto, new()
    {

        #region Private Fields

        private Dictionary<string, object> _requestValuesDictionary;

        #endregion

        #region Constructors

        public BaseDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary)
        {
            HttpContextAccessor = httpContextAccessor;
            JsonHelper = jsonHelper;

            // this is hacky - can't make requestJsonDictionary an optional parameter because Nop tries to resolve it
            //
            // when DI (or the Nop Engine) resolves this class, requestJsonDictionary will be empty (length 0)
            //    in this case, HttpMethod should be whatever the current context is
            // when we manually instantiate this class (from other validators to validate child objects), requestJsonDictionary will be null for "new" objects and populated for existing objects
            //    in this scenario, we want to check if there's an id, and force "create" (POST) validation if there isn't an id

            HttpMethod = new HttpMethod(HttpContextAccessor.HttpContext.Request.Method);
            if (requestJsonDictionary == null || requestJsonDictionary.Count > 0 && !requestJsonDictionary.ContainsKey("id"))
            {
                HttpMethod = HttpMethod.Post;
            }

            if (requestJsonDictionary != null && requestJsonDictionary.Count > 0)
            {
                _requestValuesDictionary = requestJsonDictionary;
            }

            SetRequiredIdRule();
        }

        #endregion

        #region Protected Properties

        protected IHttpContextAccessor HttpContextAccessor { get; private set; }

        protected Dictionary<string, object> RequestJsonDictionary
        {
            get
            {
                if (_requestValuesDictionary == null)
                {
                    _requestValuesDictionary = GetRequestJsonDictionaryDictionaryFromHttpContext();
                }

                return _requestValuesDictionary;
            }
        }

        protected IJsonHelper JsonHelper { get; private set; }

        #endregion

        #region Public Properties

        public HttpMethod HttpMethod { get; set; }

        #endregion

        #region Protected Methods

        protected void MergeValidationResult(CustomContext validationContext, ValidationResult validationResult)
        {
            if (!validationResult.IsValid)
            {
                foreach (var validationFailure in validationResult.Errors)
                {
                    validationContext.AddFailure(validationFailure);
                }
            }
        }

        protected Dictionary<string, object> GetRequestJsonDictionaryCollectionItemDictionary<TDto>(string collectionKey, TDto dto) where TDto : BaseDto
        {
            var collectionItems = (List<object>)RequestJsonDictionary[collectionKey];
            var collectionItemDictionary = collectionItems.FirstOrDefault(x =>
                ((Dictionary<string, object>)x).ContainsKey("id") && ((int)(long)((Dictionary<string, object>)x)["id"]) == dto.Id
            ) as Dictionary<string, object>;

            return collectionItemDictionary;
        }

        protected void SetGreaterThanZeroCreateOrUpdateRule(Expression<Func<T, int?>> expression, string errorMessage, string requestValueKey)
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey(requestValueKey))
            {
                SetGreaterThanZeroRule(expression, errorMessage);
            }
        }

        protected void SetGreaterThanZeroRule(Expression<Func<T, int?>> expression, string errorMessage)
        {
            RuleFor(expression)
                .NotNull()
                .NotEmpty()
                .Must(id => id > 0);
        }

        protected void SetNotNullOrEmptyCreateOrUpdateRule(Expression<Func<T, string>> expression, string errorMessage, string requestValueKey)
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey(requestValueKey))
            {
                SetNotNullOrEmptyRule(expression, errorMessage);
            }
        }

        protected void SetNotNullOrEmptyRule(Expression<Func<T, string>> expression, string errorMessage)
        {
            RuleFor(expression)
                .NotNull()
                .NotEmpty()
                .WithMessage(errorMessage);
        }

        #endregion

        #region Private Methods

        private Dictionary<string, object> GetRequestJsonDictionaryDictionaryFromHttpContext()
        {
            var requestJsonDictionary = JsonHelper.GetRequestJsonDictionaryFromStreamAsync(HttpContextAccessor.HttpContext.Request.Body, true);
            var rootPropertyName = JsonHelper.GetRootPropertyName<T>();

            if (requestJsonDictionary.ContainsKey(rootPropertyName))
            {
                requestJsonDictionary = (Dictionary<string, object>)requestJsonDictionary[rootPropertyName];
            }

            return requestJsonDictionary;
        }

        private void SetRequiredIdRule()
        {
            if (HttpMethod == HttpMethod.Put)
            {
                SetGreaterThanZeroCreateOrUpdateRule(x => x.Id, "invalid id", "id");
            }
        }

        #endregion

    }

    public class CustomContext
    {
        internal void AddFailure(ValidationFailure validationFailure)
        {
            throw new NotImplementedException();
        }

        public static implicit operator CustomContext(ValidationContext<CustomerDto> v)
        {
            throw new NotImplementedException();
        }

        public static implicit operator CustomContext(ValidationContext<OrderDto> v)
        {
            throw new NotImplementedException();
        }
    }
}