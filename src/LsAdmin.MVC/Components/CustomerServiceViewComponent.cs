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
    [ViewComponent(Name = "CustomerService")]
    public class CustomerServiceViewComponent : ViewComponent
    {
        public CustomerServiceViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
