using SCE_Website.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SCE_Website.Dal
{
	public class ExamDal : DbContext
	{
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Exam>().ToTable("tblExams");
        }
        public DbSet<Exam> Exams { get; set; }
    }
}