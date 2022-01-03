using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CustomerMobile.GlobalVar;
using static CustomerMobile.SQL;

namespace CustomerMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Register : ContentPage
    {
        public Customer regCustomer = new Customer();

        public uint speed = 500;
        
        public Register()
        {
            InitializeComponent();
            subCaptionText.Text = "In few easy steps...";
            progressText.Text = "1/4 YOUR NAME";
            iconImage.Source = _AppIcon64;
            backgroundImage.Source = _AppBackgroundImage;
            regFrame1.TranslationX = 100; //CHANGE TO 100 ALL OF THEM! EVERYWHERE!
            regFrame2.TranslationX = 100;
            regFrame3.TranslationX = 100;
            regFrame4.TranslationX = 100;
            regFrame5.TranslationX = 100;
            regFrame1.Opacity = 0;
            regFrame2.Opacity = 0;
            regFrame3.Opacity = 0;
            regFrame4.Opacity = 0;
            regFrame5.Opacity = 0;
            progressProgressBar.Progress = 0.1;
            
            regFrame1.TranslateTo(0, 0, speed);
            regFrame1.FadeTo(1, speed);


        }

        public async void OnNext1Clicked(object sender, EventArgs e)
        {
            if (firstNameInput.Text == null || firstNameInput.Text == String.Empty)
            {
                firstNameInput.BackgroundColor = Color.FromRgb(33, 0, 0);
            }
            else
            {
                firstNameInput.BackgroundColor = Color.Transparent;

                if (surnameInput.Text == null || surnameInput.Text == String.Empty)
                {
                    surnameInput.BackgroundColor = Color.FromRgb(33, 0, 0);
                }
                else
                {
                    surnameInput.BackgroundColor = Color.Transparent;

                    regCustomer.FirstName = firstNameInput.Text;
                    regCustomer.Surname = surnameInput.Text;

                    progressProgressBar.Progress = 0.3;
                    progressText.Text = "2/4 CONTACT INFO";

                    await Task.WhenAll(
                    regFrame1.TranslateTo(-100, 0, speed),
                    regFrame1.FadeTo(0, speed),
                    regFrame2.TranslateTo(0, 0, speed),
                    regFrame2.FadeTo(1, speed)
                    );
                    regFrame2.IsVisible = true;
                    
                    regFrame1.IsVisible = false;
                    subCaptionText.Text = "Few more steps, " + regCustomer.FirstName + "...";
                }
            }
        }

        public void OnFirstNameFocused(object sender, EventArgs e)
        {
            //regFrame1.TranslationY = -200;
        }

        public void OnFirstNameUnfocused(object sender, EventArgs e)
        {
            //regFrame1.TranslationY = 0;
        }

        public void OnSurnameFocused(object sender, EventArgs e)
        {
            //regFrame1.TranslationY = -200;
        }

        public void OnSurnameUnfocused(object sender, EventArgs e)
        {
            //regFrame1.TranslationY = 0;
        }

        public void OnEmailFocused(object sender, EventArgs e)
        {
           // regFrame2.TranslationY = -200;
        }

        public void OnEmailUnfocused(object sender, EventArgs e)
        {
            //regFrame2.TranslationY = 0;
        }

        public void OnPhoneFocused(object sender, EventArgs e)
        {
            //regFrame2.TranslationY = -200;
        }

        public void OnPhoneUnfocused(object sender, EventArgs e)
        {
            //regFrame2.TranslationY = 0;
        }

        public async void OnPrevious2Clicked(object sender, EventArgs e)
        {
            subCaptionText.Text = "In few easy steps...";
            progressProgressBar.Progress = 0.1;
            progressText.Text = "1/4 YOUR NAME";
            regFrame1.TranslationX = -100;
            await Task.WhenAll(
            regFrame2.TranslateTo(100, 0, speed),
            regFrame2.FadeTo(0, speed),
            regFrame1.TranslateTo(0, 0, speed),
            regFrame1.FadeTo(1, speed)
            );
            regFrame2.IsVisible = false;
            regFrame1.IsVisible = true;
        }

        public async void OnNext2Clicked(object sender, EventArgs e)
        {
            if (emailInput.Text == null || emailInput.Text == String.Empty || !emailInput.Text.Contains("@") || !emailInput.Text.Contains("."))
            {
                emailInput.BackgroundColor = Color.FromRgb(33, 0, 0);
                
            }
            else
            {
                if (CheckUserExists(emailInput.Text))
                {
                    emailInput.BackgroundColor = Color.FromRgb(33, 0, 0);
                    await DisplayAlert("EMAIL IN USE", "It looks like this email is already registered with us.", "OK");
                }
                else
                {
                    emailInput.BackgroundColor = Color.Transparent;
                    if (phoneInput.Text == null || phoneInput.Text == String.Empty || phoneInput.Text.Length < 6)
                    {
                        phoneInput.BackgroundColor = Color.FromRgb(33, 0, 0);
                    }
                    else
                    {
                        phoneInput.BackgroundColor = Color.Transparent;
                        regCustomer.Email = emailInput.Text;
                        regCustomer.Phone = phoneInput.Text;

                        progressProgressBar.Progress = 0.5;
                        progressText.Text = "3/4 DELIVERY ADDRESS";

                        await regFrame2.TranslateTo(-1000, 0, speed);
                        regFrame3.IsVisible = true;
                        await regFrame3.TranslateTo(0, 0, speed);
                        regFrame2.IsVisible = false;
                        subCaptionText.Text = "Few more steps, " + regCustomer.FirstName + "...";
                    }
                }
            }
        }

        public async void OnPrevious3Clicked(object sender, EventArgs e)
        {
            progressProgressBar.Progress = 0.3;
            progressText.Text = "2/4 CONTACT INFO";
            await regFrame3.TranslateTo(1000, 0, speed);
            regFrame3.IsVisible = false;
            regFrame2.IsVisible = true;
            regFrame2.TranslationX = -1000;
            await regFrame2.TranslateTo(0, 0, speed);
        }

        public async void OnNext3Clicked(object sender, EventArgs e)
        {
            if (address1Input.Text == null || address1Input.Text == String.Empty)
            {
                address1Input.BackgroundColor = Color.FromRgb(33, 0, 0);
            }
            else
            {
                address1Input.BackgroundColor = Color.Transparent;

                if (address2Input.Text == null || address2Input.Text == String.Empty)
                {
                    address2Input.BackgroundColor = Color.FromRgb(33, 0, 0);
                }
                else
                {
                    address2Input.BackgroundColor = Color.Transparent;

                    if (postCodeInput.Text == null || postCodeInput.Text == String.Empty)
                    {
                        postCodeInput.BackgroundColor = Color.FromRgb(33, 0, 0);
                    }
                    else
                    {
                        postCodeInput.BackgroundColor = Color.Transparent;
                        
                        regCustomer.Address1 = address1Input.Text;
                        regCustomer.Address2 = address2Input.Text;
                        regCustomer.Address3 = address3Input.Text;
                        regCustomer.PostCode = postCodeInput.Text;

                        progressProgressBar.Progress = 0.7;
                        progressText.Text = "4/4 PASSWORD";

                        await regFrame3.TranslateTo(-1000, 0, speed);
                        regFrame4.IsVisible = true;
                        await regFrame4.TranslateTo(0, 0, speed);
                        regFrame3.IsVisible = false;
                        subCaptionText.Text = "Few more steps, " + regCustomer.FirstName + "...";
                    }
                }
            }
        }

        public async void OnPrevious4Clicked(object sender, EventArgs e)
        {
            progressProgressBar.Progress = 0.5;
            progressText.Text = "3/4 DELIVERY ADDRESS";
            await regFrame4.TranslateTo(1000, 0, speed);
            regFrame4.IsVisible = false;
            regFrame3.IsVisible = true;
            regFrame3.TranslationX = -1000;
            await regFrame3.TranslateTo(0, 0, speed);
        }

        public async void OnNext4Clicked(object sender, EventArgs e)
        {

            if (passwordInput.Text == null || passwordInput.Text == String.Empty)
            {
                passwordInput.BackgroundColor = Color.FromRgb(33, 0, 0);
            }
            else
            {
                passwordInput.BackgroundColor = Color.Transparent;

                if (retypeInput.Text == null || retypeInput.Text == String.Empty)
                {
                    retypeInput.BackgroundColor = Color.FromRgb(33, 0, 0);
                }
                else
                {
                    retypeInput.BackgroundColor = Color.Transparent;
                    if (passwordInput.Text != retypeInput.Text)
                    {
                        notMatchText.IsVisible = true;
                        passwordInput.BackgroundColor = Color.FromRgb(33, 0, 0);
                        retypeInput.BackgroundColor = Color.FromRgb(33, 0, 0);
                    }
                    else
                    {

                        notMatchText.IsVisible = false;
                        regCustomer.Password = retypeInput.Text;

                        progressProgressBar.Progress = 1;
                        progressText.Text = "WELCOME!";

                        await regFrame4.TranslateTo(-1000, 0, speed);
                        regFrame5.IsVisible = true;
                        await regFrame5.TranslateTo(0, 0, speed);
                        regFrame4.IsVisible = false;
                        subCaptionText.Text = "Last step...";
                        NewCustomer(regCustomer.Email, regCustomer.Surname, regCustomer.FirstName, regCustomer.Phone, regCustomer.Password, "", regCustomer.Address1, regCustomer.Address2, regCustomer.Address3, regCustomer.PostCode);
                        
                    }
                }
            }
        }

        public async void OnShowMeMenuClicked(object sender, EventArgs e)
        {
            _IsLogged = true;
            _RegEmail = regCustomer.Email;
            _RegPassword = regCustomer.Password;
            await Navigation.PopModalAsync();
        }

    }
}