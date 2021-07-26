using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DIPLOM
{
    class Manager
    {
        public static Frame NavigationFrame { get; set; }
        public static Frame WorksFrame { get; set; }
        public static Frame MainFrame { get; set; }
        public static string Server = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=Diplom;Integrated Security=True";
        public static int ID_person;
        public static int ID_PersonCheck;
        public static int ID_Check;
        public static int ID_Object;
        public static string CheckName = "";
        public static string WindowDoc = "";
        public static string StatusPerson = "";

        public static string Name = "";
        public static string Fam = "";
        public static string Otch = "";
        public static string Job = "";
        public static string Phone = "";
        public static string Email = "";
        public static string Seria = "";
        public static string Number = "";
        public static string Date = "";
        public static string Photo = "";


        public static string Country = "";
        public static string City = "";
        public static string Street = "";
        public static string House = "";
        public static string Kv = "";
        public static string Form = "";
        public static string NameObject = "";
        public static string VidProdaction = "";
        public static string DateChreck = "";
    }

}
