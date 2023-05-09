using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PasswordRepository.Models
{
    public class RegistrationModel
    {
        public string textbox_USERNAME { get; set; }
        public string textbox_PASSWORD { get; set; }
        public string textbox_REPEAT_PASSWORD { get; set; }
        public string textbox_FIRST_NAME { get; set; }
        public string textbox_LAST_NAME { get; set; }
        public string textbox_EMAIL { get; set; }
        public bool checkbox_acceptTerms { get; set; }

    }
}