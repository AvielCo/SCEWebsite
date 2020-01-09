using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCE_Website.Models;

namespace SCE_Website.ViewModel
{
    public class StudentViewModel
    {
        public Student Student { get; set; }
        public List<Student> Students { get; set; }
    }
}