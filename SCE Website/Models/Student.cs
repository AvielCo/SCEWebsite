using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCE_Website.Models
{
    [Table("tblStudents")]
    public class Student
    {

        [Required]
        [Key, Column(Order = 0)]
        public string StudentId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Course Name must be under between 2 and 50 characters.")]
        [Key, Column(Order = 1)]
        public string CourseName { get; set; }
        [RegularExpression("^(0|[1-9][0-9]?|100)$", ErrorMessage = "Grade must be between 0 to 100.")]
        public string ExamGrade { get; set; }
        [RegularExpression("^(0|[1-9][0-9]?|100)$", ErrorMessage = "Grade must be between 0 to 100.")]
        public string CourseGrade { get; set; }
    }
}