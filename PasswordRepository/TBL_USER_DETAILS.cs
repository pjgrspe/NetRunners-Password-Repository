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
    
    public partial class TBL_USER_DETAILS
    {
        public int ID { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public int UID { get; set; }
        public System.DateTime DATE_CREATED { get; set; }
        public Nullable<System.DateTime> DATE_MODIFIED { get; set; }
        public string PIN { get; set; }
        public int TIMEOUT { get; set; }
    
        public virtual TBL_LOGIN TBL_LOGIN { get; set; }
    }
}
