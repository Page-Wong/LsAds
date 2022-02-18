using LsAdmin.Application.MaterialApp.Dtos;
using LsAdmin.Application.MenuApp.Dtos;
using System;

namespace LsAdmin.Application.ProgramApp.Dtos {
    public class ProgramMaterialDto {

        public Guid ProgramId { get; set; }
        public ProgramDto Program { get; set; }

        public Guid MaterialId { get; set; }
        public MaterialDto Material { get; set; }
    }
}