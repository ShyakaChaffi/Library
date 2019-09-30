using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using prbd_1819_g18.Properties;
using PRBD_Framework;
using prbd_1819_g18;

namespace prbd_1819_g18
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    /// 
    public enum AppMessages
    {
        NEW_BOOK,ISBN_CHANGED, MSG_BOOK_CHANGED,MSG_DETAIL_BOOK, MSG_CLOSE_TAB,MSG_REFRESH_BOOK_LIST,UPDATE_CATEGORY,DISPLAY_CATEGORY,PRESELECTE_CAT, MSG_ADDBASKET, MSG_REFRESH_RENTAL

    }

    public partial class App : ApplicationBase

    {
        public static Model Model = Model.CreateModel(DbType.MsSQL);

        public static User CurrentUser { get; set; }

        public static readonly string IMAGE_PATH = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../images");

        //public static bool IsAdmin
        //{ get { return App.CheckAdmin(); } }

        //public static bool CheckAdmin()
        //{
        //    if (App.CurrentUser != null && App.CurrentUser.Role == Role.Admin)
        //    {

        //        return true;
        //    }
        //    else
        //    {

        //        return false;
        //    }
        //}

        public App(){
            Model.ClearDatabase();
            Model.CreateTestData();
            ColdStart();
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Settings.Default.Culture);
        }


        public void ColdStart() {
            var query = from m in App.Model.Users
                        where m.UserName == "DUMMY"
                        select m;
        }


        public static void Msg(Object param) {
            Console.WriteLine(param);
        }

      

    }
}
