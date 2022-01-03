using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FluentFTP;
using static CustomerMobile.GlobalVar;

namespace CustomerMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
        }

        

        public async Task RunAnim()
        {
            await SplashIcon.ScaleTo(100.0, 1000, Easing.CubicInOut);
            await SplashIcon.FadeTo(0, 700, Easing.CubicInOut);
        }

        protected async override void OnAppearing()
        {
            SplashIcon.Scale = 3;
            base.OnAppearing();            
            await SplashIcon.ScaleTo(0.9, 2000, Easing.CubicInOut);
            
            var animationTasks = new[]
            {
                
            SplashIcon.ScaleTo(8.0, 1000, Easing.CubicInOut),
            SplashIcon.FadeTo(0, 700, Easing.CubicInOut)
            };         
           
            
           
            await Task.WhenAny(animationTasks);


            Navigation.InsertPageBefore(new MainPage(), Navigation.NavigationStack[0]);
            await Navigation.PopToRootAsync(false);
        }
    }
}