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
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Student ID must be under 9 characters")]
        public string StudentId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be under 50 characters and greater than 2 characters.")]
        [Key, Column(Order = 1)]
        public string CourseName { get; set; }
        [RegularExpression("^(0|[1-9][0-9]?|100)$", ErrorMessage = "Grade must be between 0 to 100.")]
        public string ExamGrade { get; set; }
        [RegularExpression("^(0|[1-9][0-9]?|100)$", ErrorMessage = "Grade must be between 0 to 100.")]
        public string CourseGrade { get; set; }
    }
}