using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.MVC.Models
{
    public class AreaModel
    {

        public AreaModel(string code, string name)
        {
            this.Code = code;
            this.Name = name;
        }

        public String Code { get; set; }
        public String Name { get; set; }
    }
}
