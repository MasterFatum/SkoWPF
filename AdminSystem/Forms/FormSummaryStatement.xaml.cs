using System;
using System.Windows;
using Bll.Concrete;
using BLL.Concrete;

namespace AdminSystem.Forms
{
    public partial class FormSummaryStatement
    {
        public FormSummaryStatement()
        {
            InitializeComponent();
        }

        readonly UserRepository _userRepository = new UserRepository();

        readonly CourseRepository _courseRepository = new CourseRepository();

        readonly OtherRepository _otherRepository = new OtherRepository();

        public String[] FullName { get; set; }
        public int UserId { get; set; }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CbxUsersSummaryStatement.ItemsSource = new UserRepository().GetFioUsers();
        }

        private void CbxYear_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataGridSummaryStatement.ItemsSource =
                _courseRepository.GetSummaryStatementByFio(Convert.ToInt16(CbxYear.SelectedItem), FullName[0], FullName[1], FullName[2]);

            _otherRepository.SettingDataGridSummaryStatement(DataGridSummaryStatement);

            LblSummary.Content = _courseRepository.GetAllRating(UserId, Convert.ToInt16(CbxYear.SelectedItem));
        }
        
        private void CbxUsersSummaryStatement_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (CbxUsersSummaryStatement.SelectedIndex != -1)
                {
                    CbxYear.IsEnabled = true;

                    String usernameFio = CbxUsersSummaryStatement.SelectedItem.ToString();

                    FullName = usernameFio.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    UserId = _userRepository.GetUserIdByFio(FullName[0], FullName[1], FullName[2]);

                    CbxYear.ItemsSource = _courseRepository.GetYears(UserId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
