using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCE_Website.Models;
using SCE_Website.Dal;
using SCE_Website.ViewModel;


namespace SCE_Website.Controllers
{
    public class StudentController : Controller
    {
        public ActionResult Menu()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "Student") return View("~/Views/Error.cshtml");
            if (Request.Form["ShowCoursesGrade"] != null)
                return RedirectToAction("GetCoursesGrade");
            if (Request.Form["ShowCoursesSchedule"] != null)
                return RedirectToAction("GetCoursesSchedule");
            if (Request.Form["ShowExamSchedule"] != null)
                return RedirectToAction("GetExamSchedule");
            return View("StudentMenu");
        }
        public ActionResult GetCoursesGrade()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "Student") return View("~/Views/Error.cshtml");
            
            var studentDal = new StudentDal();
            var id = Session["UserID"].ToString();
            var students = (from x
                           in studentDal.Students
                           where x.StudentId.Equals(id)
                           select x).ToList();
            return View("ShowCoursesGrade", new StudentViewModel { Students = students });
        }

        public ActionResult GetCoursesSchedule()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "Student") return View("~/Views/Error.cshtml");
            var studentDal = new StudentDal();
            var courseDal = new CourseDal();
            var courses = new List<Course>();
            var id = Session["UserID"].ToString();
            var students = (from x
                           in studentDal.Students
                            where x.StudentId.Equals(id)
                            select x).ToList();
            for (int i = 0; i < students.Count(); i++)
            {
                var t = students[i].CourseName;
                courses.Add((from x
                            in courseDal.Courses
                             where x.CourseName.Equals(t)
                             select x).SingleOrDefault());
            }
            return View("ShowCoursesSchedule", new CourseViewModel { Courses = courses });
        }

        public ActionResult GetExamSchedule()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "Student") return View("~/Views/Error.cshtml");
            var id = Session["UserID"].ToString();
            var examDal = new ExamDal();
            var studentDal = new StudentDal();
            var exams = new List<Exam>();
            var students = (from x
                           in studentDal.Students
                            where x.StudentId.Equals(id)
                            select x).ToList();
            for (int i = 0; i < students.Count(); i++)
            {
                var t = students[i].CourseName;
                exams.Add((from x
                            in examDal.Exams
                             where x.CourseName.Equals(t)
                             select x).SingleOrDefault());
            }
            return View("ShowExamSchedule", new ExamViewModel { Exams = exams });
        }
    }
}