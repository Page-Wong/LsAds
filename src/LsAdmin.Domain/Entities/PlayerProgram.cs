using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LsAdmin.Domain.Entities {
    public enum PlayerProgramStatus {
        Unpublished,
        Ready,
        Playing,
        Complete,
        Cancel,
        Pause,
    }
    public class PlayerProgram : BaseEntity
    {
        public Guid ProgramId { get; set; }
        public Program Program { get; set; }

        public Guid PlayerId { get; set; }
        public Player Player { get; set; }

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

        public virtual ICollection<OrderPlayerProgram> OrderPlayerPrograms { get; set; }
    }
}
