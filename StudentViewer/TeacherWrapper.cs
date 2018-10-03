using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Classroom.v1.Data;

namespace StudentViewer
{
    public class TeacherWrapper : IIdentity
    {
        private Teacher teacher;

        private List<CourseWrapper> courses;

        public TeacherWrapper(Teacher teacher)
        {
            this.teacher = teacher;
        }

        public string ID => teacher.UserId;

        public string ShortName => teacher.Profile.Name.FullName;

        public string LongName => $"{teacher.Profile.Name.FullName}";

        public override string ToString()
        {
            return $"Teacher: {LongName}";
        }
    }
}
