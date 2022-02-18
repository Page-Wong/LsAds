using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Domain.Entities
{
    public class OrderTrade {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public Guid TradeId { get; set; }
        public Trade Trade { get; set; }

    }
}
