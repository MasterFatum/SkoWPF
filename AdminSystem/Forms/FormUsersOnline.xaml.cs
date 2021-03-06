using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BLL.Concrete;

namespace AdminSystem.Forms
{
    public partial class FormUsersOnline 
    {
        public FormUsersOnline()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<UsersInOnline> users = new UserRepository().UsersInOnline();

                var usersInOnline = users as UsersInOnline[] ?? users.ToArray();

                LblUsersCount.Content = usersInOnline.Length;

                foreach (var user in usersInOnline)
                {
                    LbUserOnline.Items.Add($"{user.LastName} {user.FirstName} {user.MiddleName} ({user.Position})");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
