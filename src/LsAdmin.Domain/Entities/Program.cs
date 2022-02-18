using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LsAdmin.Domain.Entities {
    public enum ProgramType {
        Web,
        Video
    }
    public class Program : BaseEntity {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ProgramType Type { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        /// <summary>
        /// 持续时间
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// 节目内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 拥有者
        /// </summary>
        public Guid? OwnerUserId { get; set; }

        /// <summary>
        /// 素材集合
        /// </summary>
        public virtual ICollection<ProgramMaterial> ProgramMaterials { get; set; }
    }
}
