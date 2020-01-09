CREATE TABLE [dbo].[tblUsers](
   [ID] [nvarchar](50) NOT NULL,
   [Password] [nvarchar](50) NOT NULL,
   [Name] [nvarchar](50) NOT NULL,
   [PermissionType] [nvarchar](50) NOT NULL,
   CONSTRAINT [PK_tblUsers] PRIMARY KEY CLUSTERED ([ID] ASC)
) ON [PRIMARY]

CREATE TABLE [dbo].[tblStudents](
   [StudentID] [nvarchar](50) NOT NULL,
   [CourseName] [nvarchar](50) NOT NULL,
   [StudentGrade] [nvarchar](50) NOT NULL,
   CONSTRAINT [PK_tblCourses] PRIMARY KEY CLUSTERED ([StudentID], [CourseName] ASC)
) ON [PRIMARY]

CREATE TABLE [dbo].[tblLecturers](
   [LecturerID] [nvarchar](50) NOT NULL,
   [CourseName] [nvarchar](50) NOT NULL,
   CONSTRAINT [PK_tblLecturer] PRIMARY KEY CLUSTERED ([LecturerID], [CourseName] ASC)
) ON [PRIMARY]

CREATE TABLE [dbo].[tblCourses](
	[CourseName] [nvarchar](50) NOT NULL,
	[Day] [nvarchar](50) NOT NULL,
	[Hour] [nvarchar](50) NOT NULL,
	[Classroom] [nvarchar](50) NOT NULL,
	[LecturerID] [nvarchar](50) NOT NULL,
	CONSTRAINT [PK_tblCourses] PRIMARY KEY CLUSTERED (CourseName)
)

CREATE TABLE [dbo].[tblExams](
	[CourseName] [nvarchar](50) NOT NULL,
	[Classroom] [nvarchar](50) NOT NULL,
	[ExamA] [nvarchar](50),
	[ExamB] [nvarchar](50),
	[ExamGrade] [nvarchar](50),
	CONSTRAINT [PK_tblExams] PRIMARY KEY CLUSTERED (CourseName)
)
