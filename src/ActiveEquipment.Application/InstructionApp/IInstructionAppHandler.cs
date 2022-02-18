using ActiveEquipment.Application.DataModel;
using LsAdmin.Application.InstructionApp.Dto;
using System.Threading.Tasks;

namespace ActiveEquipment.Application.InstructionApp {
    public interface IInstructionAppHandler {

        Task<Result> SendInstructionAsync(InstructionDto dto, bool isEnforce = false);
        Result ReceiveInstructionResult(InstructionResultDto dto);
        Result ReceiveOriginalInstructionNotify(OriginalInstructionNotifyDto dto);
    }

}