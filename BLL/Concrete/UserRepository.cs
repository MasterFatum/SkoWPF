using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BLL.Entities;

namespace BLL.Concrete
{
    public class UserRepository
    {
        readonly SkoContext _skoContext = new SkoContext();

        public void AddUser(User user)
        {
            try
            {
                User userExists = _skoContext.Users.FirstOrDefault(em => em.Email == user.Email);

                if (userExists == null)
                {
                    _skoContext.Users.Add(user);

                    _skoContext.SaveChanges();

                    MessageBox.Show($"Пользователь {user.Email} успешно зарегистрирован в системе!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Пользователь с таким Email уже существует! Пожалуйста введите другой Email!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }

        public void DeleteUser(int id)
        {
            try
            {
                User user = _skoContext.Users.Find(id);

                if (user != null)
                {
                    _skoContext.Users.Remove(user);
                    _skoContext.SaveChanges();

                    MessageBox.Show("Пользователь успешно удалён!", "", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void EditUser(int id, string lastname, string firstname, string middlename, string position, string email)
        {
            try
            {
                var user = _skoContext.Users.Find(id);

                if (user != null)
                {
                    user.Lastname = lastname;
                    user.Firstname = firstname;
                    user.Middlename = middlename;
                    user.Position = position;
                    user.Email = email;

                    _skoContext.Users.AddOrUpdate(user);

                    _skoContext.SaveChanges();

                    MessageBox.Show("Ваш аккаунт успешно отредактирован!", "Редактирование пользователя", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void EditUser(int id, string lastname, string firstname, string middlename, string position, string email, string password, string privilege)
        {
            try
            {
                var user = _skoContext.Users.Find(id);

                if (user != null)
                {
                    user.Id = id;
                    user.Lastname = lastname;
                    user.Firstname = firstname;
                    user.Middlename = middlename;
                    user.Position = position;
                    user.Email = email;
                    user.Password = password;
                    user.Privilege = privilege;

                    _skoContext.Users.AddOrUpdate(user);

                    _skoContext.SaveChanges();

                    MessageBox.Show("Аккаунт успешно отредактирован!", "Редактирование пользователя", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public IEnumerable<User> GetAllUser()
        {
            try
            {
                IEnumerable<User> users = _skoContext.Users.ToList();

                return users;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        public IEnumerable<UserEvaluationSummary> GetAllUsersNameWithEvaluation(int year)
        {
            try
            {
                List<UserEvaluationSummary> users = _skoContext.Users.Select(x => new UserEvaluationSummary
                {
                    LastName = x.Lastname,
                    FirstName = x.Firstname,
                    MiddleName = x.Middlename,
                    EvaluationSum = x.Courses.Where(y => y.Year == year).Sum(e => e.Evaluation),
                    Year = year

                }).ToList();

                return users;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        public List<String> GetFioUsers()
        {
            try
            {
                List<User> users = _skoContext.Users.OrderBy(l => l.Lastname).ToList();

                List<String> userFio = new List<string>();

                foreach (var user in users)
                {
                    userFio.Add(String.Format($"{user.Lastname} {user.Firstname} {user.Middlename}"));
                }

                return userFio;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        public User ValidationUser(string username, string password)
        {
            try
            {
                User user = null;

                var findUser = _skoContext.Users.FirstOrDefault(u => u.Email == username);

                if (findUser != null)
                {
                    if (findUser.Password == password)
                    {
                        user = findUser;

                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        public Task<User> ValidationUserAsync(string username, string password)
        {
            return Task<User>.Factory.StartNew(() => ValidationUser(username, password));
        }

        public User ValidationAdmin(string username, string password)
        {
            try
            {
                User user = null;

                var findUser = _skoContext.Users.Where(p => p.Privilege == "Admin").FirstOrDefault(u => u.Email == username);

                if (findUser != null)
                {
                    if (findUser.Password == password)
                    {
                        user = findUser;
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        public Task<User> ValidationAdminAsync(string username, string password)
        {
            return Task<User>.Factory.StartNew(() => ValidationAdmin(username, password));
        }

        public void SokoDispose()
        {
            _skoContext.Dispose();
        }

        public string GetAllUsersCount()
        {
            try
            {
                return _skoContext.Users.Count().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        public int GetUserIdByFio(string lastname, string firstname, string middlename)
        {
            User userId = null;

            try
            { 
                userId = _skoContext.Users.Where(l => l.Lastname == lastname).Where(f => f.Firstname == firstname)
                    .FirstOrDefault(m => m.Middlename == middlename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (userId != null && userId.Id != 0)
            {
                return userId.Id;
            }
            return 0;
        }

        public bool UserIsOnline(int id)
        {
            try
            {
                var user = _skoContext.Users.Find(id);

                if (user != null)
                {
                    user.LastLoginDate =
                        String.Format($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");

                    user.IsOnline = true;

                    _skoContext.Users.AddOrUpdate(user);

                    _skoContext.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        public bool UserIsOffline(int id)
        {
            try
            {
                var user = _skoContext.Users.Find(id);

                if (user != null)
                {
                    user.IsOnline = false;

                    _skoContext.Users.AddOrUpdate(user);

                    _skoContext.SaveChanges();

                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }

        public IEnumerable<UsersInOnline> UsersInOnline()
        {
            try
            {
                IEnumerable<UsersInOnline> users = _skoContext.Users.Where(o => o.IsOnline)
                    .Select(x => new UsersInOnline
                    {
                        LastName = x.Lastname,
                        FirstName = x.Firstname,
                        MiddleName = x.Middlename,
                        Position = x.Position
                    })
                    .ToList();

                return users;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }
    }
    public class UserEvaluationSummary
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public int? EvaluationSum { get; set; }
        public int Year { get; set; }
    }

    public class UsersInOnline
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Position { get; set; }
    }
}
