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
    /// Логика взаимодействия для CteateCheck.xaml
    /// </summary>
    public partial class CteateCheck : Page
    {
        DataBase.Object ob;
        Adress adress;
        Expander expander;
        Dictionary<string, List<string>> question_cat = new Dictionary<string, List<string>>();
        public string cat, old_cat, ex_Header, ex_name, category_name;
      
        public CteateCheck()
        {
            InitializeComponent();
            MessageBox.Show("Пожалуйста, ответьте на критерий!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //Заполнение списка об объекте
            var info = DiplomEntities.GetContext().Object.Select(p=>p.name).ToList();
            ObjectBox.ItemsSource = info;
            //Заполнение списка о категории
            var info2 = DiplomEntities.GetContext().Category.Select(p => p.name).ToList();
            CategoryBox.ItemsSource = info2;
            //Если выбран проверяющий
            if (Manager.ID_PersonCheck != 0)
            {
                //отобрражение всей выбранной ранее информации
                var info1 = DiplomEntities.GetContext().Person.Where(p=>p.id_person==Manager.ID_PersonCheck).FirstOrDefault();
                txt_person_check.Text = info1.fio;                              
                if (Manager.NameObject != "")
                {
                    ObjectBox.SelectedItem = Manager.NameObject;                   
                    ExpanderInfo.Visibility = Visibility.Visible;
                }
                VidCheckBox.SelectedItem = Manager.VidProdaction;
                txt_date.Text = Manager.DateChreck;
                Cat_que_stack.Visibility = Visibility.Visible;
                Manager.NameObject = "";
            }
        }

        #region Выбор данных о проверке
        #region Выбор объекта
        private void ObjectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ObjectBox.SelectedItem = Manager.NameObject;//Имя объекта
                ExpanderInfoShow();//Вывод его полной информации
            }
            catch
            {
                return;
            }

        }

        #region Метод ищущий информацию для вывода полной информации об объекте
        public void ExpanderInfoShow()//Метод ищущий информацию для вывода полной информации об объекте
        {
            //Нахождение объекта
            using (DiplomEntities db = new DiplomEntities())
            {
                ob = db.Object.Where(p => p.name == ObjectBox.SelectedItem.ToString()).FirstOrDefault();
            }

            int id_adress = ob.id_adress;
            //Нахождение адреса объекта
            using (DiplomEntities db = new DiplomEntities())
            {
                adress = db.Adress.Where(p => p.id_adress == id_adress).FirstOrDefault();
            }
            //Вывод информации об объекте
            txt_name_ob.Text = ob.name;
            txt_person.Text = ob.person;
            txt_vid_production.Text = ob.vid_production;
            txt_street.Text = adress.street;
            txt_n_house.Text = adress.n_house.ToString();
            txt_city.Text = adress.city;
            ExpanderInfo.Visibility = Visibility.Visible;
        }
        #endregion

        #endregion
        #region Вид проверки
        private void VidCheckBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Manager.VidProdaction = VidCheckBox.SelectedItem.ToString();//Сохранение выбранного вида проверки
        }
        #endregion
        #endregion

        #region Сохранение
        private void CreateCheck_BTN(object sender, RoutedEventArgs e)//Кнопка сохранить
        {
            if (ObjectBox.SelectedItem == null || String.IsNullOrEmpty(txt_date.Text) || Manager.ID_PersonCheck == 0 || String.IsNullOrEmpty(VidCheckBox.Text))
                MessageBox.Show("Заполните все данные о проверке", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (question_cat.Count == 0)
                MessageBox.Show("Вы не создали категорию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (question_cat[old_cat].Count == 0)
                MessageBox.Show("Заполните категорию вопросами!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                InsertCheck();
            }

        }

        public void InsertCheck()//Метод создающий проверку
        {
            #region Добавление Проверки
            //Нахождение объекта для создания проверки
            var info = DiplomEntities.GetContext().Object.Where(p => p.name == ObjectBox.SelectedItem.ToString()).FirstOrDefault();
            int id_object = info.id_object;
            if (Manager.ID_PersonCheck != Manager.ID_person)
            {
                //Запись данных о проверке
                Check addCheck = new Check
                {
                    id_person = Manager.ID_PersonCheck,
                    id_object = id_object,
                    id_status = 1,
                    vid = VidCheckBox.Text,
                    date = Convert.ToDateTime(txt_date.SelectedDate.Value)
                };
                //Добавление проверки
                if (addCheck.id_check == 0)
                {
                    DiplomEntities.GetContext().Check.Add(addCheck);
                }
                Check check = new Check();
                int id_check = check.id_check;
                int id_category;
                #endregion
                #region Добавление Категории и их Вопросов
                foreach (string c in question_cat.Keys)//Цикл по ключам (категории) словаря
                {
                    //Запись категории
                    Category addCategory = new Category
                    {
                        name = c
                    };
                    //Добавление категории
                    if (addCategory.id_category == 0)
                    {
                        DiplomEntities.GetContext().Category.Add(addCategory);

                    }
                    Category category = new Category();
                    id_category = category.id_category;

                    foreach (string q in question_cat[c]) //Цикл по значениям ключа (вопросы) словаря
                    {
                        //Запись вопросов, относящихся к определенной проверке
                        Question aaQuestion = new Question
                        {
                            name = q,
                            answer = "no",
                            id_category = id_category,
                            id_check = id_check

                        };
                        //Добавление вопросов
                        if (aaQuestion.id_question == 0)
                        {
                            DiplomEntities.GetContext().Question.Add(aaQuestion);

                        }
                    }
                }
                #endregion

                //Сохранение изменений
                try
                {
                    DiplomEntities.GetContext().SaveChanges();
                    Person upPerson = null;
                    //Изменение статуса сотрудника на проверяющего
                    using (DiplomEntities db = new DiplomEntities())
                    {
                        upPerson = db.Person.Where(p => p.id_person == Manager.ID_PersonCheck).FirstOrDefault();
                        upPerson.status_check = "Проверяющий";
                        db.SaveChanges();
                    }
                    MessageBox.Show("Проверка создана!", "", MessageBoxButton.OK, MessageBoxImage.Information);
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

        #region Добавить вопрос
        private void txt_question_KeyDown(object sender, KeyEventArgs e)
        {          
            if (e.Key == Key.Enter)
            {
                if (String.IsNullOrEmpty(txt_question.Text))//Если поле с записью вопросов пустое, то ничего не происходит
                    return;
                else
                {
                    StackPanel stackPanel = new StackPanel();
                    TextBlock text = new TextBlock();

                    if (question_cat[old_cat].Count == 0)//Если пользователь хотел доьавить новуб категорию,
                                                         //не заполнив старую, то ключ словаря для записи обновляется на предыдущую Категорию
                        cat = old_cat;

                    if (question_cat[cat].Contains(txt_question.Text))//Записан ли такой вопрос
                        MessageBox.Show("Такой вопрос уже есть!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {                        
                        question_cat[cat].Add(txt_question.Text);//Запись вопроса к соотвествуюхей категории
                        for (int j = 0; j < question_cat[cat].Count; j++)
                            text.Text += question_cat[cat][j] + "\n";
                        stackPanel.Children.Add(text);
                        expander.Content = stackPanel;
                        txt_question.Clear();
                    }
                }
            }
        }
        #endregion
        public void ExpanderCategory()//Метод отобращающий записанные категории
        {

            question_cat.Add(category_name, new List<string> { });
            expander = new Expander() { Header = category_name, Name = ex_name, Visibility = Visibility.Visible };
            expander.MouseDoubleClick += expander_MouseDoubleClick;
            stack.Children.Add(expander);
            old_cat = cat;
        }

        #region Проверка записи выбранной/созданной категории
        public void RecordCategory()
        {
            cat = category_name;//категория-ключ словаря

            if (!question_cat.ContainsKey(txt_category.Text))//Записана ли такая категория
            {
                if (old_cat != null)//Если пользователь хочет создать новую К не заполнив старую
                {
                    if (question_cat[old_cat].Count == 0)
                        MessageBox.Show("Заполните категорию вопросами!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                    {
                        ExpanderCategory();
                    }
                }

                else
                {
                    ExpanderCategory();
                }

            }
            else
                MessageBox.Show("Такая категория уже есть!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        #region Добавить категорию
        private void txt_category_KeyDown(object sender, KeyEventArgs e)
        {
            category_name = txt_category.Text;
            #region Нажатие Enter-добавление категории
            if (e.Key == Key.Enter)
            {
                if (String.IsNullOrEmpty(txt_category.Text))//Если поле с записью категории пустое, то ничего не происходит
                    return;
                else
                {
                    RecordCategory();
                    txt_category.Clear();
                }
            }
            #endregion

        }
        #endregion
        private void CategorytBox_SelectionChanged(object sender, SelectionChangedEventArgs e)//Выбрать категорию из предложенных
        {
            category_name = CategoryBox.SelectedItem.ToString();           
            RecordCategory();
        }

        private void PackIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txt_category.Visibility = Visibility.Visible;
        }

        #region Проверка на выбранную дату
        private void txt_date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)//Проверка на выбранную дату
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
        #endregion
        private void expander_MouseDoubleClick(object sender, MouseButtonEventArgs e) //Вывод названия категории на текстовое поле
        {
            var q = (sender as Expander).Header;
            ex_Header = q.ToString();
            txt_category.Text = ex_Header;
        }
        
        #region Добавление сотрудника
        private void BNTperson_Clik(object sender, RoutedEventArgs e)//Добавление сотрудника
        {
            //Сохранение данных
            Manager.Form = "CreateCheck";
            if (ObjectBox.SelectedItem != null)
                Manager.NameObject = ObjectBox.SelectedItem.ToString();
            if (txt_date.Text != null)
                Manager.DateChreck = txt_date.Text;
            Manager.NavigationFrame.Navigate(new Works());
        }
        #endregion


    }
}
