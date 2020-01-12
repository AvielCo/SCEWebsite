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
            var examDal = new ExamDal();
            var courses = (from x
                           in courseDal.Courses
                           select x).ToList();
            var exams = (from x
                         in examDal.Exams
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
                return View("ChangeSchedule", new CoursesExamsViewModel { Courses = courses, Exams = exams});
            }
            return View("FaAdminMenu", new CourseViewModel { Courses = courses });
        }

        public ActionResult AddCourse()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "FaAdmin") return View("~/Views/Error.cshtml");
            //Add course
            var cname = Request.Form["CourseName"];
            var lid = Request.Form["SelectedLecturer"];
            var day = Request.Form["Day"];
            int shour = Convert.ToInt32(Request.Form["StartHour"]), 
                fhour = Convert.ToInt32(Request.Form["FinishHour"]);
            if (fhour <= shour) return RedirectToAction("Menu");
            var classroom = Request.Form["Classroom"];
            var courseDal = new CourseDal();
            var course = (from x
                          in courseDal.Courses
                          where x.CourseName.Equals(cname)
                          select x.CourseName).SingleOrDefault();
            if (course != null)
            {
                TempData["Error"] = "Course " + course + " already exists in the system";
                return RedirectToAction("Menu"); //course already exist
            }
            var schedule = (from x
                            in courseDal.Courses
                            where x.Day.Equals(day) &&
                            x.Classroom.Equals(classroom)
                            select x).ToList();
            if (IsClashingCourse(shour, fhour, schedule)) return RedirectToAction("Menu"); //schedule is occupied for classroom
            var scheduleLecturer = (from x
                     in courseDal.Courses
                     where x.Day.Equals(day) &&
                     x.LecturerID.Equals(lid)
                     select x).ToList();
            if(IsClashingCourse(shour, fhour, scheduleLecturer)) return RedirectToAction("Menu"); //lecturer is occupied for lecturer
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-GH6DGFT\\AVIEL;Initial Catalog=sce_website;Integrated Security=True");
            conn.Open();
            var query = "INSERT INTO tblCourses " +
                        "(CourseName, Day, Classroom, LecturerID, StartHour, FinishHour) " +
                        "VALUES (@cname, @day, @classroom, @lid, @shour, @fhour)";
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.Add("@cname", SqlDbType.NVarChar, 50).Value = cname;
            command.Parameters.Add("@day", SqlDbType.NVarChar, 50).Value = day;
            command.Parameters.Add("@shour", SqlDbType.Int).Value = shour;
            command.Parameters.Add("@fhour", SqlDbType.Int).Value =fhour;
            command.Parameters.Add("@classroom", SqlDbType.NVarChar, 50).Value = classroom;
            command.Parameters.Add("@lid", SqlDbType.NVarChar, 50).Value = lid;
            //Add exam schedule to course
            string ashour = Request.Form["ExamAStartHour"],
                afhour = Request.Form["ExamAFinishHour"],
                bshour = Request.Form["ExamBStartHour"],
                bfhour = Request.Form["ExamBFinishHour"];
            int astarthour = Convert.ToInt32(ashour), afinishhour = Convert.ToInt32(afhour);
            int bstarthour = Convert.ToInt32(bshour), bfinishhour = Convert.ToInt32(bfhour);
            var examDal = new ExamDal();
            if (astarthour >= afinishhour)
            {
                TempData["Error"] = "Start hour cannot be after finish hour.";
                return RedirectToAction("Menu");
            }
            if (bstarthour >= bfinishhour)
            {
                TempData["Error"] = "Start hour cannot be after finish hour.";
                return RedirectToAction("Menu");
            }
            var examADate = Convert.ToDateTime(Request.Form["ADate"]);
            var examBDate = Convert.ToDateTime(Request.Form["BDate"]);
            if (DateTime.Compare(examADate, DateTime.Now) <= 0 ||
                DateTime.Compare(examBDate, DateTime.Now) <= 0 ||
                DateTime.Compare(examADate, examBDate) >= 0)
            {
                TempData["Error"] = "No time-machines invented yet.";
                return RedirectToAction("Menu");
            }
            var exams = (from x
                        in examDal.Exams
                         where x.Classroom.Equals(classroom) &&
                         !x.CourseName.Equals(cname)
                         select x).ToList();
            if (IsClashingExam(astarthour, afinishhour, examADate, exams) ||
                IsClashingExam(bstarthour, bfinishhour, examBDate, exams))
            {
                TempData["Error"] = "Hours clashing with other exam.";
                return RedirectToAction("Menu");
            }
            var addExamQ = "INSERT INTO tblExams " +
                            "(CourseName, Classroom, ExamADate, ExamBDate, ExamAStart, ExamAFinish, ExamBStart, ExamBFinish) " +
                            "VALUES (@cname, @classroom, @adate, @bdate, @astart, @afinish, @bstart, @bfinish)";
            SqlCommand examCommand = new SqlCommand(addExamQ, conn);
            examCommand.Parameters.Add("@cname", SqlDbType.NVarChar, 50).Value = cname;
            examCommand.Parameters.Add("@adate", SqlDbType.Date).Value = examADate;
            examCommand.Parameters.Add("@bdate", SqlDbType.Date).Value = examBDate;
            examCommand.Parameters.Add("@astart", SqlDbType.Int).Value = astarthour;
            examCommand.Parameters.Add("@afinish", SqlDbType.Int).Value = afinishhour;
            examCommand.Parameters.Add("@bstart", SqlDbType.Int).Value = bstarthour;
            examCommand.Parameters.Add("@bfinish", SqlDbType.Int).Value = bfinishhour;
            examCommand.Parameters.Add("@classroom", SqlDbType.NVarChar, 50).Value = classroom;
            //assign course to lecturer
            var addCourseToLecturer = "INSERT INTO tblLecturers " +
                                      "(LecturerID, CourseName) " +
                                      "VALUES (@lid, @cname)";
            SqlCommand lecturerCommand = new SqlCommand(addCourseToLecturer, conn);
            lecturerCommand.Parameters.Add("@lid", SqlDbType.NVarChar, 50).Value = lid;
            lecturerCommand.Parameters.Add("@cname", SqlDbType.NVarChar, 50).Value = cname;
            command.ExecuteNonQuery();
            examCommand.ExecuteNonQuery();
            lecturerCommand.ExecuteNonQuery();
            conn.Close();
            TempData["Success"] = "Course " + cname + " added successfully";
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
            if (students == null)
            {
                TempData["Error"] = "No sudents to assign available in the system.";
                return RedirectToAction("Menu");
            }
            var courses = (from x
                           in studentDal.Students
                           where x.StudentId.Equals(sid)
                           select x.CourseName).ToList();
            if (courses.Contains(scourse))
            {
                TempData["Error"] = "The student already assigned to " + scourse;
                return RedirectToAction("Menu");
            }
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-GH6DGFT\\AVIEL;Initial Catalog=sce_website;Integrated Security=True");
            conn.Open();
            var query = "INSERT INTO tblStudents " +
                        "(StudentID, CourseName, CourseGrade, ExamGrade) " +
                        "VALUES (@sid, @cname, @grade, @egrade)";
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.Add("@sid", SqlDbType.NVarChar, 50).Value = sid;
            command.Parameters.Add("@cname", SqlDbType.NVarChar, 50).Value = scourse;
            command.Parameters.Add("@grade", SqlDbType.NVarChar, 50).Value = null;
            command.Parameters.Add("@egrade", SqlDbType.NVarChar, 50).Value = null;
            var result = command.ExecuteNonQuery();
            if (result == 0)
            {
                TempData["Error"] = "Failure to assign student!";
                return RedirectToAction("Menu"); //error while inserting new values
            }
            conn.Close();
            TempData["Success"] = "Added student " + sid + " to course " + scourse + " successfully.";
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
                TempData["Success"] = "Grade changed successfully for student " + sid + ".";
            }
            return RedirectToAction("GetStudentsInCourse");

        }

        public ActionResult ChangeCourseSchedule()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "FaAdmin") return View("~/Views/Error.cshtml");
            string cname = Request.Form["SelectedCourse"],
                classroom = Request.Form["Classroom"],
                day = Request.Form["Day"],
                shour = Request.Form["StartHour"],
                fhour = Request.Form["FinishHour"];
            if (classroom == null && day == null && shour == null && fhour == null)
                return RedirectToAction("Menu");
            int starthour, endhour;
            var courseDal = new CourseDal();
            var currentCourse = (from x
                                 in courseDal.Courses
                                 where x.CourseName.Equals(cname)
                                 select x).SingleOrDefault();
            if (classroom == null) classroom = currentCourse.Classroom;
            if (day == null) day = currentCourse.Day;
            if (shour == null) starthour = currentCourse.StartHour;
            else starthour = Convert.ToInt32(shour);
            if (fhour == null) endhour = currentCourse.FinishHour;
            else endhour = Convert.ToInt32(fhour);
            if (starthour >= endhour) return RedirectToAction("Menu") ;
            var courses = (from x
                           in courseDal.Courses
                           where x.Classroom.Equals(classroom) && x.Day.Equals(day) && !x.CourseName.Equals(cname)
                           select x).ToList();
            if (IsClashingCourse(starthour, endhour, courses)) //is clashing with other course 
            {
                TempData["Error"] = "The time window is already occupied by other event.";
                return RedirectToAction("Menu");
            }
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-GH6DGFT\\AVIEL;Initial Catalog=sce_website;Integrated Security=True");
            conn.Open();
            string query;
                query = "UPDATE tblCourses " +
                            "SET Day = @day, " +
                            "Classroom = @classroom, " +
                            "StartHour = @shour, " +
                            "FinishHour = @fhour " +
                            "WHERE CourseName = @cname";
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.Add("@cname", SqlDbType.NVarChar, 50).Value = cname;
            command.Parameters.Add("@day", SqlDbType.NVarChar, 50).Value = day;
            command.Parameters.Add("@shour", SqlDbType.Int).Value = shour;
            command.Parameters.Add("@fhour", SqlDbType.Int).Value = fhour;
            command.Parameters.Add("@classroom", SqlDbType.NVarChar, 50).Value = classroom;
            command.ExecuteNonQuery();
            conn.Close();
            TempData["Success"] = "Course schedule changed successfully.";
            return RedirectToAction("Menu");
        }

        public ActionResult ChangeExamSchedule()
        {
            if (Session["Permission"] == null || Session["Permission"].ToString() != "FaAdmin") return View("~/Views/Error.cshtml");
            string exam = Request.Form["SelectedExam"],
                classroom = Request.Form["Classroom"],
                cname = Request.Form["SelectedCourse"],
                shour = Request.Form["StartHour"],
                fhour = Request.Form["FinishHour"];
            var examDal = new ExamDal();
            if (classroom == null || shour == null || fhour == null || exam == null)
            {
                TempData["Error"] = "The input is empty!";
                return RedirectToAction("Menu");
            }
            int starthour = Convert.ToInt32(shour), finishhour = Convert.ToInt32(fhour);
            if (starthour >= finishhour)
            {
                TempData["Error"] = "Start hour cannot be after finish hour.";
                return RedirectToAction("Menu");
            }
            var date = Convert.ToDateTime(Request.Form["Date"]);
            if (DateTime.Compare(date, DateTime.Now) <= 0)
            {
                TempData["Error"] = "No time-machines invented yet.";
                return RedirectToAction("Menu");
            }
            if (exam == "ExamA") 
            {
                var examB = (from x
                             in examDal.Exams
                             where x.CourseName.Equals(cname)
                             select x.ExamBDate).SingleOrDefault();
                if (examB == null || DateTime.Compare(date, examB) >= 0)
                {
                    TempData["Error"] = "Exam A cannot be after Exam B.";
                    return RedirectToAction("Menu");
                }
            }
            if(exam == "ExamB")
            {
                var examA = (from x
                             in examDal.Exams
                             where x.CourseName.Equals(cname)
                             select x.ExamADate).SingleOrDefault();
                if (examA == null || DateTime.Compare(date, examA) <= 0)
                {
                    TempData["Error"] = "Exam B cannot be before exam A.";
                    return RedirectToAction("Menu");
                }
            }
            var exams = (from x
                        in examDal.Exams
                        where x.Classroom.Equals(classroom) &&
                        !x.CourseName.Equals(cname)
                        select x).ToList();
            if (IsClashingExam(starthour, finishhour, date, exams))
            {
                TempData["Error"] = "Hours clashing with other exam.";
                return RedirectToAction("Menu");
            }
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-GH6DGFT\\AVIEL;Initial Catalog=sce_website;Integrated Security=True");
            conn.Open();

            var query = "UPDATE tblExams " +
                        "SET Classroom = @classroom, " +
                        exam + "Date = @adate, " +
                        exam + "Start = @shour, " +
                        exam + "Finish = @fhour " +
                        "WHERE CourseName = @cname";
            SqlCommand command = new SqlCommand(query, conn);
            command.Parameters.Add("@cname", SqlDbType.NVarChar, 50).Value = cname;
            command.Parameters.Add("@adate", SqlDbType.Date).Value = date;
            command.Parameters.Add("@shour", SqlDbType.Int).Value = shour;
            command.Parameters.Add("@fhour", SqlDbType.Int).Value = fhour;
            command.Parameters.Add("@classroom", SqlDbType.NVarChar, 50).Value = classroom;
            command.ExecuteNonQuery();
            conn.Close();
            TempData["Success"] = "Exam schedule changed successfully.";
            return RedirectToAction("Menu");
        }

        private bool IsClashingCourse(int startHour, int finishHour, List<Course> courses)
        {
            for(int i = 0; i < courses.Count(); i++)
                if (IsClashingHour(startHour, finishHour, courses[i].StartHour, courses[i].FinishHour))
                    return true;
            return false;
        }

        private bool IsClashingExam(int startHour, int finishHour, DateTime dateToCheck, List<Exam> exams)
        {
            for(int i = 0; i < exams.Count(); i++)
            {
                if (dateToCheck.Equals(exams[i].ExamADate) && IsClashingHour(startHour, finishHour, exams[i].ExamAStart, exams[i].ExamAFinish))
                     return true;
                else if (dateToCheck.Equals(exams[i].ExamBDate) && IsClashingHour(startHour, finishHour, exams[i].ExamBStart, exams[i].ExamBFinish))
                     return true;
            }
            return false;
        }

        private bool IsClashingHour(int startHour_src, int finishHour_src, int startHour_dest, int finishHour_dest)
        {
            bool a = startHour_src <= startHour_dest &&
                finishHour_src <= finishHour_dest &&
                finishHour_src > startHour_dest;

            bool b = startHour_src > startHour_dest &&
                finishHour_src < finishHour_dest;

            bool c = startHour_src < startHour_dest &&
                finishHour_src > finishHour_dest;

            bool d = startHour_src > startHour_dest &&
                finishHour_src > finishHour_dest &&
                startHour_src < finishHour_dest;
            if (a || b || c || d)
                return true;
            return false;
            
        }
    }
}