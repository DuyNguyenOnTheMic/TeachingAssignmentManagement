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
    
    public partial class lecturer_rank
    {
        public int id { get; set; }
        public string academic_degree_rank_id { get; set; }
        public string lecturer_id { get; set; }
        public int term_id { get; set; }
    
        public virtual academic_degree_rank academic_degree_rank { get; set; }
        public virtual lecturer lecturer { get; set; }
        public virtual term term { get; set; }
    }
}
