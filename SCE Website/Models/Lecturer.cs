using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCE_Website.Models
{
    [Table("tblLecturers")]
    public class Lecturer
    {

        [Required]
        [Key, Column(Order = 0)]
        public string LecturerId { get; set; }
        [Required]
        [Key, Column(Order = 1)]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Course Name must be under between 2 and 50 characters.")]
        public string CourseName { get; set; }
    }
}