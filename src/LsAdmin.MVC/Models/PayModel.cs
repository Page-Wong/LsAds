using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.MVC.Models
{
    public class PayModel {
        public string OrderNo { get; set; }
        public string Subject { get; set; }
        public float TotalAmout { get; set; }
        public string ItemBody { get; set; }
        public string OrderId { get; set; }
        public ushort TradeMethod { get; set; }

    }
}
