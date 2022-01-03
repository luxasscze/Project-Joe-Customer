using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CustomerMobile.GlobalVar;
using System.Data.SqlClient;

namespace CustomerMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductDetail : ContentPage
    {
        Food foodDetail = new Food();
        Drink drinkDetail = new Drink();

        
        public bool IsFood { get; set; }



        

        /// <summary>
        /// Product Detail Page VISIBILITY Normal OR Info
        /// </summary>
        /// <param name="productName"> Product Name String</param>
        /// <param name="type"> Type Food or Drink</param>
        /// <param name="visibility">Visibility Normal = With add button Info = Without add button</param>
        public ProductDetail(string productName, string type, string visibility)
        {
            InitializeComponent();
            
            shortDescriptionText.TranslationX = -1000;
            longDescriptionText.TranslationX = 1000;
            isVegetarianText.TranslationX = -1000;
            isVeganText.TranslationX = 1000;
            headerFrame.TranslationY = -1000;
            allergensText.TranslationX = -200;
            allergensButton.Text = " ALLERGENS\n˅";

            if(visibility == "Info")
            {
                addToOrderButton.IsVisible = false;
                plusButton.IsVisible = false;
                minusButton.IsVisible = false;
                closeButton.IsVisible = true;
            }
            else if(visibility == "Normal")
            {
                addToOrderButton.IsVisible = true;
                plusButton.IsVisible = true;
                minusButton.IsVisible = true;
                closeButton.IsVisible = false;
            }

            if(type == "Food")
            {
                IsFood = true;
                foodDetail.Quantity = 1;
                LoadFood(productName);
                
            }
            else if(type == "Drink")
            {
                IsFood = false;
                drinkDetail.Quantity = 1;
                LoadDrink(productName);
               
            }
            productText.Text = productName;            
            bgImage.Source = _AppBackgroundImage;
            headerFrame.TranslateTo(0, 0, 600);
            shortDescriptionText.TranslateTo(0, 0, 600);
            longDescriptionText.TranslateTo(0, 0, 600);
            isVegetarianText.TranslateTo(0, 0, 600);
            isVeganText.TranslateTo(0, 0, 600);
            
            if (IsFood)
            {
                if (foodDetail.IsVegetarian)
                {
                    isVegetarianText.IsVisible = true;
                }
                else
                {
                    isVegetarianText.IsVisible = false;
                }
                if (foodDetail.IsVegan)
                {
                    isVeganText.IsVisible = true;
                }
                else
                {
                    isVeganText.IsVisible = false;
                }
            }
            else
            {
                if (drinkDetail.IsVegetarian)
                {
                    isVegetarianText.IsVisible = true;
                }
                else
                {
                    isVegetarianText.IsVisible = false;
                }
                if (drinkDetail.IsVegan)
                {
                    isVeganText.IsVisible = true;
                }
                else
                {
                    isVeganText.IsVisible = false;
                }
            }


        }

        public async void OnAddToOrderClicked(object sender, EventArgs e)
        {
            if (IsFood)
            {
                _CurrentItems.Add(new ItemsList()
                {
                    ItemName = foodDetail.Name,
                    ItemPrice = decimal.Parse(foodDetail.SellPrice.ToString("0.00")),
                    TotalPrice = (foodDetail.SellPrice * foodDetail.Quantity),
                    ItemQuantity = foodDetail.Quantity,
                    ItemId = foodDetail.Id,
                    ItemImage = _UriPath + foodDetail.Image,
                    ItemShortDesc = foodDetail.ShortDescription,
                    ItemType = "Food"
                });
            }
            else
            {
                _CurrentItems.Add(new ItemsList()
                {
                    ItemName = drinkDetail.Name,
                    ItemPrice = drinkDetail.SellPrice,
                    TotalPrice = (drinkDetail.SellPrice * drinkDetail.Quantity),
                    ItemQuantity = drinkDetail.Quantity,
                    ItemId = drinkDetail.Id,
                    ItemImage = _UriPath + drinkDetail.Image,
                    ItemShortDesc = drinkDetail.ShortDescription,
                    ItemType = "Drink"
                });
            }

            await Navigation.PopModalAsync(true);
            
        }

        public void LoadDrink(string name)
        {
            using (SqlConnection connection = new SqlConnection(_ConnectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "SELECT * FROM Products WHERE Name='" + name + "'",

                    Connection = connection
                };
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    drinkDetail.Allergens = reader["Allergens"].ToString().Split(';');
                    drinkDetail.AmountSold = int.Parse(reader["AmountSold"].ToString());
                    drinkDetail.Category = reader["Category"].ToString();
                    drinkDetail.Image = reader["Image"].ToString();
                    drinkDetail.IsVegan = bool.Parse(reader["IsVegan"].ToString());
                    drinkDetail.IsVegetarian = bool.Parse(reader["IsVegetarian"].ToString());
                    drinkDetail.LastSold = reader["LastSold"].ToString();
                    drinkDetail.LongDescription = reader["LongDescription"].ToString();
                    drinkDetail.Name = reader["Name"].ToString();
                    drinkDetail.Remarks = reader["Remarks"].ToString();
                    drinkDetail.SellPrice = decimal.Parse(reader["SellPrice"].ToString());
                    drinkDetail.ShortDescription = reader["ShortDescription"].ToString();
                    drinkDetail.Id = int.Parse(reader["Id"].ToString());


                    
                }
                connection.Close();
                productImage.Source = _UriPath + drinkDetail.Image;
                categoryText.Text = drinkDetail.Category;
                priceText.Text = " £" + drinkDetail.SellPrice + " ";
                shortDescriptionText.Text = "\t\t" + drinkDetail.ShortDescription + "\t\t";
                longDescriptionText.Text = drinkDetail.LongDescription;
                addToOrderButton.Text = "Add to order\n1x    £" + drinkDetail.SellPrice;
               
                
                    
                    string allAllergens = String.Join(";", drinkDetail.Allergens.Select(p => p.ToString()).ToArray());
                    allergensText.Text = "\nAllergens:\n\n " + allAllergens.Replace(";", "\n");
                

            }
        }

        public void LoadFood(string name)
        {
            using (SqlConnection connection = new SqlConnection(_ConnectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "SELECT * FROM Products WHERE Name='" + name + "'",

                    Connection = connection
                };
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    foodDetail.Allergens = reader["Allergens"].ToString().Split(';');
                    foodDetail.AmountSold = int.Parse(reader["AmountSold"].ToString());
                    foodDetail.Category = reader["Category"].ToString();
                    foodDetail.Image = reader["Image"].ToString();
                    foodDetail.IsVegan = bool.Parse(reader["IsVegan"].ToString());
                    foodDetail.IsVegetarian = bool.Parse(reader["IsVegetarian"].ToString());
                    foodDetail.LastSold = reader["LastSold"].ToString();
                    foodDetail.LongDescription = reader["LongDescription"].ToString();
                    foodDetail.Name = reader["Name"].ToString();
                    foodDetail.Remarks = reader["Remarks"].ToString();
                    foodDetail.SellPrice = decimal.Parse(reader["SellPrice"].ToString());
                    foodDetail.ShortDescription = reader["ShortDescription"].ToString();
                    foodDetail.Id = int.Parse(reader["Id"].ToString());

                }
                connection.Close();
                productImage.Source = _UriPath + foodDetail.Image;
                categoryText.Text = foodDetail.Category;
                priceText.Text = " £" + foodDetail.SellPrice + " ";
                shortDescriptionText.Text = "\t\t" + foodDetail.ShortDescription + "\t\t";
                longDescriptionText.Text = foodDetail.LongDescription;
                addToOrderButton.Text = "Add to order\n1x    £" + foodDetail.SellPrice;
               
                   
                    string allAllergens = String.Join(";", foodDetail.Allergens.Select(p => p.ToString()).ToArray());
                    allergensText.Text = "\nAllergens:\n\n " + allAllergens.Replace(";", "\n");
                


            }
        }

        public void OnPlusButtonClicked(object sender, EventArgs e)
        {

            AnimateHeader();
            if (IsFood)
            {
                foodDetail.Quantity += 1;
                addToOrderButton.Text = "Add to order\n" + foodDetail.Quantity + "x    £" + (foodDetail.Quantity * foodDetail.SellPrice);
            }
            else
            {
                drinkDetail.Quantity += 1;
                addToOrderButton.Text = "Add to order\n" + drinkDetail.Quantity + "x    £" + (drinkDetail.Quantity * drinkDetail.SellPrice);
            }
        }

        public void OnMinusButtonClicked(object sender, EventArgs e)
        {

            AnimateHeader();
                if (IsFood)
                {
                    if(foodDetail.Quantity == 1)
                    {

                    }
                    else
                    {
                        foodDetail.Quantity -= 1;
                        addToOrderButton.Text = "Add to order\n" + foodDetail.Quantity + "x    £" + (foodDetail.Quantity * foodDetail.SellPrice);
                    }
                    
                }
                else
                {
                if (drinkDetail.Quantity == 1)
                {

                }
                else
                {
                    drinkDetail.Quantity -= 1;
                    addToOrderButton.Text = "Add to order\n" + drinkDetail.Quantity + "x    £" + (drinkDetail.Quantity * drinkDetail.SellPrice);
                }
                }
           
        }

        public void OnAllergensClicked(object sender, EventArgs e)
        {
            AnimateHeader();
            allergensText.IsVisible = true;
            allergensText.TranslateTo(0, 0, 400);
            allergensButton.TranslateTo(-300, 0, 400);
        }

        public void OnAllergensListClicked(object sender, EventArgs e)
        {
            AnimateHeader();
            allergensText.TranslateTo(-400, 0, 400);
            allergensButton.TranslateTo(0, 0, 400);
        }

        public void AnimateHeader()
        {
            if(headerFrame.RotationY == 15)
            {
                headerFrame.RotateYTo(-15, 600);
            }
            else
            {
                headerFrame.RotateYTo(15, 600);
            }
        }

        public async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}