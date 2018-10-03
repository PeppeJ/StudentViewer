using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Classroom.v1;
using Google.Apis.Classroom.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;

namespace StudentViewer
{
    public class NewIdentifiableAddedEventArgs : EventArgs
    {
        public IIdentity Identity { get; }
        public NewIdentifiableAddedEventArgs(IIdentity identity)
        {
            Identity = identity;
        }
    }

    public class Connection : Control
    {
        static string[] Scopes = {
            ClassroomService.Scope.ClassroomAnnouncementsReadonly,
            ClassroomService.Scope.ClassroomCoursesReadonly,
            ClassroomService.Scope.ClassroomCourseworkMeReadonly,
            ClassroomService.Scope.ClassroomCourseworkStudentsReadonly,
            ClassroomService.Scope.ClassroomGuardianlinksMeReadonly,
            ClassroomService.Scope.ClassroomGuardianlinksStudentsReadonly,
            ClassroomService.Scope.ClassroomProfileEmails,
            ClassroomService.Scope.ClassroomProfilePhotos,
            ClassroomService.Scope.ClassroomPushNotifications,
            ClassroomService.Scope.ClassroomRostersReadonly,
            ClassroomService.Scope.ClassroomStudentSubmissionsMeReadonly,
            ClassroomService.Scope.ClassroomStudentSubmissionsStudentsReadonly,
        };
        static string ApplicationName = "StudentViewer LBS";
        private ClassroomService service;

        private static Connection conn;

        delegate string Identifiable();

        private Dictionary<string, CourseWrapper> courses = new Dictionary<string, CourseWrapper>();
        private Dictionary<string, StudentSubmissionWrapper> submissions = new Dictionary<string, StudentSubmissionWrapper>();
        private Dictionary<string, CourseworkWrapper> coursework = new Dictionary<string, CourseworkWrapper>();
        private Dictionary<string, TeacherWrapper> teachers = new Dictionary<string, TeacherWrapper>();
        private Dictionary<string, StudentWrapper> students = new Dictionary<string, StudentWrapper>();

        public event EventHandler<NewIdentifiableAddedEventArgs> NewCourseAdded;
        public event EventHandler<NewIdentifiableAddedEventArgs> NewStudentAdded;
        public event EventHandler<NewIdentifiableAddedEventArgs> NewTeacherAdded;

        public static Connection instance
        {
            get
            {
                if (conn == null)
                {
                    conn = new Connection();
                }
                return conn;
            }
        }

        private Connection()
        {

        }

        bool isRetrieving = false;
        public void RetrieveAll()
        {
            if (!isRetrieving)
            {
                isRetrieving = true;
                Task.Run(() =>
                {
                    Console.WriteLine("Started retrieving");
                    RetrieveCourses();
                    Console.WriteLine("Courses done");
                    foreach (var course in courses.Values.ToList())
                    {
                        RetrieveTeachersInCourse(course.ID);
                        Console.WriteLine($"Teachers done: {course.LongName}");
                        RetrieveStudentsInCourse(course.ID);
                        Console.WriteLine($"Students done: {course.LongName}");
                        RetrieveCourseWorkInCourse(course.ID);
                        Console.WriteLine($"Coursework done: {course.LongName}");
                    }
                    isRetrieving = false;
                    Console.WriteLine("Done retrieving");
                });
            }
            else
            {
                Console.WriteLine("Busy");
            }
        }

        public void Dump()
        {
            List<IIdentity> allItems = new List<IIdentity>();
            allItems.AddRange(courses.Values);
            allItems.AddRange(students.Values);
            allItems.AddRange(teachers.Values);
            foreach (var item in allItems)
            {
                Console.WriteLine(item);
            }
        }

        public CourseWrapper GetCourse(string courseId)
        {
            if (!courses.ContainsKey(courseId))
            {
                throw new NotImplementedException();
            }
            else
            {
                return courses[courseId];
            }
        }

        public StudentSubmissionWrapper GetSubmission(string submissionId)
        {
            if (!submissions.ContainsKey(submissionId))
            {
                throw new NotImplementedException();
            }
            else
            {
                return submissions[submissionId];
            }
        }

        public StudentWrapper GetStudent(string studentId)
        {
            if (!students.ContainsKey(studentId))
            {
                throw new NotImplementedException();
            }
            else
            {
                return students[studentId];
            }
        }

        public CourseworkWrapper GetCoursework(string courseworkId)
        {
            if (!coursework.ContainsKey(courseworkId))
            {
                throw new NotImplementedException();
            }
            else
            {
                return coursework[courseworkId];
            }
        }

        public TeacherWrapper GetTeacher(string teacherId)
        {
            if (!teachers.ContainsKey(teacherId))
            {
                throw new NotImplementedException();
            }
            else
            {
                return teachers[teacherId];
            }
        }

        public void Initialize()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Classroom API service.
            service = new ClassroomService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
                GZipEnabled = true,
            });
        }

        public void RetrieveCourses()
        {
            var listRequest = service.Courses.List();
            var response = listRequest.Execute();
            if (response.Courses != null && response.Courses.Count > 0)
            {
                foreach (var c in response.Courses)
                {
                    CourseWrapper cw = new CourseWrapper(c);
                    courses.AddUnique(cw.ID, cw);
                    NewCourseAdded?.Invoke(this, new NewIdentifiableAddedEventArgs(cw));
                }
            }
        }

        public void RetrieveStudentsInCourse(string courseId)
        {
            var listRequest = service.Courses.Students.List(courseId);
            var response = listRequest.Execute();
            if (response.Students != null && response.Students.Count > 0)
            {
                foreach (var s in response.Students)
                {
                    StudentWrapper sw = new StudentWrapper(s);
                    students.AddUnique(sw.ID, sw);
                    NewStudentAdded?.Invoke(this, new NewIdentifiableAddedEventArgs(sw));
                }
            }
        }

        public void RetrieveCourseWorkInCourse(string courseId)
        {
            var listRequest = service.Courses.CourseWork.List(courseId);
            var response = listRequest.Execute();
            if (response.CourseWork != null && response.CourseWork.Count > 0)
            {
                foreach (var w in response.CourseWork)
                {
                    CourseworkWrapper cw = new CourseworkWrapper(w);
                    coursework.AddUnique(cw.ID, cw);
                }
            }
        }

        public void RetrieveTeachersInCourse(string courseId)
        {
            var listRequest = service.Courses.Teachers.List(courseId);
            var response = listRequest.Execute();
            if (response.Teachers != null && response.Teachers.Count > 0)
            {
                foreach (var t in response.Teachers)
                {
                    TeacherWrapper tw = new TeacherWrapper(t);
                    teachers.AddUnique(tw.ID, tw);
                    NewTeacherAdded?.Invoke(this, new NewIdentifiableAddedEventArgs(tw));
                }
            }
        }

        public ListStudentSubmissionsResponse RetrieveStudentSubmissionsInCourse(string courseId, string courseworkId)
        {
            throw new NotImplementedException();
            var listRequest = service.Courses.CourseWork.StudentSubmissions.List(courseId, courseworkId);
            var response = listRequest.Execute();
            return response;
        }
    }
}
