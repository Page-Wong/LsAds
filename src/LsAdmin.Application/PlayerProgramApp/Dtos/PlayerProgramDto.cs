using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Application.PlayerApp.Dtos;
using LsAdmin.Application.ProgramApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LsAdmin.Application.PlayerProgramApp.Dtos {
    public class PlayerProgramDto {

        public Guid Id { get; set; }
        public Guid ProgramId { get; set; }
        public ProgramDto Program { get; set; }

        public Guid PlayerId { get; set; }
        public PlayerDto Player { get; set; }

        /// <summary>
        /// 播放时间段设置
        /// </summary>
        public string DateTimeSetting { get; set; }

        /// <summary>
        /// 曝光次数
        /// </summary>
        public uint ExposureCount { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public PlayerProgramStatus Status { get; set; }

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

        /// <summary>
        /// 排序号
        /// </summary>
        public int Sort { get; set; }

        public virtual List<OrderPlayerProgramDto> OrderPlayerPrograms { get; set; }

        public virtual List<OrderDto> Orders { get; set; }

        /// <summary>
        /// 订单状态中文
        /// </summary>
        public virtual String StatusString
        {
            get
            {
                switch (Status) {
                    case PlayerProgramStatus.Unpublished:
                        return "未发布";

                    case PlayerProgramStatus.Ready:
                        return "准备播放";

                    case PlayerProgramStatus.Playing:
                        return "播放中";

                    case PlayerProgramStatus.Complete:
                        return "完成";

                    case PlayerProgramStatus.Cancel:
                        return "取消";

                    case PlayerProgramStatus.Pause:
                        return "暂定";
                    default:
                        return "未知状态";
                }
            }
        }
    }
}
