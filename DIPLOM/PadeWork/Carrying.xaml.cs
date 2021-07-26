using DIPLOM.DataBase;
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

namespace DIPLOM.PadeWork
{
    /// <summary>
    /// Логика взаимодействия для Carrying.xaml
    /// </summary>
    public partial class Carrying : Page
    {
        public string Server = Manager.Server;
        DataSet rec;
        SqlDataAdapter sql;

        public int n;
        int ii =0;
        Dictionary<string, string> photoNarush = new Dictionary<string, string>();
        Dictionary<string, string> comment = new Dictionary<string, string>();
        Dictionary<string, string> answerQuestion = new Dictionary<string, string>();
        string question;
        string answer="";
        bool flag = false, x = false;
        public string zap;
        public Carrying()
        {
            InitializeComponent();            
            ShowInformation();

        }

        #region Вывод информации
        public void ShowInformation()
        {
            //Вывод нужный вопросов для Проверяющего
            if (Manager.StatusPerson == "Проверяющий")
            {
                zap = "with s as( select o.name +' '+CAST(c.date as nvarchar) as name_check, " +
                "o.name as name_ob, c.date, c.id_check, ca.name as category, q.name as question " +
                "from[Check] c, Question q, Category ca, Object o where c.id_check = q.id_check " +
                "and ca.id_category = q.id_category and o.id_object = c.id_object) " +
                "select* from s where id_check ="+ Manager.ID_Check;
            }
            //Вывод только нарушенных вопросов для Ответсвенного
            else if (Manager.StatusPerson == "Ответсвенный")
            {
                zap = "with s as( select o.name +' '+CAST(c.date as nvarchar) as name_check, " +
                    "o.name as name_ob, c.date, c.id_check, ca.name as category, q.name as question " +
                    "from[Check] c, Question q, Category ca, Object o, Narushenia n " +
                    "where c.id_check = q.id_check " +
                    "and ca.id_category = q.id_category and o.id_object = c.id_object and n.id_question = q.id_question)  " +
                    "select* from s where id_check =" + Manager.ID_Check;
            }
            using (SqlConnection pokl = new SqlConnection(Server))
            {
                pokl.Open();
                rec = new DataSet();
                sql = new SqlDataAdapter(zap, pokl);
                sql.Fill(rec);
            }
            txt_name.Text = rec.Tables[0].Rows[0]["name_check"].ToString();
            Manager.CheckName = rec.Tables[0].Rows[0]["name_check"].ToString();
            txt_category.Text = rec.Tables[0].Rows[0]["category"].ToString();
            txt_question.Text = rec.Tables[0].Rows[0]["question"].ToString();
            n = Convert.ToInt32(rec.Tables[0].Rows.Count);
        }
        #endregion

