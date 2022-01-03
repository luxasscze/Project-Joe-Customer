using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CustomerMobile.GlobalVar;
using System.IO;
using FluentFTP;
using System.Threading;

namespace CustomerMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TextReader : ContentPage
    {
        
        
        public TextReader(string caption, string subCaption, string content)
        {
            InitializeComponent();
            bgImage.Source = _AppBackgroundImage;
            captionText.Text = caption;
            subCaptionText.Text = subCaption;
            contentText.Text = File.ReadAllText(_TCTextFile);
        }

        public async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }

       

        public async void OnTCClicked(object sender, EventArgs e)
        {
            TCButton.BackgroundColor = Color.White;
            TCButton.BorderColor = Color.Black;
            TCButton.TextColor = Color.Black;

            GDPRButton.BackgroundColor = Color.Black;
            GDPRButton.BorderColor = Color.White;
            GDPRButton.TextColor = Color.White;

            COMPDESCButton.BackgroundColor = Color.Black;
            COMPDESCButton.BorderColor = Color.White;
            COMPDESCButton.TextColor = Color.White;

            await contentFrame.ScaleTo(0.05, 300, Easing.CubicInOut);
            contentText.Text = String.Empty;
            contentText.Text = File.ReadAllText(_TCTextFile);
            await contentFrame.ScaleTo(1, 300, Easing.CubicInOut);
            subCaptionText.Text = "Terms & Conditions";
        }

        public async void OnGDPRClicked(object sender, EventArgs e)
        {
            TCButton.BackgroundColor = Color.Black;
            TCButton.BorderColor = Color.White;
            TCButton.TextColor = Color.White;

            GDPRButton.BackgroundColor = Color.White;
            GDPRButton.BorderColor = Color.Black;
            GDPRButton.TextColor = Color.Black;

            COMPDESCButton.BackgroundColor = Color.Black;
            COMPDESCButton.BorderColor = Color.White;
            COMPDESCButton.TextColor = Color.White;

            await contentFrame.ScaleTo(0.05, 300, Easing.CubicInOut);
            contentText.Text = String.Empty;
            contentText.Text = File.ReadAllText(_GDPRTextFile);
            await contentFrame.ScaleTo(1, 300, Easing.CubicInOut);
            subCaptionText.Text = "GDPR";
        }

        public async void OnCOMPDESCClicked(object sender, EventArgs e)
        {
            TCButton.BackgroundColor = Color.Black;
            TCButton.BorderColor = Color.White;
            TCButton.TextColor = Color.White;

            GDPRButton.BackgroundColor = Color.Black;
            GDPRButton.BorderColor = Color.White;
            GDPRButton.TextColor = Color.White;

            COMPDESCButton.BackgroundColor = Color.White;
            COMPDESCButton.BorderColor = Color.Black;
            COMPDESCButton.TextColor = Color.Black;

            await contentFrame.ScaleTo(0.05, 300, Easing.CubicInOut);
            contentText.Text = String.Empty;
            contentText.Text = File.ReadAllText(_COMPDESCTextFile);
            await contentFrame.ScaleTo(1, 300, Easing.CubicInOut);
            subCaptionText.Text = "Company Description";
        }
    }
}