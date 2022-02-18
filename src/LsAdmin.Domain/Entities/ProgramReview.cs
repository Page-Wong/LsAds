using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LsAdmin.Domain.Entities {
    public enum ProgramReviewResult {
        Enable,
        Disable
    }
    public class ProgramReview : BaseEntity {
        /// <summary>
        /// 节目
        /// </summary>
        public Guid ProgramId { get; set; }
        public virtual Program Program { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public Guid ReviewerId { get; set; }
        public virtual User Reviewer { get; set; }

        /// <summary>
        /// 对应审核流向的审核任务ID
        /// </summary>
        public Guid? TaskId { get; set; }

        /// <summary>
        /// 审核结果
        /// </summary>
        public ProgramReviewResult Result { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string Content { get; set; }


    }
}
