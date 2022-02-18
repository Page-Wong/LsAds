using System;

namespace LsAdmin.Domain.Entities {
    public class ProgramMaterial {
        public Guid ProgramId { get; set; }
        public Program Program { get; set; }

        public Guid MaterialId { get; set; }
        public Material Material { get; set; }

    }
}
