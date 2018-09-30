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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
        static ClassroomService service;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
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
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Classroom API service.
            service = new ClassroomService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            CoursesResource.ListRequest courseRequest = service.Courses.List();
            ListCoursesResponse courseResponse = courseRequest.Execute();

            if (courseResponse.Courses != null && courseResponse.Courses.Count > 0)
            {
                foreach (var course in courseResponse.Courses)
                {
                    courseBox.AppendText(course.Name);
                    courseBox.AppendText("\n");

                    Task t1 = Task.Factory.StartNew(() => { GetStudentsFromCourse(course.Id); });
                }
            }
        }
        delegate void Getter(string id);

        public void GetStudentsFromCourse(string courseId)
        {
            Console.WriteLine("START\t" + Thread.CurrentThread.ManagedThreadId);
            var studentRequest = service.Courses.Students.List(courseId);
            ListStudentsResponse studentsResponse = studentRequest.Execute();

            Dispatcher.Invoke(() =>
            {
                studentBox.AppendText(courseId);
                foreach (var student in studentsResponse.Students)
                {
                    studentBox.AppendText(student.Profile.Name.GivenName + " " + student.ETag);
                    studentBox.AppendText("\n");
                }
                studentBox.AppendText("\n");
            });

            Console.WriteLine("END\t" + Thread.CurrentThread.ManagedThreadId);

        }
    }
}
