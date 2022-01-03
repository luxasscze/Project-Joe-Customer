using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerMobile
{
    public class Food
    {

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Image { get; set; }
        public bool IsAvailable { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Remarks { get; set; }
        public int AmountSold { get; set; }
        public string LastSold { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public decimal SellPrice { get; set; }
        public string[] Allergens { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public string DisplayPrice { get; set; }
        public int Quantity { get; set; }
        public int Id { get; set; }



        public override string ToString()
        {
            return Name;
        }



    }
}
