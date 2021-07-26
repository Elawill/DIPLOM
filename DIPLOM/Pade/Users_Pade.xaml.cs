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
    /// Логика взаимодействия для Users_Pade.xaml
    /// </summary>
    public partial class Users_Pade : Page
    {
        public Users_Pade()
        {
            InitializeComponent();
            
        }

        private void img_photo_MouseDown(object sender, MouseButtonEventArgs e)
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
                            
                            img_photo.Source = new BitmapImage(new Uri(ofd.FileName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void next_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StackOne.IsEnabled = false;
            StackTwo.IsEnabled = false;
        }
    }
}
