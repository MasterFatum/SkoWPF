using System;
using System.Windows;
using System.Windows.Controls;
using Bll.Concrete;
using BLL.Concrete;

namespace AdminSystem.Forms
{
    public partial class FormSummaryStatementGeneral
    {
        readonly UserRepository _userRepository = new UserRepository();
        readonly CourseRepository _courseRepository = new CourseRepository();

        public FormSummaryStatementGeneral()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CbxYear.ItemsSource = _courseRepository.GetYears();
            CbxYear.SelectedIndex = 0;
        }

        private void BtnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            new CourseRepository().ExportToExcel(DataGridTable);
        }

        private void CbxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGridTable.ItemsSource = _userRepository.GetAllUsersNameWithEvaluation(Convert.ToInt16(CbxYear.SelectedItem));

            new OtherRepository().SettingDataGridSummaryStatementGeneral(DataGridTable);
        }
    }
}
