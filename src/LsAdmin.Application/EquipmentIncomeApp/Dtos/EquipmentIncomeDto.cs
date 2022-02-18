using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Application.EquipmentIncomeApp.Dtos
{
    public class EquipmentIncomeDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 收益ID
        /// </summary>
        public DateTime? IncomeDate { get; set; }
        /// <summary>
        /// 收益日期
        /// </summary>
        public decimal Ratio { get; set; }
        /// <summary>
        /// 分配比率
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string CurrencyUnit { get; set; }
        /// <summary>
        /// 货币单位
        /// </summary>
        public Guid IDEquipment { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
    }
}
