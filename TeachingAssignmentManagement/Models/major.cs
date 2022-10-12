﻿//------------------------------------------------------------------------------
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
    using System.ComponentModel.DataAnnotations;

    public partial class major
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public major()
        {
            this.curriculum_class = new HashSet<curriculum_class>();
        }

        [Required(ErrorMessage = "Bạn chưa nhập mã ngành")]
        [MaxLength(50, ErrorMessage = "Tối đa {1} kí tự được cho phép")]
        [RegularExpression(@"^[A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Chỉ được nhập chữ cái không dấu và không có khoảng trắng!")]
        public string id { get; set; }
        [Required(ErrorMessage = "Bạn chưa nhập tên ngành")]
        [MaxLength(255, ErrorMessage = "Tối đa {1} kí tự được cho phép")]
        public string name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<curriculum_class> curriculum_class { get; set; }
    }
}
