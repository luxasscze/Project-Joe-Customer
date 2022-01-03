using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerMobile
{
    public class Order
    {
        public int OrderId { get; set; }
        public int OrderCustomerId { get; set; }
        public List<ItemsList> OrderItems { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderCreated { get; set; }
        public DateTime OrderChanged { get; set; }
        public string OrderChangeLog { get; set; }
        public decimal OrderPrice { get; set; }
        public string OrderNotes { get; set; }
        public string OrderEta { get; set; }
        public double OrderLat { get; set; }
        public double OrderLon { get; set; }


        

    }
}
