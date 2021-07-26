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
    /// Логика взаимодействия для EmployeeUser.xaml
    /// </summary>
    /// 

    public partial class EmployeeUser : Page
    {
        public string password = "";
        public string photo;
        public EmployeeUser()
        {
            InitializeComponent();

            if (Manager.Photo == "")//Если фото сотрудника не выбранно в процецессе создания, то отбражается изображение из ресурсов
            {
                System.Windows.Media.Imaging.BitmapImage bit = new BitmapImage(new Uri("/Resources/worker.png", UriKind.Relative));
                img_photo.Source = bit;
            }
            else
                img_photo.Source = new BitmapImage(new Uri(Manager.Photo));//Если фото сотрудника выбранно в процецессе создания
            //Отображение данных о создаваемом пользователе
            if (Manager.Name != "" || Manager.Fam != "" || Manager.Otch != "" || Manager.Job != "")
            {
                txt_fio.Text = Manager.Name + " " + Manager.Fam + " " + Manager.Otch;
                txt_job.Text = Manager.Job;
            }
        }

        private void Back_MouseDown(object sender, MouseButtonEventArgs e)//Кнопка назад
        {
            Manager.NavigationFrame.GoBack();
        }

        private void SaveBTN(object sender, RoutedEventArgs e)//Кнопка сохранение нового сотрудника
        {

            if (String.IsNullOrEmpty(txt_pas.Text) || String.IsNullOrEmpty(txt_login.Text))
                MessageBox.Show("Заполните поля","Ошибка",MessageBoxButton.OK, MessageBoxImage.Error);
            else if(password.Length<8)
                MessageBox.Show("Пароль должен быть больше 8 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                #region Adress
                //Запись данных об адресе сотрудника
                Adress addAdress = new Adress
                {
                    country = Manager.Country,
                    city = Manager.City,
                    street = Manager.Street,
                    n_house = Convert.ToInt32(Manager.House),
                    n_kv = Convert.ToInt32(Manager.Kv)

                };
                //Добавление адреса сотрудника
                if (addAdress.id_adress == 0)
                {
                    DiplomEntities.GetContext().Adress.Add(addAdress);
                }
                #endregion

                if (Manager.Photo == "")
                    photo = "NULL";
                else
                    photo = Manager.Photo;

                #region Person
                Adress adress = new Adress();
                //Запись данных о сотруднике
                Person addPerson = new Person
                {
                    fio = Manager.Name + " " + Manager.Fam + " " + Manager.Otch,
                    job = Manager.Job,
                    seria = Convert.ToInt32(Manager.Seria),
                    number = Convert.ToInt32(Manager.Number),
                    date = Convert.ToDateTime(Manager.Date),
                    phone = Manager.Phone,
                    email = Manager.Email,                    
                    photo = photo,
                    id_adress = Convert.ToInt32(adress.id_adress)
                };
                //Добавление сотрудника
                if (addPerson.id_person == 0)
                {
                    DiplomEntities.GetContext().Person.Add(addPerson);
                }
                #endregion

                #region User
                Person person = new Person();
                //Запись данных пользователя
                Users addUsers = new Users
                {
                    login = txt_login.Text,
                    password = txt_pas.Text,
                    id_person = Convert.ToInt32(person.id_person)
                };
                //Регистрация пользователя
                if (addUsers.id_user == 0)
                {
                    DiplomEntities.GetContext().Users.Add(addUsers);
                }
                #endregion

                try
                {
                    DiplomEntities.GetContext().SaveChanges();
                    MessageBox.Show("Пользователь сохранен!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
           
        }

        private void RandomPassword_Click(object sender, RoutedEventArgs e)//Генерация рандомного пароля
        {
            if (passwordRandom.IsChecked == true)
            {               
                string[] arr = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "B", "C", "D", "F",
                "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "V", "W", "X", "Z",
                "b", "c", "d", "f", "g", "h", "j", "k", "m", "n", "p", "q", "r", "s", "t", "v",
                "w", "x", "z", "A", "E", "U", "Y", "a", "e", "i", "o", "u", "y" };
                Random rnd = new Random();
                for (int i = 0; i < 10; i = i + 1)
                {
                    password = password + arr[rnd.Next(0, 57)];
                }
                txt_pas.Text = password;
            }
            else
                txt_pas.Clear();
            
        }
        
    }
}
