using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CustomerMobile.GlobalVar;
using static CustomerMobile.SQL;
using Acr.UserDialogs;

namespace CustomerMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        public uint animSpeed = 100;
        
        public Settings()
        {
            InitializeComponent();
            ReadCustomerData();
            settingsDeliveryAddressFrame.TranslationX = 1000;
            settingsContactInfoFrame.TranslationX = 1000;
            settingsPasswordFrame.TranslationX = 1000;
            settingsAccountFrame.TranslationX = 1000;
            bgImage.Source = "settingsBG720.png";
            settingsDeliveryAddressFrame.TranslateTo(0, 0, animSpeed);
        }

        public void ReadCustomerData()
        {
            address1Text.Text = _GlobalCustomer.Address1;
            address2Text.Text = _GlobalCustomer.Address2;
            address3Text.Text = _GlobalCustomer.Address3;
            postCodeText.Text = _GlobalCustomer.PostCode;
            emailText.Text = _GlobalCustomer.Email;
            phoneText.Text = _GlobalCustomer.Phone;
            firstNameText.Text = _GlobalCustomer.FirstName;
            SurnameText.Text = _GlobalCustomer.Surname;
        }

        public async void OnDeliveryAddressClicked(object sender, EventArgs e)
        {
            await settingsContactInfoFrame.TranslateTo(1000, 0, animSpeed);
            settingsContactInfoFrame.IsVisible = false;
            await settingsPasswordFrame.TranslateTo(1000, 0, animSpeed);
            settingsPasswordFrame.IsVisible = false;
            await settingsAccountFrame.TranslateTo(1000, 0, animSpeed);
            settingsAccountFrame.IsVisible = false;
            settingsDeliveryAddressFrame.IsVisible = true;
            await settingsDeliveryAddressFrame.TranslateTo(0, 0, animSpeed);
            
            deliveryAddressButton.BackgroundColor = Color.White;
            deliveryAddressButton.BorderColor = Color.Black;
            deliveryAddressButton.TextColor = Color.Black;

            contactInfoButton.BackgroundColor = Color.Black;
            contactInfoButton.BorderColor = Color.White;
            contactInfoButton.TextColor = Color.White;

            passwordButton.BackgroundColor = Color.Black;
            passwordButton.BorderColor = Color.White;
            passwordButton.TextColor = Color.White;

            accountButton.BackgroundColor = Color.Black;
            accountButton.BorderColor = Color.White;
            accountButton.TextColor = Color.White;
        }

        public async void OnContactInfoClicked(object sender, EventArgs e)
        {
            await settingsDeliveryAddressFrame.TranslateTo(1000, 0, animSpeed);
            settingsDeliveryAddressFrame.IsVisible = false;
            await settingsPasswordFrame.TranslateTo(1000, 0, animSpeed);
            settingsPasswordFrame.IsVisible = false;
            await settingsAccountFrame.TranslateTo(1000, 0, animSpeed);
            settingsAccountFrame.IsVisible = false;
            settingsContactInfoFrame.IsVisible = true;
            await settingsContactInfoFrame.TranslateTo(0, 0, animSpeed);

            deliveryAddressButton.BackgroundColor = Color.Black;
            deliveryAddressButton.BorderColor = Color.White;
            deliveryAddressButton.TextColor = Color.White;

            contactInfoButton.BackgroundColor = Color.White;
            contactInfoButton.BorderColor = Color.Black;
            contactInfoButton.TextColor = Color.Black;

            passwordButton.BackgroundColor = Color.Black;
            passwordButton.BorderColor = Color.White;
            passwordButton.TextColor = Color.White;

            accountButton.BackgroundColor = Color.Black;
            accountButton.BorderColor = Color.White;
            accountButton.TextColor = Color.White;
        }

        public async void OnPasswordClicked(object sender, EventArgs e)
        {
            await settingsContactInfoFrame.TranslateTo(1000, 0, animSpeed);
            settingsContactInfoFrame.IsVisible = false;
            await settingsDeliveryAddressFrame.TranslateTo(1000, 0, animSpeed);
            settingsDeliveryAddressFrame.IsVisible = false;
            await settingsAccountFrame.TranslateTo(1000, 0, animSpeed);
            settingsAccountFrame.IsVisible = false;
            settingsPasswordFrame.IsVisible = true;
            await settingsPasswordFrame.TranslateTo(0, 0, animSpeed);
            deliveryAddressButton.BackgroundColor = Color.Black;
            deliveryAddressButton.BorderColor = Color.White;
            deliveryAddressButton.TextColor = Color.White;

            contactInfoButton.BackgroundColor = Color.Black;
            contactInfoButton.BorderColor = Color.White;
            contactInfoButton.TextColor = Color.White;

            passwordButton.BackgroundColor = Color.White;
            passwordButton.BorderColor = Color.Black;
            passwordButton.TextColor = Color.Black;

            accountButton.BackgroundColor = Color.Black;
            accountButton.BorderColor = Color.White;
            accountButton.TextColor = Color.White;
        }

        public async void OnAccountClicked(object sender, EventArgs e)
        {
            await settingsContactInfoFrame.TranslateTo(1000, 0, animSpeed);
            settingsContactInfoFrame.IsVisible = false;
            await settingsDeliveryAddressFrame.TranslateTo(1000, 0, animSpeed);
            settingsDeliveryAddressFrame.IsVisible = false;
            await settingsPasswordFrame.TranslateTo(1000, 0, animSpeed);
            settingsPasswordFrame.IsVisible = false;
            settingsAccountFrame.IsVisible = true;
            await settingsAccountFrame.TranslateTo(0, 0, animSpeed);
            
            deliveryAddressButton.BackgroundColor = Color.Black;
            deliveryAddressButton.BorderColor = Color.White;
            deliveryAddressButton.TextColor = Color.White;

            contactInfoButton.BackgroundColor = Color.Black;
            contactInfoButton.BorderColor = Color.White;
            contactInfoButton.TextColor = Color.White;

            passwordButton.BackgroundColor = Color.Black;
            passwordButton.BorderColor = Color.White;
            passwordButton.TextColor = Color.White;

            accountButton.BackgroundColor = Color.White;
            accountButton.BorderColor = Color.Black;
            accountButton.TextColor = Color.Black;
        }

        public async void OnAddressUpdateClicked(object sender, EventArgs e)
        {
            _GlobalCustomer.Address1 = address1Text.Text;
            _GlobalCustomer.Address2 = address2Text.Text;
            _GlobalCustomer.Address3 = address3Text.Text;
            _GlobalCustomer.PostCode = postCodeText.Text;
            await Task.Run(() => UpdateCustomer(_GlobalCustomer));


            addressUpdateButton.BorderColor = Color.GreenYellow;
            addressUpdateButton.TextColor = Color.GreenYellow;

            address1Text.TextColor = Color.GreenYellow;
            address2Text.TextColor = Color.GreenYellow;
            address3Text.TextColor = Color.GreenYellow;
            postCodeText.TextColor = Color.GreenYellow;

            addressUpdateButton.Text = "UPDATED!";
        }

        public void OnContactUpdateClicked(object sender, EventArgs e)
        {
            _GlobalCustomer.Phone = phoneText.Text;
            UpdateCustomer(_GlobalCustomer);

            contactUpdateButton.BorderColor = Color.GreenYellow;
            contactUpdateButton.TextColor = Color.GreenYellow;

            phoneText.TextColor = Color.GreenYellow;

            contactUpdateButton.Text = "UPDATED!";
        }

        public void OnPasswordUpdateClicked(object sender, EventArgs e)
        {
            if(oldPasswordText.Text != _GlobalCustomer.Password)
            {
                oldPasswordText.BackgroundColor = Color.FromRgb(33, 0, 0);
                notMatchText.IsVisible = true;
                notMatchText.Text = "Wrong old password!";
            }
            else
            {
                oldPasswordText.BackgroundColor = Color.Transparent;
                if (newPasswordText.Text == null || newPasswordText.Text == String.Empty || newPasswordText.Text.Length < 6)
                {
                    newPasswordText.BackgroundColor = Color.FromRgb(33, 0, 0);
                    notMatchText.IsVisible = true;
                    notMatchText.Text = "Password needs to be at least 6 characters long!";
                }
                else
                {
                    newPasswordText.BackgroundColor = Color.Transparent;
                    if (newPasswordText.Text != newPasswordRetypedText.Text)
                    {
                        newPasswordText.BackgroundColor = Color.FromRgb(33, 0, 0);
                        newPasswordRetypedText.BackgroundColor = Color.FromRgb(33, 0, 0);
                        notMatchText.IsVisible = true;
                        notMatchText.Text = "Password do not match!";
                    }
                    else
                    {
                        newPasswordRetypedText.BackgroundColor = Color.Transparent;
                        _GlobalCustomer.Password = newPasswordRetypedText.Text;
                        notMatchText.IsVisible = false;
                        UpdateCustomer(_GlobalCustomer);
                        oldPasswordText.TextColor = Color.GreenYellow;
                        newPasswordText.TextColor = Color.GreenYellow;
                        newPasswordRetypedText.TextColor = Color.GreenYellow;
                        passwordUpdateButton.TextColor = Color.GreenYellow;
                        passwordUpdateButton.BorderColor = Color.GreenYellow;
                        passwordUpdateButton.Text = "UPDATED!";
                    }
                }
            }
        }

        public void OnAccountUpdateClicked(object sender, EventArgs e)
        {
            _GlobalCustomer.FirstName = firstNameText.Text;
            _GlobalCustomer.Surname = SurnameText.Text;
            UpdateCustomer(_GlobalCustomer);
            firstNameText.TextColor = Color.GreenYellow;
            SurnameText.TextColor = Color.GreenYellow;
            accountUpdateButton.TextColor = Color.GreenYellow;
            accountUpdateButton.BorderColor = Color.GreenYellow;
            accountUpdateButton.Text = "UPDATED!";
        }

        public async void OnBackClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<CustomerMobile.App>((CustomerMobile.App)Xamarin.Forms.Application.Current, "UpdateCustomer");
            await Navigation.PopModalAsync(true);
        }

        public async void OnLegalInfoClicked(object sender, EventArgs e)
        {

            using (UserDialogs.Instance.Loading("DOWNLOADING\nLEGAL INFO", null, null, true, MaskType.Black))
            {
                await DownloadDocsAsync();
                await Navigation.PushModalAsync(new TextReader("LEGAL INFO", "Terms & Conditions", "TEST CONTENT"));
            }
            
        }

        public async Task ActionAfterCheck()
        {
            if (_ResultOfCheckSum)
            {
                await DisplayAlert("CHECKSUM OK", "ITS VERIFIED!", "OK");
            }
            else
            {
                await DisplayAlert("CHECKSUM FAILED", "SHIT", "OK");
            }
        }
    }
}