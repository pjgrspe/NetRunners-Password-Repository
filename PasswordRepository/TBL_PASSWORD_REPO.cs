//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PasswordRepository
{
    using System;
    using System.Collections.Generic;
    
    public partial class TBL_PASSWORD_REPO
    {
        public int PID { get; set; }
        public int UID { get; set; }
        public string PR_TITLE { get; set; }
        public string PR_EMAIL { get; set; }
        public string PR_USERNAME { get; set; }
        public string PR_PASSWORD { get; set; }
        public string PR_URL { get; set; }
        public string PR_NOTES { get; set; }
        public bool isTrashed { get; set; }
        public bool isActive { get; set; }
    }
}