using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using Xamarin.Forms.GoogleMaps;

namespace CustomerMobile
{
    public static class GlobalVar
    {
        
        //COMPANY SETTINGS
        public static string _CompanyName = "PROJECT JOE";
        public static string _SubCaption = "Best food in the city!";
        public static string _AppIcon64 = "http://projectjoe.lukasslivka.com/Mobile/icon64col.png";
        public static string _AppBackgroundImage = "http://projectjoe.lukasslivka.com/Mobile/bg.jpg";
        public static string _ConnectionString = "Data Source=SQL5097.site4now.net;Initial Catalog=DB_A63C5C_Joe;User Id=DB_A63C5C_Joe_admin;Password=Kasumi2Goto";
        public static string _CompanyOwner = "Lukas Slivka";
        public static string _CompanyPhone = "07384759732";
        public static string _CompanyEmail = "info@lukasslivka.com";
        public static string _UriPath = "http://www.lukasslivka.com/ProjectJoe/";
        public static Position _RestaurantPosition = new Position(52.98175, -1.14635);



        public static string _ConfigFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Settings.dat");
        public static string _TCTextFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TC.txt");
        public static string _GDPRTextFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GDPR.txt");
        public static string _COMPDESCTextFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "COMPDESC.txt");
        public static string _DOCSPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static bool _AutoLogin { get; set; }
        public static bool _IsLogged { get; set; }
        public static string _RegEmail { get; set; }
        public static string _RegPassword { get; set; }

        public static Customer _GlobalCustomer { get; set; }

        public static double _OriginalHeight { get; set; }

        public static bool _ResultOfCheckSum { get; set; }
        public static bool _CheckCustomerResult { get; set; }
        
        public static List<Order> _CurrentOrder = new List<Order>();

        public static ObservableCollection<ItemsList> _CurrentItems = new ObservableCollection<ItemsList>();
        public static Order _ActiveOrder { get; set; }
        public static bool _IsMapUpdating { get; set; }
        public static Position _CustomerPosition { get; set; }
        










        public static string GetDayGreetings()
        {
            if(DateTime.Now.ToLocalTime() > DateTime.Parse("06:00") && DateTime.Now.ToLocalTime() < DateTime.Parse("11:00"))
            {
                return "Time for brunch";
            }
            else if(DateTime.Now.ToLocalTime() > DateTime.Parse("11:00") && DateTime.Now.ToLocalTime() < DateTime.Parse("14:00"))
            {
                return "Time for lunch";
            }
            else if (DateTime.Now.ToLocalTime() > DateTime.Parse("14:00") && DateTime.Now.ToLocalTime() < DateTime.Parse("17:00"))
            {
                return "Time for snack";
            }
            else if(DateTime.Now.ToLocalTime() > DateTime.Parse("17:00") && DateTime.Now.ToLocalTime() < DateTime.Parse("23:00"))
            {
                return "Time for dinner";
            }
            else
            {
                return "Can't sleep";
            }
        }
    }
}
