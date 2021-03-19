using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Categories;
using Nop.Plugin.Widgets.InstantCheckout.Helpers;

namespace Nop.Plugin.Widgets.InstantCheckout.Validators
{
    public class CategoryDtoValidator : BaseDtoValidator<CategoryDto>
    {

        #region Constructors

        public CategoryDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetNameRule();
        }

        #endregion

        #region Private Methods

        private void SetNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(c => c.Name, "invalid name", "name");
        }

        #endregion

    }
}