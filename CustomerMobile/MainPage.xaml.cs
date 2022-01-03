using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static CustomerMobile.GlobalVar;
using static CustomerMobile.SQL;
using System.IO;
using System.Data.SqlClient;
using FluentFTP;
using static CustomerMobile.ExtraAnims;
using Acr.UserDialogs;
using System.Reflection;
using Xamarin.Forms.GoogleMaps;



namespace CustomerMobile
{
    public partial class MainPage : ContentPage
    {      
        public IList<Food> Foods { get; set; }
        public IList<Drink> Drinks { get; set; }
                
        Pin pinCustomer = null;
        Pin pinRestaurant = null;
        Pin pinOrder = null;

        public bool IsFood { get; set; }

        public Position customerPosition { get; set; }
        
        public int OrderScore { get; set; }       

        public MainPage()
        {
            InitializeComponent();
            AddMapStyle();
            MessagingCenter.Subscribe<App>((App)Application.Current, "UpdateCustomer", (sender) => 
            {
                companyCaptionText.Text = GetDayGreetings() + " " + _GlobalCustomer.FirstName + "?";
                subCaptionText.Text = "Delivery address: " + _GlobalCustomer.Address1 + ", " + _GlobalCustomer.Address2 + ", " + _GlobalCustomer.Address3 + ", " + _GlobalCustomer.PostCode;
            });

            MessagingCenter.Subscribe<App>((App)Application.Current, "OrderPlaced", (sender) =>
            {                
                StartOrderUpdating();
            });

            headerFrame.IsVisible = true;
            foodButton.BackgroundColor = Color.White;
            foodButton.TextColor = Color.Black;
            hideButton.Text = "▲\nHIDE";
            drinkButton.BackgroundColor = Color.Transparent;
            drinkButton.TextColor = Color.White;
            companyCaptionText.Text = _CompanyName;
            subCaptionText.Text = _SubCaption;
            iconImage.Source = _AppIcon64;
           
            backgroundImage.Source = _AppBackgroundImage;
            
            
            _IsLogged = false;
            IsFood = true;
            //File.Delete(_ConfigFile);
            if(File.Exists(_ConfigFile))
            {

            }
            else
            {
                File.WriteAllText(_ConfigFile, "True;;;");
            }
            

            if(File.Exists(_ConfigFile))
            {
                
                
                _AutoLogin = bool.Parse(File.ReadAllText(_ConfigFile).Split(';')[0]);

                if(_AutoLogin)
                {
                    emailText.Text = File.ReadAllText(_ConfigFile).Split(';')[1];
                    passwordText.Text = File.ReadAllText(_ConfigFile).Split(';')[2];
                }
                else
                {

                }
            }
            else
            {

            }
        }

