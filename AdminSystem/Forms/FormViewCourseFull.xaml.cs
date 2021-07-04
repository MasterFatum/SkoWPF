using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Bll.Concrete;
using BLL.Concrete;
using Microsoft.Win32;

namespace AdminSystem.Forms
{

    public partial class FormViewCourseFull
    {
        CourseRepository _courseRepository = new CourseRepository();
        FtpRepository _ftpRepository = new FtpRepository();

        public string User { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string MyUrlHyperlink { get; set; }
        public string FileName { get; set; }


        public FormViewCourseFull(int userId, int id, string user, string category, string title, string description, string date, string hyperlink, string myUser, string fileName, int evaluation)
        {
            InitializeComponent();

            User = myUser;
            Id = id;
            UserId = userId;

            TxbxUser.Text = user;
            TxbxCategory.Text = category;
            TxbxTitle.Text = title;
            TxbxDescription.Text = description;
            CbxRating.Text = evaluation.ToString();
            TxbxDate.Text = date;
            MyUrlHyperlink = hyperlink;
            FileName = fileName;

            if (FileName == null)
            {
                BtnLocalMatherials.IsEnabled = false;
            }
            if (String.IsNullOrEmpty(MyUrlHyperlink))
            {
                Hyperlink.IsEnabled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TxBlSetRating.Text == " Сохранить")
            {
                MessageBoxResult result = MessageBox.Show("Баллы не были сохранены! Вы действительно хотите выйти?", "", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Close();
                }
            }
            if (TxBlSetRating.Text == " Назначить баллы")
            {
                Close();
            }
        }

        private void SetRating_Click(object sender, RoutedEventArgs e)
        {
            if (TxBlSetRating.Text == " Назначить баллы")
            {
                CbxRating.IsEnabled = true;
                TxBlSetRating.Text = " Сохранить";
            }
            else if (TxBlSetRating.Text == " Сохранить")
            {
                CbxRating.IsEnabled = false;
                TxBlSetRating.Text = " Назначить баллы";

                _courseRepository.SetRatingCourse(UserId, Id, Convert.ToByte(CbxRating.Text.Trim()), User);
            }
        }

        private void BtnDeleteEvaluation_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Удалить баллы данного пользователя?", "Удаление баллов", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _courseRepository.SetRatingCourseNull(UserId, Id);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (MyUrlHyperlink != String.Empty)
            {
                Process.Start(new ProcessStartInfo(MyUrlHyperlink));
                e.Handled = true;
            }
            else
            {
                MessageBox.Show("Материалы данного курса отсутсвуют!", "", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void BtnLocalMatherials_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog
            {
                Filter = @"Zip files (*.zip)|*.zip", 
                FileName = String.Format($"{TxbxUser.Text} {TxbxTitle.Text}")
            };


            if (saveFile.ShowDialog() == true)
            {
                string fileLocalPath = saveFile.FileName;
                Task task = new Task(() => _ftpRepository.DownloadFile(UserId.ToString(), fileLocalPath, FileName));
                task.Start();
            }
        }
    }
}
