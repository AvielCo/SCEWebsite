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
		[StringLength(50, MinimumLength = 2, ErrorMessage = "Course Name must be under between 2 and 50 characters.")]
		public string CourseName { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 2, ErrorMessage = "Classroom must be under 50 characters and greater than 2 characters.")]
		[RegularExpression("^(S|C|E|J|K)(0-9)(0-9)(0-9)$")]
		public string Classroom { get; set; }
		public int ExamAStart { get; set; }
		public int ExamBStart { get; set; }
		public int ExamAFinish { get; set; }
		public int ExamBFinish { get; set; }
		public DateTime ExamADate { get; set; } // YYYY-MM-DD
		public DateTime ExamBDate { get; set; } // YYYY-MM-DD
	}
}