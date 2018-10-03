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
        public Connection connection;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Connection.instance.Initialize();   
        }

        //public void GetStudentsFromCourse(string courseId)
        //{
        //    Console.WriteLine("START\t" + Thread.CurrentThread.ManagedThreadId);
        //    ListStudentsResponse studentsResponse = connection.RetrieveStudentsInCourse(courseId);

        //    Dispatcher.Invoke(() =>
        //    {
        //        studentBox.AppendText(courseId);
        //        if (studentsResponse.Students != null && studentsResponse.Students.Count > 0)
        //        {
        //            foreach (var student in studentsResponse.Students)
        //            {
        //                studentBox.AppendText(student.Profile.Name.GivenName + " " + student.ETag);
        //                studentBox.AppendText("\n");
        //            }
        //            studentBox.AppendText("\n");
        //        }
        //    });
        //    Console.WriteLine("END\t" + Thread.CurrentThread.ManagedThreadId);
        //}
    }
}