        #region Назад
        private void Back_MouseDown(object sender, MouseButtonEventArgs e)
        {
            flag = false;
            SaveBTN.Visibility = Visibility.Hidden;
            next.Visibility = Visibility.Visible;
            if (ii > 0)//Вывод нужных вопрос при переходе назад
            {
                ii--;
                question = rec.Tables[0].Rows[ii]["question"].ToString();
                QuestionCategory();//Метод вывода информации при переходах
            }
            else if (ii == 0)
                back.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Вперед
        private void Next_MouseDown(object sender, MouseButtonEventArgs e)
        {
            flag = true;

            question = rec.Tables[0].Rows[ii]["question"].ToString();//Получение тек вопроса 
            try
            {
                if (answerQuestion[question] != "" && answerQuestion.Count > 0)//Если при переходе вперед есть ранее сохраненный ответ
                {
                    try
                    {
                        if (answerQuestion[question] != "")//Если ли есть ответ на тек. вопрос
                        {
                            ii++;//Номер сл вопроса
                            x = true;
                        }
                        question = rec.Tables[0].Rows[ii]["question"].ToString();//След вопрос
                        //Ответ на след вопрос
                        if (answerQuestion[question] == "Да")
                            YesRB.IsChecked = true;
                        else
                            NoRB.IsChecked = true;
                    }
                    catch//Если в ходе проверки на то был ли ответ на след вопрос, произошла ошибка, то данные возвращаются
                    {
                        ii--;
                        x = false;

                    }

                }
                else
                {
                    answerQuestion.Add(question, answer);//добавление ответа на вопрос

                }

            }
            catch
            {
                if (answer != "")//Если ответ на вопрос не пустой, то добавляем его к ответу
                    answerQuestion.Add(question, answer);
            }
            if (ii < n - 1)//Вывод информации при переходе вперед
            {
                if ((YesRB.IsChecked == false) && (NoRB.IsChecked == false))//Если нет ответа на вопрос
                {
                    MessageBox.Show("Пожалуйста, ответьте на вопрос", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                //Если осуществляется переход вперед при ответе НЕТ с незаполненым коментарием
                else if (answer == "Нет" && txt_comment.Text == "")
                    MessageBox.Show("Укажите причину", "", MessageBoxButton.OK, MessageBoxImage.Information);
                //Если осуществляется переход вперед при повторной проверке с ответом ДА с незаполненым коментарием
                else if (answer == "Да" && Manager.StatusPerson == "Ответсвенный")
                    MessageBox.Show("Укажите, что было исправлено!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                {
                    if (x == false)//Если далее нет сохраненых ответов
                    {

                        if (answer == "Нет")//Если ответ на вопрос НЕТ, то сохраняется комментарий для пояснения нарушения
                        {
                            question = rec.Tables[0].Rows[ii]["question"].ToString();
                            comment.Add(question, txt_comment.Text);//Добавление коментария к вопросу             
                            txt_comment.Clear();
                            try
                            {
                                photoNarush.Add(question, null);//Добавление пустого значения при фото
                            }
                            catch { }
                        }
                        ii++;
                    }
                    QuestionCategory();//Метод вывода информации при переходах
                    back.Visibility = Visibility.Visible;
                }
            }

            else
            {
                next.Visibility = Visibility.Hidden;
                SaveBTN.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Метод вывода информации при переходах
        public void QuestionCategory()
        {
            txt_category.Text = rec.Tables[0].Rows[ii]["category"].ToString();
            txt_question.Text = rec.Tables[0].Rows[ii]["question"].ToString();
            //Всегда показывать поле с добавлением комментария при повторной проверке
            try
            {
                if (comment[txt_question.Text] != "")//Если поле с комментарием присутсвует при переходах,
                                                     //то оно отображает сохраненную информацию
                {
                    txt_comment.Text = comment[txt_question.Text];
                    narush.Visibility = Visibility.Visible;
                    //txt_comment.Clear();
                }

            }
            catch
            {
                narush.Visibility = Visibility.Hidden;//Скрывать поле с комментарием, если не было нарушений
            }

            if (x == false)//Если переход вперед и далее не было сохраненных ответов
            {
                YesRB.IsChecked = false;
                NoRB.IsChecked = false;
            }                   
            if (flag == false)//Если пользователь переходит назад, вывод ответа
            {
                if (answerQuestion[question] == "Да")
                    YesRB.IsChecked = true;
                else
                    NoRB.IsChecked = true;
            }
            answer = "";//Обнуление ответа   
            x = false;
        }
        #endregion

        #region Сохранение прохождения проверки
        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            foreach (string q in answerQuestion.Keys)//Цикл ответов на вопросы
            {
                Question quest = null;
                string a = answerQuestion[q];//Ответ на вопрос
                //Изменение ответа на вопросы
                using (DiplomEntities db = new DiplomEntities())
                {
                    quest = db.Question.Where(p => p.name == q).FirstOrDefault();
                    quest.answer = a;
                    db.SaveChanges();
                }

                if (quest != null)
                {
                    int id_question = quest.id_question;//ID вопроса
                    if (a == "Нет")//Было ли созданно нарушение
                    {
                        Narushenia addNarushenia = null;

                        foreach (string com in comment.Keys)//Цикл по комментариям
                        {
                            if (q == com)//Если ключ ответа на вопрос совпадает ключу вопроса с комментарияем
                            {
                                foreach (string ph in photoNarush.Keys)//Цикл по фото
                                {
                                    if (ph == q)//Если ключ ответа на вопрос совпадает ключу вопроса с фото
                                    {
                                        string value_photo = photoNarush[ph];//Фото
                                        string value_comment = comment[com]; //Комментарий
                                        //Запись нарушения 
                                        addNarushenia = new Narushenia
                                        {
                                            id_question = id_question,
                                            comment = value_comment,
                                            photo = value_photo
                                        };

                                    }
                                }

                            }
                        }
                        //Добавление нарушения
                        if (addNarushenia.id_narushen == 0)
                        {
                            DiplomEntities.GetContext().Narushenia.Add(addNarushenia);
                        }
                    }
                }
            }
            //Сохранение данных и переход на форму результатов
            try
            {
                DiplomEntities.GetContext().SaveChanges();
                MessageBox.Show("Проверка завершена!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                Manager.WorksFrame.Navigate(new DocumentsPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        #endregion


        private void NO_question(object sender, RoutedEventArgs e)
        {
            narush.Visibility = Visibility.Visible;
            answer = "Нет";
        }

        private void txt_comment_KeyDown(object sender, KeyEventArgs e)
        {
            //question = txt_question.Text;
            //if (e.Key == Key.Enter)
            //{
            ////    comment.Add(question, txt_comment.Text);//ofd.FileName              
            ////    txt_comment.Clear();
            ////    try
            ////    {
            ////        photoNarush.Add(question, "");//ofd.FileName 
            ////    }
            ////    catch { }
               
            //}
        }

        private void YES_question(object sender, RoutedEventArgs e)
        {
            if (Manager.StatusPerson == "Ответсвенный")
                narush.Visibility = Visibility.Visible;
            else
                narush.Visibility = Visibility.Hidden;
            answer = "Да";
        }

        #region Добавление фото
        private void ImageNarush(object sender, RoutedEventArgs e)
        {
            question = txt_question.Text;//Текущий вопрос
            Stream stream = null;
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "(*.jpg)|*.jpg";
            if (ofd.ShowDialog() != null)
            {
                try
                {
                    if ((stream = ofd.OpenFile()) != null)
                    {
                        photoNarush.Add(question, ofd.FileName);//Добавления фото                                             
                    }
                }
                catch (Exception ex)
                {
                    photoNarush[question] = ofd.FileName; //добавление фото, если пользователь вначале написал комментарий, а затем добавил фото

                }
            }

        }
        #endregion

    }
}
