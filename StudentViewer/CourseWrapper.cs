using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Classroom.v1.Data;

namespace StudentViewer
{
    public class CourseWrapper : IIdentity
    {
        private Course course;

        private List<StudentWrapper> students;
        private List<TeacherWrapper> teachers;
        private List<CourseworkWrapper> courseworks;

        public CourseWrapper(Course newCourse)
        {
            course = newCourse;
        }

        public string ID => course.Id;

        public string ShortName => course.Name;

        public string LongName => $"{course.Name} - {course.Section}";

        public override string ToString()
        {
            return $"Course: {LongName}";
        }
    }
}
