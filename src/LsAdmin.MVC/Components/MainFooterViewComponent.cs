using LsAdmin.Application.MenuApp;
using LsAdmin.Application.UserApp;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.MVC.Components
{
    [ViewComponent(Name = "MainFooter")]
    public class MainFooterViewComponent : ViewComponent
    {
        public MainFooterViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
