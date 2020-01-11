using SCE_Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCE_Website.ViewModel
{
    public class CoursesExamsViewModel
    {
        public List<Course> Courses { get; set; }
        public List<Exam> Exams { get; set; }
    }
}