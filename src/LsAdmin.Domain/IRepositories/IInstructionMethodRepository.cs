using LsAdmin.Domain.Entities;
using System;

namespace LsAdmin.Domain.IRepositories {
    public interface IInstructionMethodRepository : IRepository<InstructionMethod>
    {
        InstructionMethod GetByName(string name);
    }
}
