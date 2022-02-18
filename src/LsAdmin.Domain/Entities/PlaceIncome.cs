using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LsAdmin.Domain.Entities
{
    public class PlaceIncome : CustomIncome
    {
        [ForeignKey("IDPlace")]
        public virtual Place Place { get; set; }
        
    }
}
