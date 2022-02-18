using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentService.WebAPI.Models
{
    public class AppInfoModel {
        [Required]
        public String Id { get; set; }
        [Required]
        public String AppName { get; set; }
        public String PackageName { get; set; }
        public String VersionName { get; set; }
        public int VersionCode { get; set; }

    }
}
