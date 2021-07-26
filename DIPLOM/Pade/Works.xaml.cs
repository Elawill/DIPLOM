using DIPLOM.DataBase;
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

namespace DIPLOM.Pade
{
    /// <summary>
    /// Логика взаимодействия для Works.xaml
    /// </summary>
    public partial class Works : Page
    {
        public Works()
        {
            InitializeComponent();
            //Отображение сотрудников
            var info = DiplomEntities.GetContext().Person.ToList();
            ListWorks.ItemsSource = info;
        }

        private void ListWorks_SelectionChanged(object sender, SelectionChangedEventArgs e)//Выбор сотрудника
        {
            Person person = new Person();
            person = (sender as ListView).SelectedItem as Person;//Поиск данных по выбранному элементу
            string fio = person.fio;
            string[] t = fio.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);//Разделение фио 
            Manager.ID_PersonCheck = person.id_person;
            if (Manager.Form == "CreateCheck" )//Если выбранный пользовател нужен для создания проверки
                                              //и переход осуществлялся со стараницы создания проверки                         
                Manager.NavigationFrame.Navigate(new CteateCheck());
            
            else if(Manager.Form == "DocumentsPage")//Если выбранный пользовател нужен для создания повторной проверки
                                                    //и переход осуществлялся со стараницы вывода информации о пройденной проверке
            {                              
                Window.NavigationWindow main = new Window.NavigationWindow();
                Window.WorksWindow works = new Window.WorksWindow();
                works.Show();
                main.Close();
            }
            else//Если выбранный пользователь нужен для просмотра его полной информации или редактирования
            {
                //Сохранение его данных для передачи на форму
                Manager.Name = t[0];
                Manager.Fam = t[1];
                Manager.Otch = t[2];
                Manager.Job = person.job;
                Manager.Seria = person.seria.ToString();
                Manager.Number = person.number.ToString();
                Manager.Phone = person.phone;
                Manager.Email = person.email;
                Manager.Date = person.date.ToString();
                Manager.Photo = person.photo;
                Manager.Form = "Works";
                Manager.NavigationFrame.Navigate(new Employee());
            }
            
        }
    }
}
