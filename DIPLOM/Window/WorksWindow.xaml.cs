using DIPLOM.Pade;
using DIPLOM.PadeWork;
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
    /// Логика взаимодействия для WorksWindow.xaml
    /// </summary>
    public partial class WorksWindow
    {
        public WorksWindow()
        {
            InitializeComponent();
            //if (Manager.ID_PersonCheck != 0)//если выбран ответсвенный, то переход к результат проверки
                WokrsFrame.Navigate(new CarryingCheck());
            //else
            //    WokrsFrame.Navigate(new CarryingCheck());//Просмотр и выбор проверки
            Manager.WorksFrame = WokrsFrame;
        }

        private void AccountBNT(object sender, RoutedEventArgs e)
        {
            WokrsFrame.Navigate(new Account());
            Manager.WorksFrame = WokrsFrame;
        }

        private void ExitBNT(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void CheckBNT(object sender, RoutedEventArgs e)
        {
            WokrsFrame.Navigate(new CarryingCheck());
            Manager.WorksFrame = WokrsFrame;
        }
    }
}
