using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.WebAPI.Models
{
    public class PlayListResultModel {
        public bool Success { get; set; }

        public bool HasChange { get; set; }

        public object Data { get; set; }

        public string  DriverId { get; set; }

        public DateTime? LastRequesTime { get; set; }
    }
}
