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
    /// Логика взаимодействия для CreateObject.xaml
    /// </summary>
    public partial class CreateObject : Page
    {
        public CreateObject()
        {
            InitializeComponent();            
        }

        private void BTNSave(object sender, RoutedEventArgs e)
        {
            //Проверка на пусные поля
            if (String.IsNullOrEmpty(txt_name.Text) || String.IsNullOrEmpty(txt_face.Text) || String.IsNullOrEmpty(txt_phone.Text) || 
                String.IsNullOrEmpty(txt_email.Text) || String.IsNullOrEmpty(txt_vid.Text)  ||
                String.IsNullOrEmpty(txt_city.Text) || String.IsNullOrEmpty(txt_street.Text) || String.IsNullOrEmpty(txt_house.Text))
                MessageBox.Show("Заполните поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string name = txt_name.Text;
                string face = txt_face.Text;
               
                string city = txt_city.Text;
                string street = txt_street.Text;
                int house = Convert.ToInt32(txt_house.Text);
                string phone = txt_phone.Text;
                string email = txt_email.Text;
                string vid = txt_vid.Text;

                if (email.Contains("@") != true || email.Contains(".") != true)//Проверка на коректность Email
                    MessageBox.Show("Не коректный адрес почты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else 
                {
                    //Запись данных об адресе объекта
                    Adress addAdress = new Adress
                    {
                        
                        city = city,
                        street = street,
                        n_house = house
                    };
                    //Добавление адреса объекта
                    if (addAdress.id_adress == 0)
                    {
                        DiplomEntities.GetContext().Adress.Add(addAdress);
                    }
                    Adress adress = new Adress();

                    DataBase.Object ob = new DataBase.Object();
                    ob.id_adress = adress.id_adress;
                    int id = Convert.ToInt32(ob.id_adress);
                    //Запись данных объекта
                    DataBase.Object addObject = new DataBase.Object
                    {
                        name = name,
                        person = face,
                        id_adress = id,
                        phone = phone,
                        email = email,
                        vid_production = vid
                    };
                    //Добавление объекта
                    if (addObject.id_object == 0)
                    {
                        DiplomEntities.GetContext().Object.Add(addObject);
                    }
                    //Сохранение
                    try
                    {
                        DiplomEntities.GetContext().SaveChanges();
                        MessageBox.Show("Объект сохранен!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void txt_phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = Convert.ToChar(e.Text[0]);
            if (!Char.IsDigit(ch) && ch != 8)
                e.Handled = true;
        }

        private void txt_house_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = Convert.ToChar(e.Text[0]);
            if (!Char.IsDigit(ch) && ch != 8)
                e.Handled = true;
        }

        private void txt_face_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = e.Text[0];
            if (!char.IsLetter(ch))
                e.Handled = true;
        }

        private void txt_vid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = e.Text[0];
            if (!char.IsLetter(ch))
                e.Handled = true;
        }

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

        private void txt_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = e.Text[0];
            if (!char.IsLetter(ch))
                e.Handled = true;
        }
    }
}
