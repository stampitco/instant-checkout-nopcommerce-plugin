using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Products;
using Nop.Plugin.Widgets.InstantCheckout.Helpers;

namespace Nop.Plugin.Widgets.InstantCheckout.Validators
{
    public class ProductDtoValidator : BaseDtoValidator<ProductDto>
    {

        #region Constructors

        public ProductDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetNameRule();
        }

        #endregion

        #region Private Methods

        private void SetNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(p => p.Name, "invalid name", "name");
        }

        #endregion

    }
}