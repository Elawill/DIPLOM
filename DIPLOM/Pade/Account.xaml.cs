using DIPLOM.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.IO;

namespace DIPLOM.Pade
{
    /// <summary>
    /// Логика взаимодействия для Account.xaml
    /// </summary>
    public partial class Account : Page
    {
        public string photo="";
        public Account()
        {
            InitializeComponent();
            ShowInformation();
        }
        public void ShowInformation()
        {
            Person person = null;
            //Поиск информации о пользователе
            using (DiplomEntities db = new DiplomEntities())
            {
                person = db.Person.Where(p => p.id_person == Manager.ID_person).FirstOrDefault();
            }
            txt_fio.Text = person.fio;
            txt_job.Text = person.job;
            txt_phone.Text = person.phone;
            txt_email.Text = person.email;            
            img_photo.Source =new BitmapImage(new Uri(person.photo));
        }
 
        private void BNTUpdate(object sender, RoutedEventArgs e)
        {
            string email = txt_email.Text;
            string phone = txt_phone.Text;

            if (email.Contains("@") != true || email.Contains(".") != true)//Проверка на коректность Email
                MessageBox.Show("Не коректный адрес почты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);        
            else 
            {
                //Обновление информации о пользователе
                Person person = null;
                using (DiplomEntities db = new DiplomEntities())
                {
                    person = db.Person.Where(p => p.id_person == Manager.ID_person).FirstOrDefault();                   
                    person.phone = phone;
                    person.email = email;
                    if (photo != "")
                        person.photo = photo;
                    db.SaveChanges();
                }
                MessageBox.Show("Информация обновлена!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void img_photo_MouseDown(object sender, MouseButtonEventArgs e)//Открытие фото
        {
            Stream stream = null;
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "(*.jpg)|*.jpg";            
            if (ofd.ShowDialog() != null)
            {
                try
                {
                    if ((stream = ofd.OpenFile()) != null)
                    {
                        photo = ofd.FileName;
                        img_photo.Source = new BitmapImage(new Uri(photo));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"",MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
