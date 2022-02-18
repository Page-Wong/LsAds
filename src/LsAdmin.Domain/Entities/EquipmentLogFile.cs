using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Domain.Entities
{
    public class EquipmentLogFile {
        public Guid EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        public Guid LogFileId { get; set; }
        public Files LogFile { get; set; }

    }
}
