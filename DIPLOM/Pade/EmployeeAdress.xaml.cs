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
    /// Логика взаимодействия для EmployeeAdress.xaml
    /// </summary>
    public partial class EmployeeAdress : Page
    {
        public int id, x=0;
        public EmployeeAdress()
        {
            InitializeComponent();
            if (Manager.Photo == "")//Если фото сотрудника не выбранно в процецессе создания, то отбражается изображение из ресурсов
            {
                System.Windows.Media.Imaging.BitmapImage bit = new BitmapImage(new Uri("/Resources/worker.png", UriKind.Relative));
                img_photo.Source = bit;
            }
            else
                img_photo.Source = new BitmapImage(new Uri(Manager.Photo));//Если фото сотрудника выбранно в процецессе создания
            //Если данные о пользователе не пусты, они отобразятся на форме
            if (Manager.Country != "" || Manager.City != "" || Manager.Street != "" 
                || Manager.House != "" || Manager.Kv != "")
            {
                txt_country.Text = Manager.Country;
                txt_city.Text = Manager.City;
                txt_street.Text = Manager.Street;
                txt_house.Text = Manager.House;
                txt_kv.Text = Manager.Kv;    
            }
            if (Manager.Form != "")// Скрытие и показ элементов, если пользователь перешел на форму для изменения адреса содника
            {
                next_img.Visibility = Visibility.Hidden;
                BNTUpdate.Visibility = Visibility.Visible;
                StackAdress.IsEnabled = false;
                Person person = null;
                Adress adress = null;
                //Отображенние нужных данных сотрудника 
                using (DiplomEntities db = new DiplomEntities())
                {
                    person = db.Person.Where(p => p.id_person == Manager.ID_person).FirstOrDefault();
                    id = person.id_adress;
                    adress = db.Adress.Where(p => p.id_adress == id).FirstOrDefault();
                    txt_country.Text= adress.country;
                    txt_city.Text= adress.city;
                    txt_street.Text=adress.street;
                    txt_house.Text=adress.n_house.ToString();
                    txt_kv.Text=adress.n_kv.ToString();
                    //img_photo.Source=new BitmapImage(new Uri(person.photo));

                }
            }
        }
        
        private void Back_MouseDown(object sender, MouseButtonEventArgs e)//Кнопка назад
        {
            Manager.NavigationFrame.GoBack();
        }

        private void Next_MouseDown(object sender, MouseButtonEventArgs e)//Кнопка вперед
        {
            if (String.IsNullOrEmpty(txt_country.Text) || String.IsNullOrEmpty(txt_city.Text) || String.IsNullOrEmpty(txt_street.Text) ||
                 String.IsNullOrEmpty(txt_house.Text))
                MessageBox.Show("Заполните поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                //Сохранение данных для перехода и дальнейшего создания
                Manager.Country = txt_country.Text;
                Manager.City = txt_city.Text;
                Manager.Street = txt_street.Text;
                Manager.House = txt_house.Text;
                Manager.Kv = txt_kv.Text;
                Manager.NavigationFrame.Navigate(new EmployeeUser());
            }

        }        

        /// <summary>
        /// ВВОД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region
        private void txt_country_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = e.Text[0];
            if (!char.IsLetter(ch))
                e.Handled = true;
        }

        private void txt_city_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = e.Text[0];
            if (!char.IsLetter(ch))
                e.Handled = true;
        }

        private void txt_street_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = e.Text[0];
            if (!char.IsLetter(ch))
                e.Handled = true;
        }

        private void txt_house_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = Convert.ToChar(e.Text[0]);
            if (!Char.IsDigit(ch) && ch != 8)
                e.Handled = true;
        }

        private void txt_kv_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = Convert.ToChar(e.Text[0]);
            if (!Char.IsDigit(ch) && ch != 8)
                e.Handled = true;
        }
        #endregion

        private void BNTUpdate_Click(object sender, RoutedEventArgs e)//Изменение адреса сотрудника
        {
            //Разблокировать форму при первом нажатии
            StackAdress.IsEnabled = true;
            //Изменить данные при повторном
            if (x > 1)
            {
                //Имзенение адреса сотрудника
                Adress adress = null;
                using (DiplomEntities db = new DiplomEntities())
                {
                    adress = db.Adress.Where(p => p.id_adress == id).FirstOrDefault();
                    adress.country = txt_country.Text;
                    adress.city = txt_city.Text;
                    adress.street = txt_street.Text;
                    adress.n_house = Convert.ToInt32(txt_house.Text);
                    adress.n_kv = Convert.ToInt32(txt_kv.Text);

                }
                MessageBox.Show("Адрес пользователя изменен", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            x++;
           
        }
    }
}
