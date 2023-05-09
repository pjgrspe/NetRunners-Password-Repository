using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PasswordRepository.Models
{
    public class RegistrationModel
    {
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string REPEAT_PASSWORD { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string EMAIL { get; set; }
        public bool acceptTerms { get; set; }

    }
}