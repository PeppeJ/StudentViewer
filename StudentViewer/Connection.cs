using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Classroom.v1;
using Google.Apis.Classroom.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;

namespace StudentViewer
{
    public class Connection
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

        public Connection()
        {

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
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Classroom API service.
            service = new ClassroomService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public ListCoursesResponse ListCourses()
        {
            ListCoursesResponse response;
            CoursesResource.ListRequest listRequest;
            listRequest = service.Courses.List();
            response = listRequest.Execute();
            return response;
        }

        public ListStudentsResponse ListStudentsInCourse(string courseId)
        {
            ListStudentsResponse response;
            CoursesResource.StudentsResource.ListRequest listRequest;
            listRequest = service.Courses.Students.List(courseId);
            response = listRequest.Execute();
            return response;
        }


    }
}
