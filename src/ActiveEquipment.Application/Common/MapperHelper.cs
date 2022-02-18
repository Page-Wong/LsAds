using ActiveEquipment.Application.DataModel;
using AutoMapper;


namespace ActiveEquipment.Application.Common {
    public class MapperHelper {
        public static IMapper Mapper = null;
        public static void Initialize() {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<OriginalInstructionNotify, OriginalInstructionNotifyDto>();
                cfg.CreateMap<OriginalInstructionNotifyDto, OriginalInstructionNotify>();
                cfg.CreateMap<InstructionResultDto, InstructionResult>();
                cfg.CreateMap<InstructionResult, InstructionResultDto>();

            });
            Mapper = config.CreateMapper();
        }

    }
}