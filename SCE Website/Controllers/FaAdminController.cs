using SCE_Website.Dal;
using SCE_Website.Models;
using SCE_Website.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCE_Website.Controllers
{
    public class FaAdminController : Controller
    {
        public ActionResult Menu()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "FaAdmin") return View("~/Views/Error.cshtml"); 
            var courseDal = new CourseDal();
            var courses = (from x
                           in courseDal.Courses
                           select x).ToList();
            var coursesName = (from x
                               in courseDal.Courses
                               select x.CourseName).ToList();
            if (Request.Form["AddNewCourse"] != null)
            {
                var userDal = new UserDal();
                var lecturers = (from x
                                 in userDal.Users
                                 where x.PermissionType.Equals("Lecturer")
                                 select x.ID).ToList();
                Session["ListLecturers"] = lecturers;
                return View("AddNewCourse");
            }
            if(Request.Form["CourseList"] != null)
            {
                Session["ListCoursesName"] = coursesName;
                return View("ChangeStudentGrade");
            }
            if(Request.Form["AssignStudents"] != null)
            {
                var userDal = new UserDal();
                var students = (from x
                                in userDal.Users
                                where x.PermissionType.Equals("Student")
                                select x).ToList();
                Session["ListCourses"] = coursesName;
                return View("AssignStudents", new UserViewModel { Users = students });
            }
            if (Request.Form["ChangeSchedule"] != null) 
            {
                Session["ListCourses"] = coursesName;
                return View("ChangeSchedule");
            }
            return View("FaAdminMenu", new CourseViewModel { Courses = courses });
        }

        public ActionResult AddCourse()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "FaAdmin") return View("~/Views/Error.cshtml");
            var cname = Request.Form["CourseName"];
            var lid = Request.Form["SelectedLecturer"];
            var day = Request.Form["Day"];
            var hour = Request.Form["Hour"];
            var classroom = Request.Form["Classroom"];
            var courseDal = new CourseDal();
            var course = (from x
                          in courseDal.Courses
                          where x.CourseName.Equals(cname)
                          select x.CourseName).SingleOrDefault();
            if (course != null) return RedirectToAction("Menu"); //course already exist
            var schedule = (from x
                            in courseDal.Courses
                            where x.Day.Equals(day) &&
                            x.Hour.Equals(hour) &&
                            x.Classroom.Equals(classroom)
                            select x).SingleOrDefault();
            if (schedule != null) return RedirectToAction("Menu"); //schedule is occupied
            var a = (from x
                     in courseDal.Courses
                     where x.Day.Equals(day) &&
                     x.Hour.Equals(hour) &&
                     x.LecturerID.Equals(lid)
                     select x).SingleOrDefault();
            if(a != null) return RedirectToAction("Menu"); //lecturer is occupied
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-GH6DGFT\\AVIEL;Initial Catalog=sce_website;Integrated Security=True");
            conn.Open();
            var query = "INSERT INTO tblCourses " +
                        "(CourseName, Day, Hour, Classroom, LecturerID) " +
                        "VALUES (@cname, @day, @hour, @classroom, @lid)";
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.Add("@cname", SqlDbType.NVarChar, 50).Value = cname;
            command.Parameters.Add("@day", SqlDbType.NVarChar, 50).Value = day;
            command.Parameters.Add("@hour", SqlDbType.NVarChar, 50).Value = hour;
            command.Parameters.Add("@classroom", SqlDbType.NVarChar, 50).Value = classroom;
            command.Parameters.Add("@lid", SqlDbType.NVarChar, 50).Value = lid;
            var exams = "INSERT INTO tblExams " +
                        "(CourseName, Classroom, ExamA, ExamB) " +
                        "VALUES (@cname, @classroom, @a, @b)";
            SqlCommand examscommand = new SqlCommand(exams, conn);
            examscommand.Parameters.Add("@cname", SqlDbType.NVarChar, 50).Value = cname;
            examscommand.Parameters.Add("@classroom", SqlDbType.NVarChar, 50).Value = "NULL";
            examscommand.Parameters.Add("@a", SqlDbType.NVarChar, 50).Value = "NULL";
            examscommand.Parameters.Add("@b", SqlDbType.NVarChar, 50).Value = "NULL";
            var result = command.ExecuteNonQuery();
            var result_ = examscommand.ExecuteNonQuery();
            if (result == 0) return View(); //error while inserting new values
            conn.Close();
            return RedirectToAction("Menu");
        }

        public ActionResult AssignStudents()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "FaAdmin") return View("~/Views/Error.cshtml");
            var scourse = Request.Form["SelectedCourse"];
            var sid = Request.Form["StudentId"];
            var studentDal = new StudentDal();
            var userDal = new UserDal();
            var students = (from x
                            in userDal.Users
                            where x.ID.Equals(sid)
                            select x.ID).SingleOrDefault();
            if (students == null) return RedirectToAction("Menu");
            var courses = (from x
                           in studentDal.Students
                           where x.StudentId.Equals(sid)
                           select x.CourseName).ToList();
            if(courses.Contains(scourse)) return RedirectToAction("Menu");
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-GH6DGFT\\AVIEL;Initial Catalog=sce_website;Integrated Security=True");
            conn.Open();
            var query = "INSERT INTO tblStudents " +
                        "(StudentID, CourseName, CourseGrade, ExamGrade) " +
                        "VALUES (@sid, @cname, @grade, @egrade)";
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.Add("@sid", SqlDbType.NVarChar, 50).Value = sid;
            command.Parameters.Add("@cname", SqlDbType.NVarChar, 50).Value = scourse;
            command.Parameters.Add("@grade", SqlDbType.NVarChar, 50).Value = "NULL";
            command.Parameters.Add("@egrade", SqlDbType.NVarChar, 50).Value = "NULL";
            var result = command.ExecuteNonQuery();
            if (result == 0) return RedirectToAction("Menu"); //error while inserting new values
            conn.Close();
            return RedirectToAction("Menu");
        }

        public ActionResult GetStudentsInCourse()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "FaAdmin") return View("~/Views/Error.cshtml");
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
                });
            }
            conn.Close();
            return View("ChangeStudentGrade", cs);

        }

        public ActionResult ChangeStudentGrade()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "FaAdmin") return View("~/Views/Error.cshtml");
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
            return RedirectToAction("GetStudentsInCourse");

        }

        public ActionResult ChangeCourseSchedule()
        {
            //TODO
            return View("Menu");
        }

        public ActionResult ChangeExamSchedule()
        {
            //TODO
            return View("Menu");
        }
    }
}