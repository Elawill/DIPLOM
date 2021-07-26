using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using Exel= Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.InfoPath.Xml;
using DIPLOM.DataBase;

namespace DIPLOM.PadeWork
{
    /// <summary>
    /// Логика взаимодействия для DocumentsPage.xaml
    /// </summary>
    public partial class DocumentsPage : Page
    {
        public string Server = Manager.Server;
        DataSet ds, ds1, ds2;
        SqlDataAdapter sql;
        public double n, n_no;
        double persent = 0;
        public string zap, vid;

        public DocumentsPage()
        {
            InitializeComponent();
            //txt_name.Text = Manager.CheckName;
            //ShowInfo();
            //отображение информации если выбран ответсвенный
            //if (Manager.ID_PersonCheck != 0)
            //{
            //    var info1 = DiplomEntities.GetContext().Person.Where(p => p.id_person == Manager.ID_PersonCheck).FirstOrDefault();
            //    txt_person_check.Text = info1.fio;
            //}
        }

        private void CheckBTN_Click(object sender, RoutedEventArgs e)//создание повторной проверки
        {
            checkGrid.Visibility = Visibility.Visible;
        }

        private void BNTperson_Clik(object sender, RoutedEventArgs e)//Сохранение данных и переход для выбора сотрудника
        {
            if (txt_date.Text != null)
                Manager.DateChreck = txt_date.Text;
            Manager.Form = "DocumentsPage";
            Manager.WindowDoc = "DocumentsPage";
            Window.NavigationWindow main = new Window.NavigationWindow();                      
            Window.WorksWindow works = new Window.WorksWindow();
            main.Show();
            works.Close();
        }


        #region Создание Документа
        private void wordBTN_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\Word\\";
            var WordApp = new Word.Application();
            WordApp.Visible = false;
            ////путь к шаблону
            var Worddoc = WordApp.Documents.Open(path+"1.docx");
            Random rnd = new Random();

            //Нахождение информации об организации
            var or = DiplomEntities.GetContext().Organization.FirstOrDefault();
            int id_personOr = or.id_person;
            var per = DiplomEntities.GetContext().Person.Where(p => p.id_person == id_personOr).FirstOrDefault();

            //Нахождение нужной информации о проверке
            string zp1 = "select Object.name, Object.person, Object.vid_production, [Check].vid, [Check].dateStart," +
                " Person.fio, Person.job, Person.id_person, Adress.street, Adress.n_house, Adress.city " +
                "from Object inner join [Check] on [Check].id_object = Object.id_object " +
                "join Adress on Adress.id_adress = Object.id_adress join Person on Person.id_person =[Check].id_person " +
                "where Object.id_object ="+Manager.ID_Object;
            using (SqlConnection pokl = new SqlConnection(Server))
            {
                pokl.Open();
                ds2 = new DataSet();
                sql = new SqlDataAdapter(zp1, pokl);
                sql.Fill(ds2);
            }
            DateTime date;
            string date1 = "";
            for(int i=0;i< ds2.Tables[0].Rows.Count; i++)//Цикл проверяющий информацию по каждой проверке
            {

                if (i > 0)//Если были повторный проверки, то запись проверяющего
                {
                    Repwo("{{fio_otvet} {job_otvet}}",", "+ ds2.Tables[0].Rows[i]["fio"].ToString()+",  "+ ds2.Tables[0].Rows[i]["job"].ToString(), Worddoc);
                }
                date =Convert.ToDateTime( ds2.Tables[0].Rows[i]["dateStart"]);//Запись последней даты
                date1 = date.ToLongDateString();
            }
            
            string[] d = date1.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);//Разделение даты

