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
    
    public partial class curriculum_class
    {
        public string id { get; set; }
        public string original_id { get; set; }
        public string type { get; set; }
        public string student_class_id { get; set; }
        public Nullable<int> minimum_student { get; set; }
        public Nullable<int> total_lesson { get; set; }
        public string room { get; set; }
        public string day { get; set; }
        public int start_lesson { get; set; }
        public Nullable<int> lesson_number { get; set; }
        public string lesson_time { get; set; }
        public string room_2 { get; set; }
        public string room_type { get; set; }
        public Nullable<int> capacity { get; set; }
        public Nullable<int> student_number { get; set; }
        public Nullable<int> free_slot { get; set; }
        public string state { get; set; }
        public string learn_week { get; set; }
        public int day_2 { get; set; }
        public int start_lesson_2 { get; set; }
        public Nullable<int> student_registered_number { get; set; }
        public Nullable<int> start_week { get; set; }
        public Nullable<int> end_week { get; set; }
        public string note_1 { get; set; }
        public string note_2 { get; set; }
        public int term_id { get; set; }
        public string major_id { get; set; }
        public string lecturer_id { get; set; }
        public string curriculum_id { get; set; }
    
        public virtual curriculum curriculum { get; set; }
        public virtual lecturer lecturer { get; set; }
        public virtual major major { get; set; }
        public virtual term term { get; set; }
    }
}
