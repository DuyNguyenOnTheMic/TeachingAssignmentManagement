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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CP25Team03Entities : DbContext
    {
        public CP25Team03Entities()
            : base("name=CP25Team03Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<academic_degree> academic_degree { get; set; }
        public virtual DbSet<academic_degree_rank> academic_degree_rank { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<class_section> class_section { get; set; }
        public virtual DbSet<coefficient> coefficients { get; set; }
        public virtual DbSet<lecturer> lecturers { get; set; }
        public virtual DbSet<lecturer_rank> lecturer_rank { get; set; }
        public virtual DbSet<major> majors { get; set; }
        public virtual DbSet<room> rooms { get; set; }
        public virtual DbSet<subject> subjects { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<term> terms { get; set; }
        public virtual DbSet<unit_price> unit_price { get; set; }
    }
}
