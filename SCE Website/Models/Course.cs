using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCE_Website.Models
{
    [Table("tblCourses")]
    public class Course
    {
        [Key, Column(Order = 0)]
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Course name must be under 50 characters and greater than 2 characters.")]
        public string CourseName { get; set; }
        [Required]
        [RegularExpression("^\b((Mon|Tues|Wed(nes)?|Thur(s)?|Fri|Sat(ur)?|Sun)(day)?)\b$")]
        public string Day { get; set; }
        [Required]
        [RegularExpression("^0[8-9] | 1[0-9]$")]
        public int StartHour { get; set; }
        [RegularExpression("^0[8-9] | 1[0-9]$")]
        public int FinishHour { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Classroom must be under 50 characters and greater than 2 characters.")]
        public string Classroom { get; set; }
        [Required]
        public string LecturerID { get; set; }
    }
}