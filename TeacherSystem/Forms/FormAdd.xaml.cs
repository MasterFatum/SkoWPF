using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Bll.Concrete;
using BLL.Concrete;
using BLL.Entities;
using MessageBox = System.Windows.MessageBox;


namespace UserSystem.FormsAddEducations
{
    public partial class FormAdd
    {
        readonly CourseRepository _courseRepository = new CourseRepository();
        readonly FtpRepository _ftpRepository = new FtpRepository();

        public string SelectedCategory { get; set; }
        public int UserIdDirectory { get; set; }
        public string FilePath { get; set; }
        public string FileNameGuid { get; set; }

        public FormAdd(int userId, string selectedCategory)
        {
            InitializeComponent();

            SelectedCategory = selectedCategory;
            TxbxSelectedCategory.Text = SelectedCategory;
            UserIdDirectory = userId;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TxbxTitle.Text = String.Empty;
            TxbxDescription.Text = String.Empty;
            TxbxHyperlink.Text = String.Empty;
            TxbxFilePath.Text = String.Empty;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TxbxTitle.Text != String.Empty && TxbxDescription.Text != String.Empty)
                {
                    if (TxbxFilePath.Text != String.Empty)
                    {
                        FileNameGuid = Guid.NewGuid().ToString();
                    }
                    else
                    {
                        FileNameGuid = null;
                    }

                    Course course = new Course
                    {
                        UserId = UserIdDirectory,
                        Category = SelectedCategory,
                        Title = TxbxTitle.Text.Trim(),
                        Description = TxbxDescription.Text.Trim(),
                        Date = String.Format($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}"),
                        Hyperlink = TxbxHyperlink.Text.Trim(),
                        FileName = FileNameGuid,
                        Year = DateTime.Now.Year
                    };

                    if (TxbxFilePath.Text != String.Empty)
                    {
                        if (!_ftpRepository.ExistDirectory(UserIdDirectory.ToString()))
                        {
                            _ftpRepository.CreateDirectory(UserIdDirectory.ToString());
                        }

                        Task task = new Task(() => _ftpRepository.UploadFile("/" + UserIdDirectory + "/", FilePath, FileNameGuid));
                        task.Start();
                    }
                    _courseRepository.AddCourse(course, TxbxTitle, TxbxDescription, TxbxHyperlink, TxbxFilePath);
                }
                else
                {
                    MessageBox.Show("Одно или несколько полей не заполнено!", "", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void BtnBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = @"(*.zip)|*.zip"
            };
            
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                long fileLength = new FileInfo(openFileDialog.FileName).Length;

                if (fileLength > 26214400)
                {
                    MessageBox.Show("Размер файла превышает допустимый! Размер файла не должен превышать 25 MB.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    FilePath = openFileDialog.FileName;
                    TxbxFilePath.Text = FilePath;
                }
            }
        }
    }
}
