﻿using ECaterer.WebApi.Common.Interfaces;
using System.Linq;

namespace ECaterer.WebApi.Common.Filters.Orders
{
    public class OrderPriceLowerThanFilter : IFilter<Core.Models.Order>
    {
        public int? price { get; private set; }

        public OrderPriceLowerThanFilter(int? price)
        {
            this.price = price;
        }

        public IQueryable<Core.Models.Order> Filter(IQueryable<Core.Models.Order> data)
        {
            if (price is null)
                return data;
            return data.Where(o => o.Price <= price);
        }
    }
}
