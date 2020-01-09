using SCE_Website.Dal;
using SCE_Website.Models;
using SCE_Website.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace SCE_Website.Controllers
{
    public class LecturerController : Controller
    {
        public ActionResult Menu()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "Lecturer") return View("~/Views/Error.cshtml");
            if (Request.Form["ShowCoursesSchedule"] != null)
                return RedirectToAction("GetCoursesSchedule");
            if (Request.Form["ShowCourseStudents"] != null)
            {
                var lecturerDal = new LecturerDal();
                var id = Session["UserID"].ToString();
                var courses = (from x
                              in lecturerDal.Lecturers
                              where x.LecturerId.Equals(id)
                              select x.CourseName).ToList();
                Session["ListCourses"] = courses;
                return View("ShowCourseStudents");
            }
            return View("LecturerMenu");
        }

        public ActionResult GetCoursesSchedule()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "Lecturer") return View("~/Views/Error.cshtml");
            var lecturerDal = new LecturerDal();
            var courseDal = new CourseDal();
            var courses = new List<Course>();
            var id = Session["UserID"].ToString();
            var lecturers = (from x
                           in lecturerDal.Lecturers
                            where x.LecturerId.Equals(id)
                            select x).ToList();
            for (int i = 0; i < lecturers.Count(); i++)
            {
                var t = lecturers[i].CourseName;
                courses.Add((from x
                            in courseDal.Courses
                             where x.CourseName.Equals(t)
                             select x).SingleOrDefault());
            }
            return View("ShowCoursesSchedule", new CourseViewModel { Courses = courses });
        }

        public ActionResult GetStudentsInMyCourses()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "Lecturer") return View("~/Views/Error.cshtml");
            if (Request.Form["SelectedCourse"] != null)
                Session["CurrentCourse"] = Request.Form["SelectedCourse"];
            var cs = new List<CourseStudents>();
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-GH6DGFT\\AVIEL;Initial Catalog=sce_website;Integrated Security=True");
            conn.Open();
            string query = "SELECT StudentID, Name, CourseGrade, ExamGrade FROM tblUsers, tblStudents WHERE ID = StudentID AND CourseName = @c";
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.Add("@c", SqlDbType.NVarChar, 50).Value = Session["CurrentCourse"];
            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                cs.Add(new CourseStudents
                {
                    StudentId = dr["StudentID"].ToString(),
                    Name = dr["Name"].ToString(),
                    CourseGrade = dr["CourseGrade"].ToString(),
                    ExamGrade = dr["ExamGrade"].ToString()
                }) ; 
            }
            conn.Close();
            return View("ShowCourseStudents", cs);
           
        }

        public ActionResult ChangeStudentGrade()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "Lecturer") return View("~/Views/Error.cshtml");
            var sid = Request.Form["StudentId"];
            var newgrade = Request.Form["NewGrade"];
            var examg = Request.Form["ExamGrade"];
            var course = Session["CurrentCourse"];
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-GH6DGFT\\AVIEL;Initial Catalog=sce_website;Integrated Security=True");
            if (newgrade.Length != 0 || examg.Length != 0)
            {
                conn.Open();
                if (newgrade.Length == 0)
                {
                    var s = "SELECT CourseGrade " +
                            "FROM tblStudents " +
                            "WHERE StudentID = @sid " +
                            "AND CourseName = @cn";
                    SqlCommand command = new SqlCommand(s, conn);
                    command.Parameters.Add("@sid", SqlDbType.NVarChar, 50).Value = sid;
                    command.Parameters.Add("@cn", SqlDbType.NVarChar, 50).Value = course;
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    DataRow dr = dt.Rows[0];
                    newgrade = dr["CourseGrade"].ToString();
                }

                if (examg.Length == 0)
                {
                    var s = "SELECT ExamGrade " +
                            "FROM tblStudents " +
                            "WHERE StudentID = @sid " +
                            "AND CourseName = @cn";
                    SqlCommand command = new SqlCommand(s, conn);
                    command.Parameters.Add("@sid", SqlDbType.NVarChar, 50).Value = sid;
                    command.Parameters.Add("@cn", SqlDbType.NVarChar, 50).Value = course;
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    DataRow dr = dt.Rows[0];
                    newgrade = dr["ExamGrade"].ToString();
                }

                string UpdateStudent = "UPDATE tblStudents " +
                               "SET CourseGrade = @ng, " +
                               "ExamGrade = @eg " +
                               "WHERE StudentID = @sid " +
                               "AND CourseName = @cn";
                SqlCommand UpdateComm = new SqlCommand(UpdateStudent, conn);
                UpdateComm.Parameters.Add("@ng", SqlDbType.NVarChar, 50).Value = newgrade;
                UpdateComm.Parameters.Add("@sid", SqlDbType.NVarChar, 50).Value = sid;
                UpdateComm.Parameters.Add("@cn", SqlDbType.NVarChar, 50).Value = course;
                UpdateComm.Parameters.Add("@eg", SqlDbType.NVarChar, 50).Value = examg;
                UpdateComm.ExecuteNonQuery();
                conn.Close();
            }
            return RedirectToAction("GetStudentsInMyCourses");

        }

    }
}