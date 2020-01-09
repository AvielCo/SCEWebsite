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
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Lecturer ID must be under 9 characters")]
        public string LecturerId { get; set; }
        [Required]
        [Key, Column(Order = 1)]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be under 50 characters and greater than 2 characters.")]
        public string CourseName { get; set; }
    }
}