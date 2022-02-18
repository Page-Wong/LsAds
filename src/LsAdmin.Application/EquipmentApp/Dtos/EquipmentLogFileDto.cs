using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.FilesApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Domain.Entities
{
    public class EquipmentLogFileDto  {
        public Guid EquipmentId { get; set; }
        public EquipmentDto Equipment { get; set; }

        public Guid LogFileId { get; set; }
        public FilesDto LogFile { get; set; }

    }
}