            //заполнение документа
            Repwo("{rnd}", (1000 + rnd.Next(100)).ToString(), Worddoc);
            Repwo("{organization}", or.name, Worddoc);
            Repwo("{director_organization}", per.fio, Worddoc);
            Repwo("{day}", d[0], Worddoc);
            Repwo("{month}", d[1], Worddoc);
            Repwo("{year}", d[2], Worddoc);
            Repwo("{object}", ds2.Tables[0].Rows[0]["name"].ToString(), Worddoc);
            Repwo("{vid_object}", ds2.Tables[0].Rows[0]["vid_production"].ToString(), Worddoc);
            Repwo("{director_object}", ds2.Tables[0].Rows[0]["person"].ToString(), Worddoc);
            Repwo("{address_object}","ул. "+ ds2.Tables[0].Rows[0]["street"].ToString() +", "+
                ds2.Tables[0].Rows[0]["n_house"].ToString()+", "+ ds2.Tables[0].Rows[0]["city"].ToString(), Worddoc);
            Repwo("{vid_check}", ds2.Tables[0].Rows[0]["vid"].ToString(), Worddoc);
            Repwo("{fio_prover}", ds2.Tables[0].Rows[0]["fio"].ToString(), Worddoc);
            Repwo("{job_prover}", ds2.Tables[0].Rows[0]["job"].ToString(), Worddoc);

