using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Classroom.v1.Data;

namespace StudentViewer
{
    public class CourseworkWrapper : IIdentity
    {
        private CourseWork coursework;

        private CourseWrapper course;
        private List<StudentSubmissionWrapper> submissions;

        public CourseworkWrapper(CourseWork newCoursework)
        {
            coursework = newCoursework;
        }

        public string ID => coursework.Id;

        public string ShortName => coursework.Title;

        public string LongName => $"{coursework.Title} - {course.ShortName}";

        public override string ToString()
        {
            return $"Coursework: {LongName}";
        }
    }
}
