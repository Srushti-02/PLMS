//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PLMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Officer
    {
        public int officerID { get; set; }
        public int userID { get; set; }
        public int applicationID { get; set; }
    
        public virtual LoanApplication LoanApplication { get; set; }
        public virtual USER USER { get; set; }
    }
}
