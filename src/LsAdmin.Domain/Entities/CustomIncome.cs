using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LsAdmin.Domain.Entities
{
    public class CustomIncome : BaseEntity
    {
        [Required]
        [StringLength(36)]
        [Display(Name = "订单ID")]
        public string IDOrders { get; set; }

        [Required]
        [Display(Name = "收益日期")]
        public DateTime? IncomeDate { get; set; }

        [Required]
        [DefaultValue(0)]
        [Display(Name = "分配比率")]
        public decimal Ratio { get; set; }

        [Required]
        [DefaultValue(0)]
        [Display(Name = "金额")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(36)]
        [Display(Name = "货币单位")]
        public string CurrencyUnit { get; set; }


    }
}
