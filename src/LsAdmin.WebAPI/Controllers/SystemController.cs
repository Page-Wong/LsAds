using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using NLog;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.WebAPI.Controllers {
    public class SystemController : Controller {
        static Logger Logger = LogManager.GetCurrentClassLogger();        

        public IActionResult Error() {
            var error = HttpContext.Features.Get<IExceptionHandlerFeature>();
            Logger.Error($"<h1>Error: {error.Error.Message}</h1>{error.Error.StackTrace }");
            return View();
        }

    }
}
