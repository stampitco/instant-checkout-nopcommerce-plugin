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
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);

        IList<CustomerDto> Search(string query = "", string order = Configurations.DefaultOrder,
            int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit);

        Dictionary<string, string> GetFirstAndLastNameByCustomerId(int customerId);
    }
}