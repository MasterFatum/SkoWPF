using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Bll.Concrete;
using BLL.Concrete;
using BLL.Entities;

namespace AdminSystem.Forms
{
    
    public partial class FormViewCoursesUser
    {
        public string User { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Lastname { get; set; }
        public string Category { get; set; }
        public new string Title { get; set; }
        public string Description { get; set; }
        public int? Evaluation { get; set; }
        public string Date { get; set; }
        public string Hyperlink { get; set; }
        public string FilePath { get; set; }
        public int Year { get; set; }

        CourseRepository _courseRepository = new CourseRepository();
        OtherRepository _otherRepository = new OtherRepository();
        
        public FormViewCoursesUser(int id, string lastname, string firstname, string middlename, string user)
        {
            InitializeComponent();

            CbxYear.ItemsSource = _courseRepository.GetYears(id);

            User = user;

            Id = id;

            LblUserId.Content = Id;

            Lastname = lastname;
            
            LblUsername.Content = String.Format($"{lastname} {firstname} {middlename}");
        }

        private void CbxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGridUserCourses.ItemsSource = _courseRepository.GetCoursesByUserId(Convert.ToInt32(LblUserId.Content), Convert.ToInt16(CbxYear.SelectedItem));
            _otherRepository.SettingDataGridUsers(DataGridUserCourses);

            TxbxAllRating.Text = _courseRepository.GetRatingByYear(Convert.ToInt32(LblUserId.Content),
                Convert.ToInt32(CbxYear.SelectedItem));

            if (TxbxAllRating.Text == String.Empty)
            {
                TxbxAllRating.Text = "-";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CbxYear.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DataGridUserCourses_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var items = DataGridUserCourses.CurrentItem as Course;

                if (items == null)
                {
                    return;
                }

                UserId = items.UserId;
                Id = items.Id;
                Category = items.Category;
                Title = items.Title;
                Description = items.Description;
                Evaluation = items.Evaluation ?? 0;
                Date = items.Date;
                Hyperlink = items.Hyperlink;
                FilePath = items.FileName;

                new FormViewCourseFull(UserId, Id, Lastname, Category, Title, Description, Date, Hyperlink, User, FilePath, Evaluation.Value).ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            DataGridUserCourses.ItemsSource = _courseRepository.GetCoursesByUserId(Convert.ToInt32(LblUserId.Content), Convert.ToInt16(CbxYear.SelectedItem)).ToList();

            string rating = _courseRepository.GetAllRating(Convert.ToInt32(LblUserId.Content), Convert.ToInt16(CbxYear.SelectedItem));

            if (rating == String.Empty)
            {
                TxbxAllRating.Text = "-";
            }
            else
            {
                TxbxAllRating.Text = rating;
            }

            new OtherRepository().SettingDataGridUsers(DataGridUserCourses);
        }

        SolidColorBrush orange = new SolidColorBrush(Colors.Orange);
        SolidColorBrush white = new SolidColorBrush(Colors.White);
        SolidColorBrush green = new SolidColorBrush(Colors.Green);

        private void DataGridUserCourses_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Course course = (Course) e.Row.DataContext;

            if (course.Evaluation == null)
            {
                e.Row.Background = orange;
            }
            if (course.Evaluation != null && course.DateEdit == null)
            {
                e.Row.Background = white;
            }
            if (course.Evaluation != null && course.DateEdit != null)
            {
                e.Row.Background = green;
            }
        }
    }
}
