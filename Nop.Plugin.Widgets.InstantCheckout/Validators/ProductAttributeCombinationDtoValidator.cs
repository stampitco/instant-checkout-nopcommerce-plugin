using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Products;
using Nop.Plugin.Widgets.InstantCheckout.Helpers;

namespace Nop.Plugin.Widgets.InstantCheckout.Validators
{
    public class ProductAttributeCombinationDtoValidator : BaseDtoValidator<ProductAttributeCombinationDto>
    {

        #region Constructors

        public ProductAttributeCombinationDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetAttributesXmlRule();
            SetProductIdRule();
        }

        #endregion

        #region Private Methods

        private void SetAttributesXmlRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(p => p.AttributesXml, "invalid attributes xml", "attributes_xml");
        }

        private void SetProductIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(p => p.ProductId, "invalid product id", "product_id");
        }

        #endregion

    }
}