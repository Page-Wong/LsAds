using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.MVC.Models
{
    public class PlayListResultModel {
        public bool Success { get; set; }

        public bool HasChange { get; set; }

        public object Data { get; set; }

        public String DriverId { get; set; }
    }
}
