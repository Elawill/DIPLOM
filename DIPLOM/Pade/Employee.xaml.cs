using DIPLOM.DataBase;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class Employee : Page
    {
        public string photo = "";
        int x = 0;
        public Employee()
        {
            InitializeComponent();
            
            //Если данные о пользователе не пусты, они отобразятся на форме
            if(Manager.Name!=""|| Manager.Fam != "" || Manager.Otch != "" ||Manager.Job != ""|| Manager.Photo!=""||
                Manager.Phone != "" || Manager.Email != ""|| Manager.Seria != "" || Manager.Number != "" || Manager.Date != "")
            {
                txt_name.Text = Manager.Name;
                txt_fam.Text = Manager.Fam;
                txt_otch.Text = Manager.Otch;
                txt_job.Text = Manager.Job;
                txt_phone.Text = Manager.Phone;
                txt_email.Text = Manager.Email;
                txt_seria.Text = Manager.Seria;
                txt_number.Text = Manager.Number;
                txt_date.Text = Manager.Date;
                if(Manager.Photo!=null)//Если у пользователя есть фото
                    img_photo.Source = new BitmapImage(new Uri(Manager.Photo));
                else
                {
                    //Если фото нет, берется изображение из ресурсов
                    System.Windows.Media.Imaging.BitmapImage bit = new BitmapImage(new Uri("/Resources/worker.png", UriKind.Relative));
                    img_photo.Source = bit;
                }
            }
            if (Manager.Form != "")//Скрытие и показ элементов, если пользователь перешел на форму со станицы просмотра сотрудников
            {
                //next.Visibility= Visibility.Hidden;
                back.Visibility = Visibility.Visible;
                BNTUpdate.Visibility = Visibility.Visible;
                img_photo.IsEnabled = false;
                StackOne.IsEnabled = false;
                StackTwo.IsEnabled = false;                
            }

        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Проверка на заполненые поля
            if (String.IsNullOrEmpty(txt_name.Text) || String.IsNullOrEmpty(txt_fam.Text) || String.IsNullOrEmpty(txt_otch.Text) ||
                 String.IsNullOrEmpty(txt_job.Text) || String.IsNullOrEmpty(txt_phone.Text) || String.IsNullOrEmpty(txt_seria.Text)
                 || String.IsNullOrEmpty(txt_number.Text) || String.IsNullOrEmpty(txt_date.Text))
                MessageBox.Show("Заполните поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string email = txt_email.Text;                
                if (email != "")//Если введен email, то проверка на его корректность
                {
                    if (email.Contains("@") != true || email.Contains(".") != true)//Проверка на коректность Email
                        MessageBox.Show("Не коректный адрес почты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                //Сохранение данных для перехода и дальнейшего создания
                Manager.Name = txt_name.Text;
                Manager.Fam = txt_fam.Text;
                Manager.Otch = txt_otch.Text;
                Manager.Job = txt_job.Text;
                Manager.Phone = txt_phone.Text;
                Manager.Email = txt_email.Text;
                Manager.Seria = txt_seria.Text;
                Manager.Number = txt_number.Text;
                Manager.Date = txt_date.Text;
                Manager.Photo = photo;
                Manager.NavigationFrame.Navigate(new EmployeeAdress());
            }
        }

        private void BNTUpdate_Click(object sender, RoutedEventArgs e)//Изменение информации пользователя
        {
            //Разлокировать элементы
            next.Visibility = Visibility.Visible;
            back.Visibility = Visibility.Visible;
            GridZero.IsEnabled = true;
            img_photo.IsEnabled = true;
            StackOne.IsEnabled = true;
            StackTwo.IsEnabled = true;
            //Изменить данные при повторе нажатия кнопки
            if (x > 1)
            {
                Person person = null;
                //Измнение значений
                using (DiplomEntities db = new DiplomEntities())
                {
                    person = db.Person.Where(p => p.id_person == Manager.ID_person).FirstOrDefault();
                    person.fio = txt_name.Text + " " + txt_fam.Text + " " + txt_otch.Text;
                    person.job = txt_job.Text;
                    person.phone = txt_phone.Text;
                    person.email = txt_email.Text;
                    person.seria = Convert.ToInt32(txt_seria.Text);
                    person.number = Convert.ToInt32(txt_number.Text);
                    person.date = Convert.ToDateTime(txt_date.Text);
                    if (photo != "")
                        person.photo = photo;
                    db.SaveChanges();
                    Manager.Photo = person.photo;
                }

                MessageBox.Show("Пользователь изменен", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            x++;           
        }
        private void img_photo_MouseDown(object sender, MouseButtonEventArgs e)//Добавить фото
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
                        using (stream)
                        {
                            photo = ofd.FileName;
                            img_photo.Source = new BitmapImage(new Uri(photo));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void back_MouseDown(object sender, MouseButtonEventArgs e)//Кнопка назад
        {
            Manager.NavigationFrame.GoBack();
        }

        #region ВВОД
        private void txt_seria_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = Convert.ToChar(e.Text[0]);
            if (!Char.IsDigit(ch) && ch != 8)
                e.Handled = true;
        }

        private void txt_number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = Convert.ToChar(e.Text[0]);
            if (!Char.IsDigit(ch) && ch != 8)
                e.Handled = true;
        }

        private void txt_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = e.Text[0];
            if (!char.IsLetter(ch))
                e.Handled = true;
        }

        private void txt_fam_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = e.Text[0];
            if (!char.IsLetter(ch))
                e.Handled = true;
        }

        private void txt_otch_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = e.Text[0];
            if (!char.IsLetter(ch))
                e.Handled = true;
        }

        private void txt_job_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char ch = e.Text[0];
            if (!char.IsLetter(ch))
                e.Handled = true;
        }

       
    }
    #endregion

}

