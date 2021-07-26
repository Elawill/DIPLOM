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
using System.Data;
using System.Data.SqlClient;
using DIPLOM.DataBase;

namespace DIPLOM.PadeWork
{
    /// <summary>
    /// Логика взаимодействия для CarryingCheck.xaml
    /// </summary>
    public partial class CarryingCheck : Page
    {
        public string Server = Manager.Server;
        DataSet db;
        SqlDataAdapter sql;
        Expander expander;
        Button button;

        public string name_check, zap;
        public CarryingCheck()
        {
            InitializeComponent();
            ShowCheck();
        }

        #region Вывод информации
        public void ShowCheck()
        {
            zap = "with s as ( select Object.name as name_ob, [Check].date, [Check].id_check, Object.id_object, [Check].replay, Person.status_check, " +
                "Person.fio, [Check].id_person, Object.name + ' ' + CAST([Check].date as nvarchar) as name_check " +
                "from Person inner join [Check] on [Check].id_person = Person.id_person " +
                "inner join Object on Object.id_object =[Check].id_object where [Check].id_person =3)" +
                " select* from s ";
            //Нахождение проверок для ответсвенного
            if (Manager.StatusPerson == "Ответсвенный")
                zap = zap.Replace("where [Check].id_person =" + Manager.ID_person + ")",
                    "where [Check].id_person =" + Manager.ID_person + " and [Check].replay is not null)");
            using (SqlConnection pokl = new SqlConnection(Server))
            {
                pokl.Open();
                db = new DataSet();
                sql = new SqlDataAdapter(zap, pokl);
                sql.Fill(db);
            }

            for (int i = 0; i < db.Tables[0].Rows.Count; i++)//Вывод проверок
            {
                name_check = db.Tables[0].Rows[i]["name_check"].ToString();
                //Запись названия проверок
                expander = new Expander() { Header = name_check };
                stack.Children.Add(expander);
                //Возможность проверяющему или ответсвенному начинать провеку
                if ((db.Tables[0].Rows[i]["replay"].ToString() != "yes" && Manager.StatusPerson == "Проверяющий")|| Manager.StatusPerson == "Ответсвенный")
                {
                    //Если проверка имеет replay=yes у проверяющего, то ее нельзя начинать, возможен только просмотр
                    button = new Button() { Content = "Начать проверку" };
                    button.Width = 300;
                    button.HorizontalAlignment = HorizontalAlignment.Right;
                    button.Click += CheckBTN;
                    expander.Content = button;
                }
                button = new Button() { Content = "Начать проверку" };
                button.Width = 300;
                button.HorizontalAlignment = HorizontalAlignment.Right;
                button.Click += CheckBTN;
                expander.Content = button;

            }

        }
        #endregion


        #region Начать проверку
        private void CheckBTN(object sender, RoutedEventArgs e)
        {
            var parent = (sender as Button).Parent as Expander;
            string nameParent = parent.Header.ToString();//Имя выбранной проверки
            zap += "where name_check='" + nameParent + "'";
            //Поиск информации о выбранной проверке 
            using (SqlConnection pokl = new SqlConnection(Server))
            {
                pokl.Open();
                db = new DataSet();
                sql = new SqlDataAdapter(zap, pokl);
                sql.Fill(db);
            }
            int id_check = Convert.ToInt32(db.Tables[0].Rows[0]["id_check"]);
            Manager.ID_Check = id_check;//Сохранение ID проверки
            Manager.ID_Object = Convert.ToInt32(db.Tables[0].Rows[0]["id_object"]);//Сохранение ID объекта, для возможной повторной проверки
            Check check = null;
            DateTime date = DateTime.Now;
            //Изменение проверки, запись даты начала проведения и смена статуса
            using (DiplomEntities d = new DiplomEntities())
            {
                check = d.Check.Where(p => p.id_check == id_check).FirstOrDefault();
                check.dateStart = date;
                check.id_status = 2;
                d.SaveChanges();
            }
            Manager.WorksFrame.Navigate(new Carrying());

        }
        #endregion

    }
}
