using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Application.PlaceTypeApp.Dtos
{
    public class PlaceTypeDto
    {
        public Guid Id { get; set; }
        public string Remarks { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? CreateTime { get; set; }

        //场所类型
        public string Type { get; set; }
    }
}
