using LsAdmin.Application.OrderApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.WebAPI.Models
{
    public class OrderPlayPlaceModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 时间段
        /// </summary>
        public string TimeRange { get; set; }

        /// <summary>
        /// 曝光率
        /// </summary>
        public string ExposureRate { get; set; }

        /// <summary>
        /// 曝光次数
        /// </summary>
        public int ExposureCount { get; set; }

        //剩余可播放时间
        public double Availabletimes { get; set; }

        public double Totaltimes { get; set; }

        public int AvailableDays { get; set; }

        public OrderDto Order { get; set; }
        
         
    }
}
