using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Classroom.v1.Data;

namespace StudentViewer
{
    public class StudentWrapper : IIdentity
    {
        public StudentWrapper(Student newStudent)
        {
            student = newStudent;
        }

        private Student student;
        private List<CourseWrapper> courses;
        private List<StudentSubmissionWrapper> submissions;

        public void AddCourse(string courseId)
        {
            CourseWrapper newCourse = Connection.instance.GetCourse(courseId);
            courses.Add(newCourse);
        }

        public void AddStudentSubmission(string submissionId)
        {
            StudentSubmissionWrapper newSubmission = Connection.instance.GetSubmission(submissionId);
            submissions.Add(newSubmission);
        }

        public string ID => student.UserId;

        public string ShortName => student.Profile.Name.FullName;

        public string LongName => ShortName;

        public override string ToString()
        {
            return $"Student: {LongName}";
        }
    }
}
