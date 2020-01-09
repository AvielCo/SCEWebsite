using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCE_Website.Models;
using System.Data.Entity;

namespace SCE_Website.Dal
{
    public class CourseDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Course>().ToTable("tblCourses");
        }
        public DbSet<Course> Courses { get; set; }

    }
}