            Worddoc.SaveAs2(path + ds2.Tables[0].Rows[0]["name"].ToString()+" "+DateTime.Now+ ".docx");
            WordApp.Visible = true;
        }
        
        private void Repwo(string subToReplace, string text, Word.Document worddoc)//Метод записывающий информацию в документ
        {
            var range = worddoc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: subToReplace, ReplaceWith: text);
        }
        #endregion


        #region Создание повторной проверки
        private void BNTSave_Clik(object sender, RoutedEventArgs e)
        {
            if (Manager.ID_PersonCheck != Manager.ID_person)
            {
                Check upCkek = null;
                Person upPerson = null;
                //Изнение данных начальной проверки/ сообщение о том, что по ней проводиться повторная 
                using (DiplomEntities db = new DiplomEntities())
                {
                    upCkek = db.Check.Where(p => p.id_check == Manager.ID_Check).FirstOrDefault();
                    upCkek.replay = "yes";
                    vid = upCkek.vid;
                    db.SaveChanges();
                }

                //Создание повторной проверки
                Check addCheck = new Check
                {
                    id_person = Manager.ID_PersonCheck,
                    id_object = Manager.ID_Object,
                    id_status = 2,
                    vid = vid,
                    date = Convert.ToDateTime(txt_date.SelectedDate.Value)
                };
                //Добавление проверки
                if (addCheck.id_check == 0)
                {
                    DiplomEntities.GetContext().Check.Add(addCheck);
                }
                //Изменение данных выбранного пользователя, чтобы он смог заходить под ответсвенным
                using (DiplomEntities db = new DiplomEntities())
                {
                    upPerson = db.Person.Where(p => p.id_person == Manager.ID_PersonCheck).FirstOrDefault();
                    upPerson.status_check = "Ответсвенный";
                    db.SaveChanges();
                }
                //Сохранение изменений
                try
                {
                    DiplomEntities.GetContext().SaveChanges();
                    MessageBox.Show("Повторная проверка назначена на " + txt_date.Text + " !", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    Manager.WorksFrame.Navigate(new DocumentsPage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
                MessageBox.Show("Вы не можете выбрать себя!", "", MessageBoxButton.OK, MessageBoxImage.Error);            
        }
        #endregion


        private void txt_date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)//Выбор даты
        {
            if (txt_date.Text != "")
            {
                DateTime now = DateTime.Now;
                DateTime date = txt_date.SelectedDate.Value;
                int rez = DateTime.Compare(date, now);
                if (rez < 0)
                {
                    MessageBox.Show("Эта дата уже прошла!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    txt_date.Text = "";
                }
            }

        }

        #region Создание Exel таблицы
        private void ExelBTN_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\Exel\\";
            var appExel = new Exel.Application();
            var xlWB = appExel.Workbooks.Open(path + "1.xlsx");//Открытие шаблона
            appExel.Visible = false;

            Exel.Worksheet xlSht = appExel.ActiveSheet;//Страница таблицы
            string new_cat = "", old_cat = "";
            Exel.Range range, rangeBorder;//Рабочая область таблицы
            xlSht.Cells[1, "A"] = txt_name.Text;//Запись в таблицу названия проверки

            for (int i = 0; i < n; i++)//Запись в таблицу
            {
                new_cat = ds.Tables[0].Rows[i]["category"].ToString();
                if (i < n)
                {
                    if (new_cat != old_cat)//Запись в таблицу только уникальных категорий
                    {
                        xlSht.Cells[3 + i, "A"] = ds.Tables[0].Rows[i]["category"].ToString();//Запись категории в таблицу на 1 столбец, начиная с 3 строки                       
                    }
                    else//Объединение строк по одной категорий
                    {
                        range = (Exel.Range)xlSht.get_Range("A3:A" + i).Cells;
                        range.Merge(Type.Missing);
                    }

                    //Запись данных под нужными столбцами
                    xlSht.Cells[3 + i, "B"] = ds.Tables[0].Rows[i]["question"].ToString();
                    xlSht.Cells[3 + i, "C"] = ds.Tables[0].Rows[i]["answer"].ToString();
                    xlSht.Cells[3 + i, "D"] = ds.Tables[0].Rows[i]["comment"].ToString();

                    rangeBorder = (Exel.Range)xlSht.get_Range("A3" + i, "D3" + i).Cells;//диапазон по которму будет создаваться рамка

                    //Создание рамки ячеек ко всех сторон
                    rangeBorder.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous; // внутренние вертикальные
                    rangeBorder.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous; // внутренние горизонтальные            
                    rangeBorder.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous; // верхняя внешняя
                    rangeBorder.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous; // правая внешняя
                    rangeBorder.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous; // левая внешняя
                    rangeBorder.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                }
                old_cat = ds.Tables[0].Rows[i]["category"].ToString();
            }

            xlSht.SaveAs(path+ Manager.CheckName+" "+ DateTime.Now.ToShortDateString()+".xlsx");//Сохранение таблицы 
            appExel.Visible = true;

        }
        #endregion

        #region Вывод информации и расчет
        public void ShowInfo()
        {
            zap = "with s as( select ca.name as category, q.name as question, q.answer,n.comment, c.id_check " +
                 "from[Check] c, Question q, Category ca, Narushenia n " +
                 "where ca.id_category = q.id_category and c.id_check = q.id_check and n.id_question = q.id_question " +
                 "union " +
                 "select ca.name as category, q.name as question, q.answer, null as comment, c.id_check " +
                 "from[Check] c, Question q, Category ca where ca.id_category = q.id_category and c.id_check = q.id_check) " +
                 "select* from s where id_check =5";// + Manager.ID_Check;
            using (SqlConnection pokl = new SqlConnection(Server))
            {
                pokl.Open();
                ds = new DataSet();
                sql = new SqlDataAdapter(zap, pokl);
                sql.Fill(ds);
            }
            n = Convert.ToDouble(ds.Tables[0].Rows.Count);//общее кол-во вопросов

            zap += "and answer = 'Нет'";
            using (SqlConnection pokl = new SqlConnection(Server))
            {
                pokl.Open();
                ds1 = new DataSet();
                sql = new SqlDataAdapter(zap, pokl);
                sql.Fill(ds1);
            }
            n_no = Convert.ToDouble(ds1.Tables[0].Rows.Count);//Кол-во вопросов с нарушениями
            //Процентный расчет прохождения проверки
            persent = n_no / n * 100;
            persent = Math.Round(persent, 0);
            persent = 100 - persent;
            txt_persent.Text = persent.ToString() + "%";
            if (persent == 100)
            {
                checkBTN.Visibility = Visibility.Hidden;
                //wordBTN.Visibility = Visibility.Visible;
                ////Изменение статуса проверки, проверка завершена
                //Check upCkek = null;
                //using (DiplomEntities db = new DiplomEntities())
                //{
                //    upCkek = db.Check.Where(p => p.id_check == Manager.ID_Check).FirstOrDefault();
                //    upCkek.id_status = 3;
                //    db.SaveChanges();
                //}

            }

        }
        #endregion

    }
}
