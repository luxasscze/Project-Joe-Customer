using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerMobile
{
    public class Customer
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public bool HasActiveOrder { get; set; }
        public int Orders { get; set; }
        public int Tier { get; set; }
        public string Status { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostCode { get; set; }
        public decimal TotalSpending { get; set; }
        public bool IsPasswordReset { get; set; }
        public string Password { get; set; }
        public int Id { get; set; }




    }
}
