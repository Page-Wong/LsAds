using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentService.WebAPI.Models
{
    public class JsonResultModel {
        public bool Success { get; set; }

        public int Total { get; set; }

        public object DataList { get; set; }
    }
}
