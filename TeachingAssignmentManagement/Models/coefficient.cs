//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TeachingAssignmentManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class coefficient
    {
        public int id { get; set; }
        public decimal vietnamese_coefficient { get; set; }
        public decimal foreign_coefficient { get; set; }
        public decimal theoretical_coefficient { get; set; }
        public decimal practice_coefficient { get; set; }
        public int start_year { get; set; }
        public int end_year { get; set; }
        public string subject_id { get; set; }
    
        public virtual subject subject { get; set; }
    }
}
