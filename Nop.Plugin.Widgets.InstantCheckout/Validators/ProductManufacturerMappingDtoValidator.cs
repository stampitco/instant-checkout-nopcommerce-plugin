
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.ProductManufacturerMappings;
using Nop.Plugin.Widgets.InstantCheckout.Helpers;

namespace Nop.Plugin.Widgets.InstantCheckout.Validators
{
    public class ProductManufacturerMappingDtoValidator : BaseDtoValidator<ProductManufacturerMappingsDto>
    {

        #region Constructors

        public ProductManufacturerMappingDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetManufacturerIdRule();
            SetProductIdRule();
        }

        #endregion

        #region Private Methods

        private void SetManufacturerIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(p => p.ManufacturerId, "invalid manufacturer_id", "manufacturer_id");
        }

        private void SetProductIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(p => p.ProductId, "invalid product_id", "product_id");
        }

        #endregion

    }
}