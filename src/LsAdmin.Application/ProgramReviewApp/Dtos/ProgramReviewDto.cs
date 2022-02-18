using LsAdmin.Application.ProgramApp.Dtos;
using LsAdmin.Application.UserApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.ProgramReviewApp.Dtos {
    public class ProgramReviewDto {

        public Guid Id { get; set; }

        /// <summary>
        /// 节目
        /// </summary>
        public Guid ProgramId { get; set; }
        public virtual ProgramDto Program { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public Guid ReviewerId { get; set; }
        public virtual UserDto Reviewer { get; set; }

        /// <summary>
        /// 对应审核流向的审核任务ID
        /// </summary>
        public Guid? TaskId { get; set; }

        /// <summary>
        /// 审核结果
        /// </summary>
        public ProgramReviewResult Result { get; set; }

        public string ResultName {
            get {
                switch (Result) {
                    case ProgramReviewResult.Enable:
                        return "启用";
                    case ProgramReviewResult.Disable:
                        return "禁用";
                    default:
                        return "无";
                }
            }
        }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string Content { get; set; }


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
       
    }
}
