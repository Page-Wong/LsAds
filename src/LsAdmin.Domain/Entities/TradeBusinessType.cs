using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Domain.Entities
{
    public class TradeBusinessType : BaseEntity {
        //类型名称
        public string Name { get; set; }
        //类型显示名称
        public string DisplayName { get; set; }
    }
}
