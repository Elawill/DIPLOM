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
using System.Data;
using System.Data.SqlClient;

namespace DIPLOM.Pade
{
    /// <summary>
    /// Логика взаимодействия для CheckShow.xaml
    /// </summary>
    public partial class CheckShow : Page
    {
        public CheckShow()
        {
            InitializeComponent();
            ShowCheck();
        }
        Expander expanderObject, expanderCategory;
        int now_check, old_check;
        string now_cat, old_cat, name_check;
        public int n;
        bool x, y,z;

        public string Server = Manager.Server;
        DataSet rec;
        SqlDataAdapter sql;
        public void ShowCheck()
        {
            //Иерархический запрос, отображающий всю информацию о проверках
            string rec_zap = "with s as( select o.name as name_check, c.date, c.id_check, ca.name as category, q.name as question, s.name as status " +
                "from[Check] c, Question q, Category ca, Object o, Status s " +
                "where c.id_check = q.id_check " +
                "and ca.id_category = q.id_category " +
                "and o.id_object = c.id_object " +
                "and s.id_status = c.id_status) " +
                "select* from s ";
            //Параметры вывода информации
            if (x == true && y == true)
                rec_zap += "where name_check like '" + txt_search.Text + "%' order by date asc";
            else if (x == true && z == true)
                rec_zap += "where name_check like '" + txt_search.Text + "%' order by date desc";
            else if (x == true)
                rec_zap += "where name_check like '" + txt_search.Text + "%'";
            else if (y == true)
                rec_zap += "order by date asc";
            else if (z == true)
                rec_zap += "order by date desc";

            using (SqlConnection pokl=new SqlConnection(Server))
            {
                pokl.Open();
                rec = new DataSet();
                sql = new SqlDataAdapter(rec_zap,pokl);
                sql.Fill(rec);
            }
            n = Convert.ToInt32(rec.Tables[0].Rows.Count);
            stack.Children.Clear();

            Dictionary<int, Dictionary<string, List<string>>> allCheck = new Dictionary<int, Dictionary<string, List<string>>>();//Главный словарь, содержаший в себе всю информацию о проверке
            Dictionary<int, List<string>> checkList = new Dictionary<int, List<string>>();//Словарь проверок (Ключ ID проверки и категории, которые к ней относятся)
            Dictionary<string, List<string>> categoryQuestion = new Dictionary<string, List<string>>(); //Словарь (ключ категория, и ее вопросы)
            Dictionary<string, int> categoryCheck = new Dictionary<string, int>();//Вспомогательный словарь
                                                                                  //(ключ категория и ее значение ID проверки, к которой она относиться)

            #region Заполнение ключей (id проверки) Словаря Проверки
            for (int i = 0; i < n; i++)
            {
                now_check = Convert.ToInt32(rec.Tables[0].Rows[i]["id_check"]);
                try//Запись только уникальных проверок
                {
                    checkList.Add(now_check, new List<string> { });
                }
                catch { }

            }
            #endregion

            #region Заполнение Словаря Вопросы по категориям
            for (int i = 0; i < n; i++)
            {
                now_cat = rec.Tables[0].Rows[i]["category"].ToString();
                now_check = Convert.ToInt32(rec.Tables[0].Rows[i]["id_check"]);
                try//Запись только уникальных категорий
                {
                    categoryQuestion.Add(now_cat, new List<string> { });//Создание Категории и листа вопросов
                    categoryQuestion[now_cat].Add(rec.Tables[0].Rows[i]["question"].ToString());//Заполнение вопросов относящихся к категории
                    categoryCheck.Add(now_cat, now_check);//заполнение словаря ключей проверки по категориям
                }
                catch //Если ключ категории повторяется в цикле, значит надо записать его вопросы
                {
                    categoryQuestion[now_cat].Add(rec.Tables[0].Rows[i]["question"].ToString());//Заполнение вопросов относящихся к категории
                }
                
            }
            #endregion

            #region Запись В Проверку нужных категорий
            foreach (string c in categoryCheck.Keys)//Цикл по ключам промежуточного словаря КАТЕГОРИИ ПО КЛЮЧАМ ПРОВЕРКИ
            {
                int id = categoryCheck[c]; //значение промежуточного словаря-КЛЮЧ ПРОВЕРКИ               
                foreach (int id_ch in checkList.Keys)//Цикл по ключам  словаря КЛЮЧИ ПРОВЕРКИ И ИХ КАТЕГОРИИ
                {
                    if (id_ch == id)//если ключи равны, то заполняем категории к нужным ключа и создаем общий словарь
                    {
                        checkList[id_ch].Add(c);//заполние категории к нужным ключам
                        try
                        {
                            allCheck.Add(id, new Dictionary<string, List<string>> { });//создание общего словаря
                        }
                        catch { }
                    }
                    else
                        continue;
                }

            }
            #endregion
 
            #region Заполнение Главного словаря значениями-Ключ-название категории
            List<string> category = new List<string>();
            foreach (int id in allCheck.Keys)//Цикл по ключам главного словаря КЛЮЧИ ПРОВЕРКИ
            {
                foreach (int id_ch in checkList.Keys)//Цикл по ключам  словаря КЛЮЧИ ПРОВЕРКИ И ИХ КАТЕГОРИИ
                {
                    if (id == id_ch)
                    {
                        category = checkList[id_ch];//Значение ключа проверки
                        for (int i = 0; i < category.Count; i++)//запись каждой категории
                            allCheck[id].Add(category[i], new List<string> { });//Запись ключа проверку в главный словарь
                    }
                }
            }
            #endregion

            #region Заполнение Главного словаря значениями-Запись вопросов по Ключу категорий
            Dictionary<string, List<string>> test = new Dictionary<string, List<string>>();//Вспомогательный словарь, словарь вопросов по категориям
            List<string> question = new List<string>();
            foreach (int id_ch in allCheck.Keys)//Цикл по ключам главного словаря КЛЮЧИ ПРОВЕРКИ
            {
                test = allCheck[id_ch];//значения главного словаря-КАТЕГОРИИ
                foreach (string cq in categoryQuestion.Keys)//Цикл по ключам словаря ВОПРОСЫ ПО КАТЕГОРИЯМ
                {
                    foreach (string t in test.Keys)//Цикл по ключам словаря НАЗВАНИЯ КАТЕГОРИЙ
                    {
                        if (t == cq)//Если ключи словарей равны, то в главный словарь записываются вопросы в порядке:КЛЮЧ ПРОВЕРКИ-КАТЕГОРИИ-ВОПРОСЫ
                        {
                            question = categoryQuestion[t];
                            for (int i = 0; i < question.Count; i++)//запись вопросов
                            {
                                allCheck[id_ch][t].Add(question[i]);
                            }
                        }
                    }
                }
            }
            #endregion

            #region Вывод проверок
            for (int i = 0; i < n; i++)//Общий цикл, по нему записываюся названия проверок
            {
                foreach (int id_ch in allCheck.Keys)//цикл по главному словарю со всей информацей о проверках
                {
                    if (id_ch == Convert.ToInt32(rec.Tables[0].Rows[i]["id_check"]))//Если ключи проверок равны
                    {
                        now_check = Convert.ToInt32(rec.Tables[0].Rows[i]["id_check"]);
                        if (now_check == old_check)//Если названия проверок развны, цикл переходит к следуюшей проверке, запись только уникальных проверок
                            continue;
                        else
                        {
                            DateTime date = Convert.ToDateTime(rec.Tables[0].Rows[i]["date"]);
                            name_check = rec.Tables[0].Rows[i]["name_check"].ToString() + " " +date.ToShortDateString() ;
                            string status = rec.Tables[0].Rows[i]["status"].ToString();
                            //Запись названия проверок
                            expanderObject = new Expander() { Header = name_check };
                            //Цвет проверки в зависимости от ее статуса
                            if (status == "Завершена")
                                expanderObject.Background = new SolidColorBrush(Color.FromRgb(194, 255, 97));
                            if (status == "Активна")
                                expanderObject.Background = new SolidColorBrush(Color.FromRgb(255, 197, 97));
                            stack.Children.Add(expanderObject);
                            test = allCheck[id_ch];//Вспомогательный словарь, словарь вопросов по категориям
                            StackPanel stackCategory = new StackPanel();
                            foreach (string t in test.Keys)//Цикл по категориям проверки
                            {
                                //запись категорий к каждой проверке
                                expanderCategory = new Expander() { Header = t };
                                expanderCategory.Margin = new Thickness(20, 0, 0, 0);                              
                                expanderObject.Content = stackCategory;
                                stackCategory.Children.Add(expanderCategory);
                                StackPanel stackQuestion = new StackPanel();

                                foreach (string q in test[t])//Цикл вопросов категории
                                {
                                    //Запись вопросов к категории
                                    TextBlock text = new TextBlock();
                                    text.Margin = new Thickness(40, 0, 0, 0);
                                    text.Text += q + "\n";
                                    stackQuestion.Children.Add(text);
                                    expanderCategory.Content = stackQuestion;
                                }
                            }

                        }
                        old_check = Convert.ToInt32(rec.Tables[0].Rows[i]["id_check"]);
                    }
                }
            }
            #endregion

        }
        private void newdateBTN(object sender, RoutedEventArgs e)//Фильтр даты: Сначала новые
        {
            z = true;
            ShowCheck();
        }

        private void oldDateBTN(object sender, RoutedEventArgs e)//Фильтр даты: Сначала старые
        {
            y = true;            
            ShowCheck();
            
        }

        private void txt_login_TextChanged(object sender, TextChangedEventArgs e)//Поиск
        {
            if (txt_search.Text == "")
                x = false;
            else
                x = true;
            ShowCheck();
        }

    }
}
