using System;

namespace LsAdmin.Domain.Entities {
    public class Androidapk : BaseEntity {
        public String AppName { get; set; }
        public String PackageName { get; set; }
        public String VersionName { get; set; }
        public int VersionCode { get; set; }          
        public int EquipmentType { get; set; }
    }
}
