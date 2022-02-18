using LsAdmin.Application.ProgramReviewApp.Dtos;
using LsAdmin.Application.Imp;
using System.Collections.Generic;
using System;
using LsAdmin.Application.ProgramApp.Dtos;

namespace LsAdmin.Application.ProgramReviewApp {
    public interface IProgramReviewAppService : IBaseAppService<ProgramReviewDto> {
        ProgramReviewDto Get(Guid reviewerId, Guid programId);

        List<ProgramDto> LoadProgramPageListByUserId(Guid userId, int startPage, int pageSize, out int rowCount);
    }
}