        /// <summary>
        /// Start order updating every 3 seconds
        /// </summary>
        public void StartOrderUpdating()
        {
            //SHOWING ORDER PROGRESS WINDOW
            MenuFrame.IsVisible = false;
            thanksFrame.IsVisible = false;
            _ActiveOrder = LoadActiveOrder(_GlobalCustomer.Email);
            _IsMapUpdating = true;            
            activeOrderFrame.IsVisible = true;            
            orderIdText.Text = "Order #" + _ActiveOrder.OrderId;
            orderCreatedText.Text = "Last change: " + _ActiveOrder.OrderChanged.ToString("HH:mm:ss");
            orderTotalText.Text = "Total: £" + _ActiveOrder.OrderPrice;
            loginFrame.IsVisible = false;            
            TierFrame.IsVisible = false;       
            logoutButton.IsVisible = true;
            settingsButton.IsVisible = false;
            hideButton.IsVisible = true;
           
            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                _ActiveOrder = LoadActiveOrder(_GlobalCustomer.Email);             


                if (_ActiveOrder.OrderStatus == "NewOrder")
                {
                    orderStatusText.Text = "Waiting for restaurant to accept your order...";
                    orderCreatedText.Text = "Last change: " + _ActiveOrder.OrderChanged.ToString("HH:mm:ss");
                    orderProgressImage.Source = "OrderProgress1.png";
                    etaText.Text = "--:--";
                    trackMap.Pins.Clear();
                    trackMap.Pins.Add(pinCustomer);
                    GetPosition("Customer");
                    
                }
                else if (_ActiveOrder.OrderStatus == "InTheKitchen")
                {
                    orderStatusText.Text = "Restaurant is preparing your order...";
                    orderCreatedText.Text = "Last change: " + _ActiveOrder.OrderChanged.ToString("HH:mm:ss");

                    etaText.Text = _ActiveOrder.OrderEta;
                    orderProgressImage.Source = "OrderProgress2.png";
                    trackMap.Pins.Clear();
                    trackMap.Pins.Add(pinRestaurant);                    
                    GetPosition("Restaurant");
                }
                else if (_ActiveOrder.OrderStatus == "ReadyForDelivery")
                {
                    orderStatusText.Text = "Waithing for pick up...";
                    orderCreatedText.Text = "Last change: " + _ActiveOrder.OrderChanged.ToString("HH:mm:ss");
                    etaText.Text = _ActiveOrder.OrderEta;
                    orderProgressImage.Source = "OrderProgress3.png";
                    trackMap.Pins.Clear();
                    trackMap.Pins.Add(pinRestaurant);                    
                    GetPosition("Restaurant");
                }
                else if (_ActiveOrder.OrderStatus == "OnTheWay")
                {
                    orderStatusText.Text = "Your order is on the way...";
                    orderCreatedText.Text = "Last change: " + _ActiveOrder.OrderChanged.ToString("HH:mm:ss");
                    etaText.Text = _ActiveOrder.OrderEta;
                    orderProgressImage.Source = "OrderProgress4.png";                    
                    trackMap.Pins.Clear();
                    pinOrder.Position = new Position(_ActiveOrder.OrderLat, _ActiveOrder.OrderLon);
                    trackMap.Pins.Add(pinOrder);
                    trackMap.Pins.Add(pinCustomer);
                    AdjustMap(pinCustomer, pinOrder);
                    pinOrder.Address = "#" + _ActiveOrder.OrderId;
                    


                }
                else if (_ActiveOrder.OrderStatus == "Finished")
                {
                    activeOrderFrame.IsVisible = false;
                    thanksFrame.Opacity = 0;
                    thanksFrame.IsVisible = true;
                    thanksFrame.FadeTo(0.7, 600);
                    orderStatusText.Text = "Your order has been delivered";
                    orderCreatedText.Text = "Last change: " + _ActiveOrder.OrderChanged.ToString("HH:mm:ss");
                    etaText.Text = "ENJOY YOUR FOOD :)";
                    orderProgressImage.Source = "OrderProgress4.png";                    
                    _CurrentItems.Clear();
                    _CurrentOrder.Clear();
                    _IsMapUpdating = false;
                    UpdateCustomerHasActiveOrder(_GlobalCustomer.Email, "False");
                    LoadProducts();                                   
                    UpdateOrderIsActive(_ActiveOrder.OrderId, "False");
                    trackMap.Pins.Clear();
                   
                    


                }
                if (_IsMapUpdating)
                {
                    return true;
                }
                else
                {
                    
                    return false;

                }
            });
        }

        void AddMapStyle()
        {
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream($"CustomerMobile.MapStyle.json");
            string styleFile;
            using (var reader = new System.IO.StreamReader(stream))
            {
                styleFile = reader.ReadToEnd();
            }

            trackMap.MapStyle = MapStyle.FromJson(styleFile);
            trackMap.UiSettings.ZoomControlsEnabled = false;
            trackMap.UiSettings.ZoomGesturesEnabled = false;          

            
        }

        public async void SetMapPins()
        {

            var geocoder = new Xamarin.Forms.GoogleMaps.Geocoder();
            var positions = await geocoder.GetPositionsForAddressAsync(_GlobalCustomer.Address1 + ", " + _GlobalCustomer.Address2);
            
            pinCustomer = new Pin()
            {
                Type = PinType.SavedPin,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.DarkRed),
                Label = _GlobalCustomer.FirstName,
                Address = _GlobalCustomer.Address1 + ", " + _GlobalCustomer.Address2,
                Position = positions.First(),
                Rotation = 33.3f,
                Tag = "id_customer",
                IsVisible = true,
                IsDraggable = true

            };

            pinRestaurant = new Pin()
            {
                Type = PinType.SavedPin,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.DarkOrange),
                Label = "FOR YOUR SOUL",
                Address = "162 Haydn Road, Nottingham",
                Position = _RestaurantPosition,
                Rotation = 33.3f,
                Tag = "id_restaurant",
                IsVisible = true

            };

            pinOrder = new Pin()
            {
                Type = PinType.SavedPin,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.CadetBlue),
                Label = "YOUR ORDER",
                Address = "***ETA: ***",
                Position = _RestaurantPosition,
                Rotation = 33.3f,
                Tag = "id_order",
                IsVisible = true,                
            };                       
        }

        

        public async Task GetPosition(string type)
        {
            var geocoder = new Xamarin.Forms.GoogleMaps.Geocoder();
            var positions = await geocoder.GetPositionsForAddressAsync(_GlobalCustomer.Address1 + ", " + _GlobalCustomer.Address2);
            if (positions.Count() > 0)
            {
                _CustomerPosition = positions.First();
                if (type == "Customer")
                {
                    await trackMap.MoveCamera(CameraUpdateFactory.NewCameraPosition(new CameraPosition(_CustomerPosition, 17d)));
                }
                else if(type == "Restaurant")
                {
                    await trackMap.MoveCamera(CameraUpdateFactory.NewCameraPosition(new CameraPosition(pinRestaurant.Position, 17d)));
                }      
                             
                             

            }
            else
            {
                await DisplayAlert("NO POSITION", "THERE IS NO POSITION", "OK");
            }
        }



        protected override void OnAppearing()
        {
           
            base.OnAppearing();
            if(_IsLogged)
            {



            }
            else
            {
               
            }
           
        }

        

        public void LoadProducts()
        {
            Foods = new List<Food>();
            Drinks = new List<Drink>();

            using(SqlConnection connection = new SqlConnection(_ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "SELECT * FROM Products WHERE Type='Food' AND TimeFrom<'" + DateTime.Now.ToString("HH:mm") + "' AND TimeTo>'" + DateTime.Now.ToString("HH:mm") + "' AND IsAvailable='True' ORDER BY Category, Name ASC",
                    Connection = connection
                };

                SqlDataReader reader = cmd.ExecuteReader();
                Foods.Clear();
                while (reader.Read())
                {
                    Foods.Add(new Food
                    {
                        Allergens = reader["Allergens"].ToString().Split(';'),
                        AmountSold = int.Parse(reader["AmountSold"].ToString()),
                        Category = reader["Category"].ToString(),
                        Image = _UriPath + reader["Image"].ToString(),
                        IsAvailable = bool.Parse(reader["IsAvailable"].ToString()),
                        IsVegan = bool.Parse(reader["IsVegan"].ToString()),
                        IsVegetarian = bool.Parse(reader["IsVegetarian"].ToString()),
                        LastSold = reader["LastSold"].ToString(),
                        LongDescription = reader["LongDescription"].ToString(),
                        Name = reader["Name"].ToString(),
                        Remarks = reader["Remarks"].ToString(),
                        SellPrice = decimal.Parse(reader["SellPrice"].ToString()),
                        ShortDescription = reader["ShortDescription"].ToString(),
                        TimeFrom = reader["TimeFrom"].ToString(),
                        TimeTo = reader["TimeTo"].ToString(),
                        Type = reader["Type"].ToString(),
                        DisplayPrice = "£" + reader["SellPrice"].ToString()
                    });
                }
                connection.Close();
            }

            using (SqlConnection connection = new SqlConnection(_ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "SELECT * FROM Products WHERE Type='Drink' AND TimeFrom<'" + DateTime.Now.ToString("HH:mm") + "' AND TimeTo>'" + DateTime.Now.ToString("HH:mm") + "' AND IsAvailable='True' ORDER BY Category, Name ASC",
                    Connection = connection
                };

                SqlDataReader reader = cmd.ExecuteReader();
                Drinks.Clear();
                while (reader.Read())
                {
                    Drinks.Add(new Drink
                    {
                        Allergens = reader["Allergens"].ToString().Split(';'),
                        AmountSold = int.Parse(reader["AmountSold"].ToString()),
                        Category = reader["Category"].ToString(),
                        Image = _UriPath + reader["Image"].ToString(),
                        IsAvailable = bool.Parse(reader["IsAvailable"].ToString()),
                        IsVegan = bool.Parse(reader["IsVegan"].ToString()),
                        IsVegetarian = bool.Parse(reader["IsVegetarian"].ToString()),
                        LastSold = reader["LastSold"].ToString(),
                        LongDescription = reader["LongDescription"].ToString(),
                        Name = reader["Name"].ToString(),
                        Remarks = reader["Remarks"].ToString(),
                        SellPrice = decimal.Parse(reader["SellPrice"].ToString()),
                        ShortDescription = reader["ShortDescription"].ToString(),
                        TimeFrom = reader["TimeFrom"].ToString(),
                        TimeTo = reader["TimeTo"].ToString(),
                        Type = reader["Type"].ToString(),
                        DisplayPrice = "£" + reader["SellPrice"].ToString()
                    });
                }
                connection.Close();
            }

            BindingContext = this;
        }




        public void DisplayStatusInfo(string status)
        {
            if(status == "Active")
            {
                MenuFrame.IsVisible = true;
                InfoFrame.IsVisible = false;
            }
            else if(status == "Inactive")
            {
                MenuFrame.IsVisible = false;
                InfoFrame.IsVisible = true;
                infoText.TextColor = Color.White;
                infoText.Text = "YOUR ACCOUNT IS INACTIVE!";
            }
            else if(status == "Suspended")
            {
                MenuFrame.IsVisible = false;
                InfoFrame.IsVisible = true;
                infoText.TextColor = Color.Orange;
                infoText.Text = "YOUR ACCOUNT IS SUSPENDED!";
            }
            else if(status == "Banned")
            {
                MenuFrame.IsVisible = false;
                InfoFrame.IsVisible = true;
                infoText.TextColor = Color.IndianRed;
                infoText.Text = "YOUR ACCOUNT HAS BEEN BANNED!";
            }
        }


        public async void OnFacebookLoginClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Read from file", File.ReadAllText(_ConfigFile), "OK");
        }

        public void SetTierAndStatus(Customer customer)
        {
            if (customer.Status == "Active")
            {
                tierProgressBar.ProgressColor = Color.GreenYellow;
            }
            else if (customer.Status == "Inactive")
            {
                tierProgressBar.ProgressColor = Color.White;
            }
            else if (customer.Status == "Suspended")
            {
                tierProgressBar.ProgressColor = Color.Orange;
            }
            else if (customer.Status == "Banned")
            {
                tierProgressBar.ProgressColor = Color.IndianRed;
            }


            if (customer.Tier == 0)
            {
                tierProgressBar.Progress = customer.Orders / 10;
                tierDescText.Text = "You have entry tier level. Tear up for more personal offers.";
            }
            else if (customer.Tier == 1)
            {
                double realProgress = (customer.Orders - 10);
                tierProgressBar.Progress = realProgress / 10;
                tierDescText.Text = "Tier 1 provides you with free delivery up to 10km.";
            }
            else if (customer.Tier == 2)
            {
                double realProgress = (customer.Orders - 20);
                tierProgressBar.Progress = realProgress / 10;
                tierDescText.Text = "Tier 2 gives you 5% discount on everything in our menu.";

            }
            else if (customer.Tier == 3)
            {
                double realProgress = (customer.Orders - 30);
                tierProgressBar.Progress = realProgress / 10;
                tierDescText.Text = "Tier 3 means 5% discount and free delivery. Enjoy!";
            }
            else if (customer.Tier == 4)
            {
                double realProgress = (customer.Orders - 40);
                tierProgressBar.Progress = realProgress / 10;
                tierDescText.Text = "Tier 4 gives you one extra side for free!";
            }
            else if (customer.Tier == 5)
            {
                if (customer.Orders > 50)
                {
                    tierText.Text = "Tier MAX -- " + customer.Orders + " Orders";
                    tierProgressBar.Progress = 1;
                    tierDescText.Text = "Tier 5 means free delivery and 15% discount. Forever!";

                }
                else
                {
                    double realProgress = (customer.Orders - 50);
                    tierProgressBar.Progress = realProgress / 10;
                }
            }
        }

        public async Task LoadCustomerAsync()
        {
            await Task.Run(() =>
            {
                _GlobalCustomer = LoadCustomer(emailText.Text);
                
            });
        }

       

        public async void OnLoginClicked(object sender, EventArgs e)
        {

            using (UserDialogs.Instance.Loading("LOADING", null, null, true, MaskType.Black))
            {
                await CheckUserLogin(emailText.Text, passwordText.Text);
                try
                {
                    
                    if (_CheckCustomerResult)
                    {   
                        await LoadCustomerAsync();
                        SetMapPins();
                        if (!_GlobalCustomer.HasActiveOrder)
                        {
                            LoadProducts();
                            //await DisplayAlert("TEST", Foods[3].SellPrice.ToString("0.00"), "OK");

                            companyCaptionText.FontSize = 24;
                            companyCaptionText.Text = GetDayGreetings() + " " + _GlobalCustomer.FirstName + "?";
                            subCaptionText.Text = "Delivery address: " + _GlobalCustomer.Address1 + ", " + _GlobalCustomer.Address2 + ", " + _GlobalCustomer.Address3 + ", " + _GlobalCustomer.PostCode;
                            loginFrame.IsVisible = false;
                            DisplayStatusInfo(_GlobalCustomer.Status);
                            TierFrame.IsVisible = false;
                            TierFrame.TranslationY = 100;
                            showMenuButton.TranslationY = -100;

                            logoutButton.IsVisible = true;
                            settingsButton.IsVisible = true;
                            
                            hideButton.IsVisible = true;
                            tierText.Text = "Tier " + _GlobalCustomer.Tier + " -- " + _GlobalCustomer.Orders + " Orders";


                            SetTierAndStatus(_GlobalCustomer);

                            await TierFrame.TranslateTo(0, 0, 500);

                            if (keepMeLoggedSwitch.IsToggled)
                            {

                                File.WriteAllText(_ConfigFile, "True;" + emailText.Text + ";" + passwordText.Text + ";");

                            }
                            else
                            {
                                File.WriteAllText(_ConfigFile, "False;" + emailText.Text + ";" + passwordText.Text + ";");

                            }
                        }
                        else
                        {

                            
                            StartOrderUpdating();                           
                        }
                    }
                    else
                    {
                        await DisplayAlert("USER", "WRONG USER OR PASSWORD", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Expception...", ex.ToString(), "OK");
                }
            }
        }

        

        public async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Register());
        }

        public void OnLogoutClicked(object sender, EventArgs e)
        {
            _IsLogged = false;
            _IsMapUpdating = false;
            trackMap.Pins.Clear();
            MenuFrame.IsVisible = false;

            activeOrderFrame.IsVisible = false;
            InfoFrame.IsVisible = false;
            TierFrame.IsVisible = false;
            loginFrame.IsVisible = true;
            settingsButton.IsVisible = false;
            logoutButton.IsVisible = false;
            hideButton.IsVisible = false;
            _CurrentItems.Clear();
            _CurrentOrder.Clear();

            if(keepMeLoggedSwitch.IsToggled)
            {

            }
            else
            {
                emailText.Text = String.Empty;
                passwordText.Text = String.Empty;
            }
            

            companyCaptionText.FontSize = 36;
            companyCaptionText.Text = _CompanyName;
            subCaptionText.FontSize = 14;
            subCaptionText.Text = _SubCaption;
        }

        public async void OnSettingsClicked(object sender, EventArgs e)
        {            
            await Navigation.PushModalAsync(new Settings());             
        }

        public async void OnActivateAccountClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ActivateAccount());           
            
        }

        public void OnFoodClicked(object sender, EventArgs e)
        {
            IsFood = true;
            menuList.ItemsSource = Foods;
            foodButton.BackgroundColor = Color.CadetBlue;
            foodButton.TextColor = Color.Black;

            drinkButton.BackgroundColor = Color.Transparent;
            drinkButton.TextColor = Color.White;
        }

        public void OnDrinkClicked(object sender, EventArgs e)
        {
            IsFood = false;
            menuList.ItemsSource = Drinks;
            foodButton.BackgroundColor = Color.Transparent;
            foodButton.TextColor = Color.White;

            drinkButton.BackgroundColor = Color.FromRgb(115, 76, 159);
            drinkButton.TextColor = Color.Black;
        }

        public async void OnMenuItemSelected(object sender, ItemTappedEventArgs e)
        {

            if (IsFood)
            {
                Food selectedProduct = e.Item as Food;
                await Navigation.PushModalAsync(new ProductDetail(selectedProduct.ToString(), "Food", "Normal"));
            }
            else
            {
                Drink selectedProduct = e.Item as Drink;
                await Navigation.PushModalAsync(new ProductDetail(selectedProduct.ToString(), "Drink", "Normal"));
            }
        }

        public async void OnMyOrderClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MyOrder(_GlobalCustomer));
        }

        public void AdjustMap(Pin customer, Pin restaurant)
        {

            //MAIN ADRRESS: 162 Haydn Road, Nottingham

            //SCENARIO 1: 34 Hazelwood Rd, Nottingham
            //SCENARIO 2: 75 Britannia Ave, Nottingham
            //SCENARIO 3: 402A Woodborough Rd, Nottingham
            //SCENARIO 4: 2 Brookdale Ct, Nottingham

            if (customer.Position.Latitude < restaurant.Position.Latitude && customer.Position.Longitude < restaurant.Position.Longitude) // SCENARIO 1
            {
                Position southwestBound = new Position(customer.Position.Latitude - 0.003, customer.Position.Longitude - 0.006);
                Position northeastBound = new Position(restaurant.Position.Latitude + 0.003, restaurant.Position.Longitude + 0.006);
                var bounds = new Bounds(southwestBound, northeastBound);
                trackMap.MoveToRegion(MapSpan.FromBounds(bounds));
                //DisplayAlert("SET BOUNDS", "SCENARIO 1", "OK");
            }
            else if (customer.Position.Latitude > restaurant.Position.Latitude && customer.Position.Longitude < restaurant.Position.Longitude) // SCENARIO 2
            {
                Position southwestBound = new Position(customer.Position.Latitude + 0.003, customer.Position.Longitude - 0.006);
                Position northeastBound = new Position(restaurant.Position.Latitude - 0.003, restaurant.Position.Longitude + 0.006);
                var bounds = new Bounds(northeastBound, southwestBound);
                trackMap.MoveToRegion(MapSpan.FromBounds(bounds));
                //DisplayAlert("SET BOUNDS", "SCENARIO 2", "OK");
            }
            else if (customer.Position.Latitude < restaurant.Position.Latitude && customer.Position.Longitude > restaurant.Position.Longitude) // SCENARIO 3
            {
                Position southwestBound = new Position(customer.Position.Latitude - 0.004, customer.Position.Longitude + 0.003);
                Position northeastBound = new Position(restaurant.Position.Latitude + 0.004, restaurant.Position.Longitude - 0.003);
                var bounds = new Bounds(southwestBound, northeastBound);
                trackMap.MoveToRegion(MapSpan.FromBounds(bounds));
                //DisplayAlert("SET BOUNDS", "SCENARIO 3", "OK");
            }
            else if (customer.Position.Latitude > restaurant.Position.Latitude && customer.Position.Longitude > restaurant.Position.Longitude) // SCENARIO 4
            {
                Position southwestBound = new Position(customer.Position.Latitude + 0.003, customer.Position.Longitude + 0.006);
                Position northeastBound = new Position(restaurant.Position.Latitude - 0.003, restaurant.Position.Longitude - 0.006);
                var bounds = new Bounds(southwestBound, northeastBound);
                trackMap.MoveToRegion(MapSpan.FromBounds(bounds));
                //DisplayAlert("SET BOUNDS", "SCENARIO 4", "OK");
            }

        }

        public async void OnHideClicked(object sender, EventArgs e)
        {
            _OriginalHeight = MenuFrame.Height;
            showMenuButton.IsVisible = true;        

            
            menuList.MinimumHeightRequest = mainLayout.Height - 200;
            await Task.WhenAll(           
            headerFrame.TranslateTo(0, -500, 600, Easing.CubicInOut),            
            menuList.RotateXTo(0, 600),
            showMenuButton.TranslateTo(0, 0, 800, Easing.CubicInOut)          
            );            
            headerFrame.IsVisible = false;           
        }

        public async void OnShowMenuClicked(object sender, EventArgs e)
        {
            menuList.MinimumHeightRequest = _OriginalHeight;
            headerFrame.IsVisible = true;                     
            await showMenuButton.TranslateTo(0, -100, 600, Easing.CubicInOut);
            await Task.WhenAll(
            headerFrame.TranslateTo(0, 0, 600, Easing.CubicInOut),
            MenuFrame.TranslateTo(0, 0, 600, Easing.CubicInOut),            
            menuList.RotateXTo(5, 600)         
            );
            showMenuButton.IsVisible = false;        
        }

        public async void OnThanksMenuClicked(object sender, EventArgs e)
        {
           
            TierFrame.IsVisible = false;
            UpdateOrder(_ActiveOrder.OrderId, "Score", OrderScore.ToString());
            UpdateOrder(_ActiveOrder.OrderId, "ScoreText", scoreText.Text);            
            await thanksFrame.FadeTo(0, 1000);
            thanksFrame.IsVisible = false;

            scoreText.Text = String.Empty;
            star1Button.Source = "Star_BW.png";
            star2Button.Source = "Star_BW.png";
            star3Button.Source = "Star_BW.png";
            star4Button.Source = "Star_BW.png";
            star5Button.Source = "Star_BW.png";
            OrderScore = 0;

            activeOrderFrame.IsVisible = false;
            MenuFrame.Opacity = 0;
            MenuFrame.IsVisible = true;
            await MenuFrame.FadeTo(0.7, 2000);
            settingsButton.IsVisible = true;
            settingsButton.Opacity = 0;
            await settingsButton.FadeTo(1, 600);
        }

        public void OnStar1Clicked(object sender, EventArgs e)
        {
            star1Button.Source = "Star_Color.png";
            star2Button.Source = "Star_BW.png";
            star3Button.Source = "Star_BW.png";
            star4Button.Source = "Star_BW.png";
            star5Button.Source = "Star_BW.png";
            OrderScore = 1;
        }

        public void OnStar2Clicked(object sender, EventArgs e)
        {
            star1Button.Source = "Star_Color.png";
            star2Button.Source = "Star_Color.png";
            star3Button.Source = "Star_BW.png";
            star4Button.Source = "Star_BW.png";
            star5Button.Source = "Star_BW.png";
            OrderScore = 2;
        }

        public void OnStar3Clicked(object sender, EventArgs e)
        {
            star1Button.Source = "Star_Color.png";
            star2Button.Source = "Star_Color.png";
            star3Button.Source = "Star_Color.png";
            star4Button.Source = "Star_BW.png";
            star5Button.Source = "Star_BW.png";
            OrderScore = 3;
        }

        public void OnStar4Clicked(object sender, EventArgs e)
        {
            star1Button.Source = "Star_Color.png";
            star2Button.Source = "Star_Color.png";
            star3Button.Source = "Star_Color.png";
            star4Button.Source = "Star_Color.png";
            star5Button.Source = "Star_BW.png";
            OrderScore = 4;
        }

        public void OnStar5Clicked(object sender, EventArgs e)
        {
            star1Button.Source = "Star_Color.png";
            star2Button.Source = "Star_Color.png";
            star3Button.Source = "Star_Color.png";
            star4Button.Source = "Star_Color.png";
            star5Button.Source = "Star_Color.png";
            OrderScore = 5;
        }        
    }
}
