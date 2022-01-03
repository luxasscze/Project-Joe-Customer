using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CustomerMobile.GlobalVar;
using static CustomerMobile.SQL;
using System.Threading;
using Acr.UserDialogs;

namespace CustomerMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyOrder : ContentPage
    {
        public string ItemType { get; set; }
        public int ItemIndex { get; set; }
        public string itemsString { get; set; }
        public Customer _Customer { get; set; }

        public Token stripeToken;
        public TokenService TokenService;

        public MyOrder(Customer customer)
        {
            InitializeComponent();
            _Customer = customer;
            bgImage.Source = _AppBackgroundImage;
            ListScrollView.Opacity = 0;
            ListScrollView.RotationX = 15;
            headerFrame.Rotation = 0;
            headerFrame.Scale = 0.7;
            headerFrame.Opacity = 0;
            //ListScrollView.FadeTo(1, 300);
            BindingContext = _CurrentItems;
            ListOfItems.ItemsSource = _CurrentItems;
            totalPriceText.Text = " TOTAL £" + _CurrentItems.Sum(item => item.TotalPrice).ToString() + " ";            
            if(_CurrentItems.Count == 0)
            {
                ListOfItems.IsVisible = false;
                noItemsFrame.IsVisible = true;
                placeOrderButton.IsVisible = false;
                
            }
            else
            {
                ListOfItems.IsVisible = true;
                noItemsFrame.IsVisible = false;
                placeOrderButton.IsVisible = true;
                
            }
            ListScrollView.FadeTo(0.7, 600);
            Task.WhenAll(
                headerFrame.RotateTo(5, 2000, Easing.CubicInOut),
                headerFrame.ScaleTo(1, 2000, Easing.CubicInOut),
                headerFrame.FadeTo(0.7, 2000),
                ListScrollView.RotateXTo(0, 1000, Easing.CubicInOut)
                );

        }

        public string CreatePaymentToken(Customer customer)
        {
            try
            {
                StripeConfiguration.ApiKey = "pk_test_8nV20N6D6mheY9RYiDCABVhL";
                var service = new ChargeService();
                var tokenOption = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = CCnumber.Text,
                        ExpYear = long.Parse(CCexpire.Text[2].ToString() + CCexpire.Text[3].ToString()),
                        ExpMonth = long.Parse(CCexpire.Text[0].ToString() + CCexpire.Text[1].ToString()),
                        Cvc = CCccv.Text,
                        Name = customer.Email
                    }
                };

                TokenService = new TokenService();
                stripeToken = TokenService.Create(tokenOption);

                return stripeToken.Id;
            }
            catch(StripeException ex)
            {
                DisplayAlert("NOT GOOD DATA", ex.ToString(), "OK");
                return "BULLSHIT";
            }
        }

        public bool MakePayment()
        {
            try
            {
                
                StripeConfiguration.ApiKey = "sk_test_51C3ZfrG8UHUWd2620xzUT8GkZ7cd5fkb1vu4ZtMv9z4NjLW4Q8fRgmK7cUv3RL6uwrndDzhE4gczEOc196u0GqRR00Ez60heg3";
                var options = new ChargeCreateOptions
                {
                    Amount = long.Parse(_CurrentItems.Sum(item => item.TotalPrice).ToString("0.00").Replace(".", "")),
                    Currency = "gbp",
                    Description = "Project Joe - Order #" + _ActiveOrder.OrderId,
                    StatementDescriptor = "Project Joe",
                    Capture = true,
                    ReceiptEmail = "info@lukasslivka.com",
                    Source = stripeToken.Id
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task PaymentAsync(Customer customer)
        {
            bool IsTransactionSuccess = false;

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            try
            {

                UserDialogs.Instance.ShowLoading("Payment proccessing...");
                await Task.Run(async () =>
                {
                    
                    var Token = CreatePaymentToken(customer);
                    if(Token != null)
                    {
                        IsTransactionSuccess = MakePayment();
                    }

                    
                });
               

                foreach (ItemsList outputString in _CurrentItems)
                {
                    itemsString = itemsString + outputString.ItemName + ";" + outputString.ItemQuantity + ";" + outputString.ItemPrice + ";";
                }
                NewOrder(itemsString, _CurrentItems.Sum(item => item.TotalPrice).ToString(), "0");
                _GlobalCustomer.HasActiveOrder = true;
                UpdateCustomer(_GlobalCustomer);
                MessagingCenter.Send<CustomerMobile.App>((CustomerMobile.App)Xamarin.Forms.Application.Current, "OrderPlaced");
                UserDialogs.Instance.HideLoading();
                await Navigation.PopModalAsync(true);
            }
            catch(StripeException ex)
            {
                await DisplayAlert("CC ERROR", ex.StripeError.ToString(), "OK");
                UserDialogs.Instance.HideLoading();
            }
        }


        public void ProccessPayment()
        {
            
            
            
            
            Task.WhenAll(
                ListScrollView.FadeTo(0, 200),
                placeOrderButton.FadeTo(0.1, 200)
                );
            ListScrollView.TranslationX = 1000;

            paymentFrame.IsVisible = true;

            
        }

        public async void OnPayClicked(object sender, EventArgs e)
        {

            if (CCnumber.Text != null || CCexpire.Text != null || CCccv.Text != null) // TODO: Continue here....
            {
                await PaymentAsync(_Customer);
            }
            else
            {
                await DisplayAlert("Invalid Card", "Please check your input...", "OK");
            }
            
        }

        public void OnPlaceOrderClicked(object sender, EventArgs e)
        {
            /*
            if (CCnumber.Text != null || CCexpire.Text != null || CCccv.Text != null)
            {
                ProccessPayment();
            }
            else
            {
                DisplayAlert("Invalid Card", "Please, check your card information...", "OK");
            }
            */
            
            try
            {
                ProccessPayment();
            }
            catch(StripeException se)
            {
                DisplayAlert("STRIPE ERROR", se.StripeError.ToString(), "OK");
            }
        }

        public async void OnItemSelected(object sender, ItemTappedEventArgs e)
        {


            removeFrameCaptionText.Text = _CurrentItems[e.ItemIndex].ItemName;
            removeFrameProductImage.Source = _CurrentItems[e.ItemIndex].ItemImage;
            removeFrameShortDescText.Text = _CurrentItems[e.ItemIndex].ItemShortDesc;
            ItemType = _CurrentItems[e.ItemIndex].ItemType;
            ItemIndex = e.ItemIndex;

            await Task.WhenAll(
                ListScrollView.FadeTo(0, 200),                
                placeOrderButton.FadeTo(0.1, 200)
                );
            ListScrollView.TranslationX = 1000;
            placeOrderButton.IsEnabled = false;
            removeFrame.IsVisible = true;
            await removeFrame.FadeTo(1, 400);             
                          
                          

        }

        public async void OnRemoveButtonClicked(object sender, EventArgs e)
        {
            _CurrentItems.RemoveAt(ItemIndex);          
                       
                      

            if(_CurrentItems.Count == 0)
            {
                noItemsFrame.Opacity = 0;
                await removeFrame.FadeTo(0, 400);
                removeFrame.IsVisible = false;
                await totalPriceText.ScaleTo(0.5, 200);
                totalPriceText.Text = " TOTAL £" + _CurrentItems.Sum(item => item.TotalPrice).ToString() + " ";
                await totalPriceText.ScaleTo(1, 200);                
                noItemsFrame.IsVisible = true;
                await noItemsFrame.FadeTo(1, 600);
                placeOrderButton.IsEnabled = false;
                placeOrderButton.IsVisible = false;
                
            }
            else
            {
                ListScrollView.TranslationX = 0;
                await removeFrame.FadeTo(0, 400);
                removeFrame.IsVisible = false;
                await Task.WhenAll(
                               
                ListScrollView.FadeTo(1, 400),
                placeOrderButton.FadeTo(0.7, 400),
                totalPriceText.ScaleTo(0.5, 200, Easing.CubicInOut),                
                totalPriceText.ScaleTo(1, 200, Easing.CubicInOut),
                totalPriceText.ScaleTo(0.5, 200)
                );               
                totalPriceText.Text = " TOTAL £" + _CurrentItems.Sum(item => item.TotalPrice).ToString() + " ";
                await totalPriceText.ScaleTo(1, 200);
                ListScrollView.IsVisible = true;
                noItemsFrame.IsVisible = false;
                placeOrderButton.IsEnabled = true;
                placeOrderButton.IsVisible = true;
            }
        }

        public async void OnRemoveFrameCloseClicked(object sender, EventArgs e)
        {
            await removeFrame.FadeTo(0, 400);
            removeFrame.IsVisible = false;
            placeOrderButton.IsEnabled = true;
            ListScrollView.TranslationX = 0;

            await Task.WhenAll(
                
                ListScrollView.FadeTo(1, 400),
                placeOrderButton.FadeTo(0.7, 400)
                );

            
        }

        public async void OnRemoveFrameInfoClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ProductDetail(removeFrameCaptionText.Text, ItemType, "Info"));
            
            await removeFrame.FadeTo(0, 400);
            removeFrame.IsVisible = false;
            placeOrderButton.IsEnabled = true;

            await Task.WhenAll(
                ListScrollView.TranslateTo(0, 0, 400),
                ListScrollView.FadeTo(1, 400),
                placeOrderButton.FadeTo(0.7, 400)
                );
            await totalPriceText.ScaleTo(0.5, 400, Easing.CubicInOut);
            totalPriceText.Text = " TOTAL £" + _CurrentItems.Sum(item => item.TotalPrice).ToString() + " ";
            await totalPriceText.ScaleTo(1, 200, Easing.CubicInOut);

        }
    }
}