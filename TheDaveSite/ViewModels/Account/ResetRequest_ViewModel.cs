using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheDaveSite.ViewModels.Account
{
    public class ResetRequest_ViewModel
    {
        public string Email { get; set; }
        public bool AccountFound { get; set; }
    }
}