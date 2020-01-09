using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCE_Website.Models;
using System.Data.Entity;

namespace SCE_Website.Dal
{
    public class LecturerDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Lecturer>().ToTable("tblLecturers");
        }
        public DbSet<Lecturer> Lecturers { get; set; }

    }
}