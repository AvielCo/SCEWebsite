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
		[RegularExpression("\b(S | C | E)[1-9]+\b$")]
		public string Classroom { get; set; }
		[RegularExpression("^0[8-9] | 1[0-9]$")]
		public string HourA { get; set; }
		[RegularExpression("^0[8-9] | 1[0-9]$")]
		public string HourB { get; set; }
		public DateTime ExamA { get; set; } // YYYY-MM-DD
		public DateTime ExamB { get; set; } // YYYY-MM-DD
	}
}