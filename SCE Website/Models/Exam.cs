using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCE_Website.Models
{
	[Table("tblExams")]
	public class Exam
	{
		[Key, Column(Order = 0)]
		[Required]
		[StringLength(50, MinimumLength = 2, ErrorMessage = "Course name must be under 50 characters and greater than 2 characters.")]
		public string CourseName { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 2, ErrorMessage = "Classroom must be under 50 characters and greater than 2 characters.")]
		public string Classroom { get; set; }
		public int ExamAStart { get; set; }
		public int ExamBStart { get; set; }
		public int ExamAFinish { get; set; }
		public int ExamBFinish { get; set; }
		public string ExamADate { get; set; } // YYYY-MM-DD
		public string ExamBDate { get; set; } // YYYY-MM-DD
	}
}