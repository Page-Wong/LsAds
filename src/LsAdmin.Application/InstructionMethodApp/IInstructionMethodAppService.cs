using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;

public interface IInstructionMethodAppService : IBaseAppService<InstructionMethodDto> {    
    InstructionMethodDto GetByName(string name);
}

