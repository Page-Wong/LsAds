using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LsAdmin.Domain.Entities {
    public enum PlayerType {
        Private,
        Public,
        None//可私有可共有
    }
    public class Player : BaseEntity
    {
        public Guid EquipmentId { get; set; }
        public virtual Equipment Equipment { get; set; }       

        public float Width { get; set; }
        public float Height { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Sort { get; set; }

        /// <summary>
        /// 拥有者
        /// </summary>
        public Guid? OwnerUserId { get; set; }


        public PlayerType Type { get; set; }
        public virtual ICollection<PlayerProgram> PlayerPrograms { get; set; }
    }
}
