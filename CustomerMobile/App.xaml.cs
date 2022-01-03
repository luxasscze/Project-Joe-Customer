using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomerMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var splashPage = new NavigationPage(new SplashPage());
            MainPage = splashPage;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
