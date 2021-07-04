using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data.Entity.Migrations;
using System.Windows.Controls;
using BLL;
using BLL.Entities;
using DataGrid = System.Windows.Controls.DataGrid;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace Bll.Concrete
{
    public class CourseRepository
    {
        readonly SkoContext _skoContext = new SkoContext();

        public void AddCourse(Course course, TextBox title, TextBox description, TextBox hyperlink, TextBox dataFileName)
        {
            try
            {
                _skoContext.Courses.Add(course);
                _skoContext.SaveChanges();
                MessageBox.Show("Запись успешно добавлена!", "Добавление записи", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                title.Text = String.Empty;
                description.Text = String.Empty;
                hyperlink.Text = String.Empty;
                dataFileName.Text = String.Empty;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void DeleteCourse(int id, int userId)
        {
            try
            {
                Course course = _skoContext.Courses.Where(u => u.UserId == userId).FirstOrDefault(c => c.Id == id);

                if (course != null)
                {
                    _skoContext.Courses.Remove(course);
                    _skoContext.SaveChanges();
                    MessageBox.Show("Запись успешно удалена!", "Удаление записи", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void EditCourse(Course course)
        {
            try
            {
                Course courseEdit = _skoContext.Courses.Where(u => u.UserId == course.UserId)
                    .FirstOrDefault(c => c.Id == course.Id);

                if (courseEdit != null)
                {
                    courseEdit.Title = course.Title;
                    courseEdit.Description = course.Description;
                    courseEdit.Hyperlink = course.Hyperlink;
                    courseEdit.FileName = course.FileName;

                    if (courseEdit.Evaluation != null)
                    {
                        courseEdit.DateEdit =
                            String.Format($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
                    }

                    _skoContext.Courses.AddOrUpdate(courseEdit);
                    _skoContext.SaveChanges();

                    MessageBox.Show("Запись успешно отредактирована!", "Редактирование записи", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public IEnumerable<Course> GetCoursesByUserId(int userId, int year)
        {
            try
            {
                IQueryable<Course> courses = new SkoContext().Courses.Where(u => u.UserId == userId).Where(y => y.Year == year);

                return courses.ToList();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }

        public IEnumerable<Course> GetCoursesByCategory(int userId, string category, int year)
        {
            IQueryable<Course> courses = null;

            try
            {
                courses = _skoContext.Courses.Where(u => u.UserId == userId).Where(c => c.Category == category).Where(y => y.Year == year);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (courses != null)
            {
                return courses.ToList();
            }
            else
            {
                MessageBox.Show("Записи данной категории отсутствуют!", "", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            return null;
        }

        public IEnumerable<Course> GetCoursesByFio(string lastname, string firstname, string middlename, string category)
        {
            try
            {
                User userId = _skoContext.Users.OrderBy(l => l.Lastname).Where(l => l.Lastname == lastname).Where(f => f.Firstname == firstname)
                    .FirstOrDefault(m => m.Middlename == middlename);

                IEnumerable<Course> courses = _skoContext.Courses.Where(u => u.UserId == userId.Id)
                    .Where(c => c.Category == category);

                return courses.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        public IEnumerable<Course> GetCoursesByFio(string lastname, string firstname, string middlename)
        {
            try
            {
                User userId = _skoContext.Users.Where(l => l.Lastname == lastname).Where(f => f.Firstname == firstname)
                    .FirstOrDefault(m => m.Middlename == middlename);

                IEnumerable<Course> courses = _skoContext.Courses.Where(u => u.UserId == userId.Id);

                return courses.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        public void SetRatingCourse(int userId, int id, int rating, string evaluating)
        {
            try
            {
                Course course = _skoContext.Courses.Where(u => u.UserId == userId).FirstOrDefault(c => c.Id == id);

                if (course != null)
                {
                    course.Evaluation = rating;
                    course.Evaluating = evaluating;

                    _skoContext.Courses.AddOrUpdate(course);
                    _skoContext.SaveChanges();

                    MessageBox.Show("Баллы успешно назначены!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SetRatingCourseNull(int userId, int id)
        {
            try
            {
                Course course = _skoContext.Courses.Where(u => u.UserId == userId).FirstOrDefault(c => c.Id == id);

                if (course != null)
                {
                    course.Evaluation = null;
                    course.Evaluating = null;

                    _skoContext.Courses.AddOrUpdate(course);
                    _skoContext.SaveChanges();

                    MessageBox.Show("Баллы успешно удалены!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string GetAllRating(int userId, int year)
        {
            try
            {
                return _skoContext.Courses.Where(x => x.UserId == userId).Where(y => y.Year == year).Sum(r => r.Evaluation).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        public List<int> GetYears()
        {
            try
            {
                var years = _skoContext.Courses.Select(y => y.Year).Distinct().OrderByDescending(y => y).ToList();

                return years;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        public List<int> GetYears(int userId)
        {
            try
            {
                var years = _skoContext.Courses.Where(u => u.UserId == userId).Select(y => y.Year).Distinct().OrderByDescending(y => y).ToList();

                return years;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }
        public string GetRatingByYear(int userId, int year)
        {
            try
            {
                return _skoContext.Courses.Where(x => x.UserId == userId).Where(y => y.Year == year).Sum(r => r.Evaluation).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        public IEnumerable<Course> GetSummaryStatementByFio(int year, string lastname, string firstname, string middlename)
        {
            try
            {
                User userId = _skoContext.Users.Where(l => l.Lastname == lastname).Where(f => f.Firstname == firstname)
                    .FirstOrDefault(m => m.Middlename == middlename);

                var summary = _skoContext.Courses.Where(u => u.UserId == userId.Id).Where(y => y.Year == year).GroupBy(c => c.Category).Select(c => new
                {
                    category = c.Key,
                    evaluation = c.Sum(e => e.Evaluation)
                }).AsEnumerable().Select(an => new Course { Category = an.category, Evaluation = an.evaluation });

                return summary.ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        public void ExportToExcel(DataGrid dataGrid)
        {
            try
            {
                Excel.Application excel = new Excel.Application();
                excel.Visible = true;
                Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
                Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

                for (int j = 0; j < dataGrid.Columns.Count; j++)
                {
                    Range myRange = (Range)sheet1.Cells[1, j + 1];
                    sheet1.Cells[1, j + 1].Font.Bold = true;
                    sheet1.Columns[j + 1].ColumnWidth = 15;
                    myRange.Value2 = dataGrid.Columns[j].Header;
                }
                for (int i = 0; i < dataGrid.Columns.Count; i++)
                {
                    for (int j = 0; j < dataGrid.Items.Count; j++)
                    {
                        var b = dataGrid.Columns[i].GetCellContent(dataGrid.Items[j]) as TextBlock;
                        Range myRange = (Range)sheet1.Cells[j + 2, i + 1];

                        if (b != null) myRange.Value2 = b.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
