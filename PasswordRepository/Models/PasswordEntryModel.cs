using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PasswordRepository.Models
{
    public class PasswordEntryModel
    {
        public List<TBL_PASSWORD_REPO> Passwords { get; set; }
        public TBL_PASSWORD_REPO Password { get; set; }
        public List<TBL_LOGIN> UserCredList { get; set; }
        public TBL_LOGIN UserCred { get; set; }
        public List<TBL_USER_DETAILS> UserDetailsList { get; set; }
        public TBL_USER_DETAILS UserDetails { get; set; }
        public int PasswordID_Variable { get; set; }
        public string textbox_PR_TITLE { get; set; }
        public string textbox_PR_EMAIL { get; set; }
        public string textbox_PR_USERNAME { get; set; }
        public string textbox_PR_PASSWORD { get; set; }
        public string textbox_PR_URL { get; set; }
        public string textbox_PR_NOTES { get; set; }
    }
}