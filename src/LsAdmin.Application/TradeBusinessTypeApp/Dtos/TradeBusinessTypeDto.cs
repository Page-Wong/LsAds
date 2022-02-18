using System;

namespace LsAdmin.Application.TradeBusinessTypeApp.Dtos {
    public class TradeBusinessTypeDto {
        public Guid Id { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }


        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }


        //类型名称
        public string Name { get; set; }
        //类型显示名称
        public string DisplayName { get; set; }
    }
}
