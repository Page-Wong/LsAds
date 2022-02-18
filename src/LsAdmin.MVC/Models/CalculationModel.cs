using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.MVC.Models
{
    public class CalculationModel
    {
        public Guid Id { get; set; }
        public String TimeRangeType{get;set;}
        public float Price { get; set; }
        public int Count { get; set; }
        public int Time { get; set; }
    }
}
