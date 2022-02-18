using System;

namespace LsAdmin.MVC.Models {
    public class ActionResult {

        public string Code { get; set; }
        public string Exception { get; set; }
        public string Msg { get; set; }

        public bool IsSuccess() {
            return Code == "1";
        }

    }

}