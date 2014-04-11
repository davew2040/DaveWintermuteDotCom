using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheDaveSite.ViewModels.Account
{
    public class ResetPasswordLink_ViewModel
    {
        public bool TokenFound { get; set; }
        public string NewPassword { get; set; }
    }
}