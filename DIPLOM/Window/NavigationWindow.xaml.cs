using DIPLOM.Pade;
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
using System.Windows.Shapes;

namespace DIPLOM.Window
{
    /// <summary>
    /// Логика взаимодействия для NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow
    {
        public NavigationWindow()
        {
            InitializeComponent();
            //Переход на страницу выбора и просмотра сотрудников,если требуется создание повторной проверки
            if (Manager.WindowDoc == "DocumentsPage")
                NavigationFrame.Navigate(new Works());
            else
                NavigationFrame.Navigate(new CteateCheck());//Переход на личный кабинет в остальных случаях

            Manager.NavigationFrame = NavigationFrame;
        }

        private void Account_MouseDown(object sender, MouseButtonEventArgs e)//Переход на личный кабинет
        {
            NavigationFrame.Navigate(new Account());
            Manager.NavigationFrame = NavigationFrame;
        }

        private void Exit_MouseDown(object sender, MouseButtonEventArgs e)//Выход
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void Worker_MouseDown(object sender, MouseButtonEventArgs e)//Переход на страницу выбора и просмотра сотрудников
        {
            NavigationFrame.Navigate(new Works());
            Manager.NavigationFrame = NavigationFrame;
        }

        private void Employee_MouseDown(object sender, MouseButtonEventArgs e)//Переход на страницу добавления сотрудников
        {
            NavigationFrame.Navigate(new Employee());
            Manager.NavigationFrame = NavigationFrame;
        }
        private void Object_MouseDown(object sender, MouseButtonEventArgs e)//Переход на страницу добавления объекта
        {
            NavigationFrame.Navigate(new CreateObject());
            Manager.NavigationFrame = NavigationFrame;
        }

        private void CteateCheck_MouseDown(object sender, MouseButtonEventArgs e)//Переход на страницу добавления проверки
        {
            NavigationFrame.Navigate(new CteateCheck());
            Manager.NavigationFrame = NavigationFrame;
        }

        private void Check_MouseDown(object sender, MouseButtonEventArgs e)//Переход на страницу просмотра проверки
        {
            NavigationFrame.Navigate(new CheckShow());
            Manager.NavigationFrame = NavigationFrame;
        }
    }
}
