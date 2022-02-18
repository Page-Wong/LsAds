using LsAdmin.Application.ProgramApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pomelo.AspNetCore.TimedJob;

namespace LsAdmin.MVC.Job
{
    /// <summary>
    /// 打包节目文件
    /// </summary>
    public class AutoPackProgramFiles : Pomelo.AspNetCore.TimedJob.Job
    {
        private readonly IProgramAppService _programService;
        public AutoPackProgramFiles(IProgramAppService programService) : base()
        {
            _programService = programService;
        }

        // Begin 起始时间；Interval执行时间间隔，单位是毫秒，建议使用以下格式，此处为10分钟；
        //SkipWhileExecuting是否等待上一个执行完成，true为等待；
        [Invoke(Interval = 1000 * 60*10, SkipWhileExecuting = true)]
        public void Run()
        {
            _programService.PackageMaterialsZipFromNoPackage();
        }
    }

}
