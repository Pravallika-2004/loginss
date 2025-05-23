//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication4.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ClubRegistration
    {
        public int RegistrationID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> ClubID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string PreferredRole { get; set; }
        public string RoleJustification { get; set; }
        public string ProfileImagePath { get; set; }
        public Nullable<int> ApprovalStatusID { get; set; }
        public string AssignedRole { get; set; }
        public Nullable<System.DateTime> RegisteredAt { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public Nullable<int> UniversityID { get; set; }
    
        public virtual ApprovalStatusTable ApprovalStatusTable { get; set; }
        public virtual CLUB CLUB { get; set; }
        public virtual USER USER { get; set; }
        public virtual DEPARTMENT DEPARTMENT { get; set; }
        public virtual UNIVERSITY UNIVERSITY { get; set; }
    }
}
