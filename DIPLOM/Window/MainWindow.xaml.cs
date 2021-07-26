using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DIPLOM.DataBase;

namespace DIPLOM
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();          
        }

        private void ExitBTN(object sender, RoutedEventArgs e)
        {
            string login = txt_login.Text;
            string pas = txt_pas.Password;
            int id = 0;
            Users users = null;
            //Подключение к БД, проверка зарегистрированн ли данный пользователь
            using (DiplomEntities db = new DiplomEntities())
            {
                users = db.Users.Where(p => p.password == pas && p.login == login).FirstOrDefault();
            }
            if (users != null)//Пользователь есть в системе
            {
                id = users.id_person;
                Manager.ID_person = id;
                //Проверка статуса пользователя для перехода на нужную страницу
                var person = DiplomEntities.GetContext().Person.Where(p => p.id_person == id).FirstOrDefault();                
                if (person.job == "Организатор")//Переход на страницу организатора
                {
                    Window.NavigationWindow navigation = new Window.NavigationWindow();
                    navigation.Show();
                }
                //Переход на страницу с проведением проверок
                else if(person.status_check.Trim()== "Проверяющий" || person.status_check.Trim() == "Ответсвенный" && person.job != "Организатор")
                {
                    Manager.StatusPerson = person.status_check.Trim();
                    Window.WorksWindow works = new Window.WorksWindow();
                    works.Show();
                }
                Application.Current.MainWindow.Close();

            }
            else
                MessageBox.Show("Не верный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
