using System;
using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.InstantCheckout.Constants;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Customers;

namespace Nop.Plugin.Widgets.InstantCheckout.Services
{
    public interface ICustomerApiService
    {
        int GetCustomersCount();

        CustomerDto GetCustomerById(int id, bool showDeleted = false);

        Customer GetCustomerEntityById(int id);

        IList<CustomerDto> GetCustomersDtos(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DEFAULT_LIMIT, int page = Configurations.DEFAULT_PAGE_VALUE, int sinceId = Configurations.DEFAULT_SINCE_ID);

        IList<CustomerDto> Search(string query = "", string order = Configurations.DEFAULT_ORDER,
            int page = Configurations.DEFAULT_PAGE_VALUE, int limit = Configurations.DEFAULT_LIMIT);

        Dictionary<string, string> GetFirstAndLastNameByCustomerId(int customerId);
    }
}