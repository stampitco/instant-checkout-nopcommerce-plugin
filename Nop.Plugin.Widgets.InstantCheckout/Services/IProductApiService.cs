using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.InstantCheckout.Constants;

namespace Nop.Plugin.Widgets.InstantCheckout.Services
{
    public interface IProductApiService
    {
        IList<Product> GetProducts(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
           int limit = Configurations.DEFAULT_LIMIT, int page = Configurations.DEFAULT_PAGE_VALUE, int sinceId = Configurations.DEFAULT_SINCE_ID,
           bool? publishedStatus = null);

        Product GetProductById(int productId);

        Product GetProductByIdNoTracking(int productId);
    }
}