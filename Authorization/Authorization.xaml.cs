using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BLL.Concrete;
using BLL.Entities;

namespace Authorization
{
    public partial class MainWindow
    {
        readonly UserRepository userRepository = new UserRepository();

        public MainWindow()
        {
            InitializeComponent();

            if (Properties.Settings.Default.IsSaveUser)
            {
                TxbxLogin.Text = Properties.Settings.Default.Username;
                TxbxPassword.Password = Properties.Settings.Default.Password;
                ChkBoxSaveUser.IsChecked = Properties.Settings.Default.IsSaveUser;
            }
        }
        
        private void BtnAuthorizeExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Registrations().ShowDialog();
        }

        private void BtnAuthorize_Click(object sender, RoutedEventArgs e)
        {
            LoginProgress();
            BtnAuthorize.IsEnabled = false;
        }

        public async void LoginProgress()
        {
            try
            {
                while (pb_userLoginProgress.Value != 100)
                {
                    pb_userLoginProgress.Value++;
                    await Task.Delay(10);
                }
                UserLogin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void UserLogin()
        {

            try
            {
                switch (((ComboBoxItem) CbxAuthorizeAs.SelectedItem).Content.ToString())
                {

                    //АВТОРИЗАЦИЯ ПОЛЬЗОВАТЕЛЯ
                    case "Преподаватель":
                        Task<User> userResult =
                            userRepository.ValidationUserAsync(TxbxLogin.Text.Trim(), TxbxPassword.Password);

                        User user = userResult.Result;


                        if (user != null)
                        {

                            if (ChkBoxSaveUser.IsChecked == true)
                            {
                                Properties.Settings.Default.Username = TxbxLogin.Text.Trim();
                                Properties.Settings.Default.Password = TxbxPassword.Password.Trim();
                                Properties.Settings.Default.IsSaveUser = true;

                                Properties.Settings.Default.Save();
                            }

                            if (ChkBoxSaveUser.IsChecked == false)
                            {
                                Properties.Settings.Default.Username = String.Empty;
                                Properties.Settings.Default.Password = String.Empty;
                                Properties.Settings.Default.IsSaveUser = false;

                                Properties.Settings.Default.Save();
                            }


                            Visibility = Visibility.Collapsed;
                            Visibility = Visibility.Hidden;
                            new TeacherSystem.MainWindow(user).ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("Неправильный логин или пароль!", "Ошибка", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }

                        break;

                    //АВТОРИЗАЦИЯ АДМИНИСТРАТОРА
                    case "Администратор":
                        Task<User> userAdmin =
                            userRepository.ValidationAdminAsync(TxbxLogin.Text.Trim(), TxbxPassword.Password);

                        User admin = userAdmin.Result;

                        if (admin != null)
                        {
                            if (ChkBoxSaveUser.IsChecked == true)
                            {
                                Properties.Settings.Default.Username = TxbxLogin.Text.Trim();
                                Properties.Settings.Default.Password = TxbxPassword.Password.Trim();
                                Properties.Settings.Default.IsSaveUser = true;

                                Properties.Settings.Default.Save();
                            }

                            if (ChkBoxSaveUser.IsChecked == false)
                            {
                                Properties.Settings.Default.Username = String.Empty;
                                Properties.Settings.Default.Password = String.Empty;
                                Properties.Settings.Default.IsSaveUser = false;

                                Properties.Settings.Default.Save();
                            }

                            Visibility = Visibility.Collapsed;
                            Visibility = Visibility.Hidden;
                            new AdminSystem.MainWindow(admin).ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("Неправильный логин или пароль!", "Ошибка", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            finally
            {
                BtnAuthorize.IsEnabled = true;
            }
        }

        private void AuthorizeForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel == false)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
