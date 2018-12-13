using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace DapperCRUDApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string sqlConnectionString = @"Data Source = .;initial catalog=JarvisEF;user id=sa;password=1";
        int studentId;
        public MainWindow()
        {
            InitializeComponent();
            GetStudents();
        }

        //This method is responsible to get data from database.
        private void GetStudents()
        {
            List<Student> students = GetAllStudent();

            this.studentDataGrid.ItemsSource = students.OrderBy(x => x.Id);
            this.studentDataGrid.CanUserAddRows = false;
        }

        private void InsertOrUpdateStudent(object sender, RoutedEventArgs e)
        {
            if (btnsubmit.Content.ToString() == "Update")
            {
                if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtMarks.Text) || txtName.Text == " " || txtMarks.Text == " ")
                {
                    MessageBox.Show("Please enter name and city");
                }
                else
                {
                    MessageBox.Show(UpdateStudent(new Student() { Id = studentId, Name = txtName.Text, Marks = Convert.ToInt32(txtMarks.Text) }).ToString() + " row(s) affected");
                    GetStudents();
                    ClearControls();
                    btnsubmit.Content = "Submit";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtMarks.Text) || txtName.Text == " " || txtMarks.Text == " ")
                {
                    MessageBox.Show("Please enter name and marks");
                }
                else
                {
                    MessageBox.Show(InsertStudent(new Student() { Name = txtName.Text, Marks = Convert.ToInt32 (txtMarks.Text)}).ToString() + " row(s) affected");

                    GetStudents();
                    ClearControls();
                }
            }
        }

        //This method gets all record from student table
        private List<Student> GetAllStudent()
        {
            List<Student> students = new List<Student>();
            using (var connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                students = connection.Query<Student>("Select Id, Name, Marks from tbl_dapper_Student").ToList();
                connection.Close();
            }
            return students;
        }

        //This method inserts a student record in database
        private int InsertStudent(Student student)
        {
            using (var connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                var affectedRows = connection.Execute("Insert into tbl_dapper_Student (Name, Marks) values (@Name, @Marks)", new { Name = student.Name, Marks = student.Marks });
                connection.Close();
                return affectedRows;
            }
        }

        //This method update student record in database
        private int UpdateStudent(Student student)
        {
            using (var connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                var affectedRows = connection.Execute("Update tbl_dapper_Student set Name = @Name, Marks = @Marks Where Id = @Id", new { Id = studentId, Name = txtName.Text, Marks = txtMarks.Text });
                connection.Close();
                return affectedRows;
            }
        }

        //This method deletes a student record from database
        private int DeleteStudent(Student student)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                var affectedRows = connection.Execute("Delete from tbl_dapper_Student Where Id = @Id", new { Id = studentId });
                connection.Close();
                return affectedRows;
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {

            studentId = ((Student)studentDataGrid.SelectedItem).Id;
            txtName.Text = ((Student)studentDataGrid.SelectedItem).Name;
            txtMarks.Text = ((Student)studentDataGrid.SelectedItem).Marks.ToString();
            btnsubmit.Content = "Update";
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            studentId = ((Student)studentDataGrid.SelectedItem).Id;
            MessageBox.Show(DeleteStudent(new Student() { Id = studentId}).ToString() + " row(s) affected");


            GetStudents();
        }

        private void ClearControls()
        {
            txtName.Text = string.Empty;
            txtMarks.Text = string.Empty;
        }
    }

    //This class is will be mapped to student table in of database
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Marks { get; set; }
    }
}
