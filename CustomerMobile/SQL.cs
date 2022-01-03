using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using static CustomerMobile.GlobalVar;
using System.Threading.Tasks;
using FluentFTP;
using System.Threading;

namespace CustomerMobile
{
    public static class SQL
    {
        public static async Task CheckUserLogin(string email, string password)
        {
            using (SqlConnection conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "SELECT Password FROM Customers WHERE Email='" + email + "'",

                    Connection = conn
                };

                object result = await Task.Run(() => cmd.ExecuteScalar());

                if (result == null)
                {
                    _CheckCustomerResult = false;
                }
                else
                {
                    if (result.ToString() == password)
                    {
                        _CheckCustomerResult = true;
                    }
                    else
                    {
                        _CheckCustomerResult = false;
                    }
                }
            }
        }

        public static bool CheckUserExists(string email)
        {
            using (SqlConnection connectionCheck = new SqlConnection(_ConnectionString))
            {
                connectionCheck.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "SELECT Email FROM Customers WHERE Email='" + email + "'",

                    Connection = connectionCheck
                };

                object result = cmd.ExecuteScalar();

                if (result == DBNull.Value || result == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }

        public static Customer LoadCustomer(string email)
        {
            Customer customer = new Customer();
            using (SqlConnection connection = new SqlConnection(_ConnectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "SELECT * FROM Customers WHERE Email='" + email + "'",

                    Connection = connection
                };



                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    customer.Address1 = reader["Address1"].ToString();
                    customer.Address2 = reader["Address2"].ToString();
                    customer.Address3 = reader["Address3"].ToString();
                    customer.Email = email;
                    customer.FirstName = reader["FirstName"].ToString();
                    customer.HasActiveOrder = bool.Parse(reader["HasActiveOrder"].ToString());
                    customer.IsPasswordReset = bool.Parse(reader["IsPasswordReset"].ToString());
                    customer.Orders = int.Parse(reader["Orders"].ToString());
                    customer.Phone = reader["Phone"].ToString();
                    customer.PostCode = reader["PostCode"].ToString();
                    customer.Status = reader["Status"].ToString();
                    customer.Surname = reader["Surname"].ToString();
                    customer.Tier = int.Parse(reader["Tier"].ToString());
                    customer.TotalSpending = decimal.Parse(reader["TotalSpending"].ToString());
                    customer.Password = reader["Password"].ToString();
                    customer.Id = int.Parse(reader["Id"].ToString());

                }

                connection.Close();



            }
            return customer;
        }

        public static Order LoadActiveOrder(string email)
        {
            Order activeOrder = new Order();
            using (SqlConnection connection = new SqlConnection(_ConnectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "SELECT * FROM Orders WHERE Customer='" + email + "' AND IsActive='True'",

                    Connection = connection
                };



                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    activeOrder.OrderCreated = DateTime.Parse(reader["Created"].ToString());
                    activeOrder.OrderChanged = DateTime.Parse(reader["StatusChanged"].ToString());
                    activeOrder.OrderId = int.Parse(reader["Id"].ToString());
                    activeOrder.OrderPrice = decimal.Parse(reader["TotalAmount"].ToString());
                    activeOrder.OrderStatus = reader["Status"].ToString();
                    activeOrder.OrderEta = reader["Eta"].ToString();
                    activeOrder.OrderLat = double.Parse(reader["LocationLat"].ToString());
                    activeOrder.OrderLon = double.Parse(reader["LocationLon"].ToString());

                }

                connection.Close();



            }
            return activeOrder;
           
        }

        public static void UpdateOrderIsActive(int orderId, string active)
        {
            using (SqlConnection connectionCheck = new SqlConnection(_ConnectionString))
            {
                connectionCheck.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "UPDATE Orders SET IsActive='" + active + "' WHERE Id='" + orderId + "'",

                    Connection = connectionCheck
                };



                cmd.ExecuteNonQuery();


            }
        }

        public static void UpdateCustomerHasActiveOrder(string customer, string hasActiveOrder)
        {
            using (SqlConnection connectionCheck = new SqlConnection(_ConnectionString))
            {
                connectionCheck.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "UPDATE Customers SET HasActiveOrder='" + hasActiveOrder + "' WHERE Email='" + customer + "'",

                    Connection = connectionCheck
                };



                cmd.ExecuteNonQuery();


            }
        }

        public static void UpdateOrder(int id, string toSet, string newValue)
        {
            using (SqlConnection connectionCheck = new SqlConnection(_ConnectionString))
            {
                connectionCheck.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "UPDATE Orders SET " + toSet + "='" + newValue + "' WHERE Id='" + id + "'",

                    Connection = connectionCheck
                };



                cmd.ExecuteNonQuery();


            }
        }

        public static void NewCustomer(string email, string surname, string firstname, string telephone, string password, string notes, string address1, string address2, string address3, string postcode)
        {
            using (SqlConnection connectionCheck = new SqlConnection(_ConnectionString))
            {
                connectionCheck.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "INSERT INTO Customers (Email, Surname, FirstName, Phone, Password, Notes, Address1, Address2, Address3, PostCode, Status, Registered, LastActive, StatusChanged, " +
                                    "Tier, Orders, HasActiveOrder) VALUES ('" + email + "', '" + surname + "', '" + firstname + "', '" + telephone + "', '" + password + "', '" + notes + "', '" +
                                    address1 + "', '" + address2 + "', '" + address3 + "', '" + postcode + "', 'Active', '" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "', '" +
                                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "', '" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "', '0', '0', 'False')",

                    Connection = connectionCheck
                };



                cmd.ExecuteNonQuery();


            }
        }

        public static void NewOrder(string items, string totalAmount, string deliveryFee)
        {
            using (SqlConnection connectionCheck = new SqlConnection(_ConnectionString))
            {
                connectionCheck.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "INSERT INTO Orders (Created, Customer, DeliveryAddress, DeliveryPostCode, TotalAmount, IsActive, Items, DeliveryFee, Status, LocationLat, LocationLon, StatusChanged) VALUES ('" +
                                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "', '" + _GlobalCustomer.Email + "', '" + _GlobalCustomer.Address1 + "', '" + _GlobalCustomer.PostCode + "', '" + totalAmount + "', 'True', '" +
                                    items + "', '" + deliveryFee + "', 'NewOrder', '" + _RestaurantPosition.Latitude + "', '" + _RestaurantPosition.Longitude + "', '" + DateTime.Now.ToString("HH:mm:ss") + "')",

                    Connection = connectionCheck
                };



                cmd.ExecuteNonQuery();


            }
        }

        /// <summary>
        /// UPDATE CUSTOMER ->> Delivery address, Phone, Password, Name, and HasActiveOrder property
        /// </summary>
        /// <param name="customer"></param>
        public static void UpdateCustomer(Customer customer)
        {
            using (SqlConnection connectionCheck = new SqlConnection(_ConnectionString))
            {
                connectionCheck.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "UPDATE Customers SET Address1='" + customer.Address1 + "', Address2='" + customer.Address2 + "', Address3='" + customer.Address3 + "', PostCode='" + customer.PostCode +
                                    "', Phone='" + customer.Phone + "', Password='" + customer.Password + "', FirstName='" + customer.FirstName + "', Surname='" + customer.Surname + "', HasActiveOrder='True' WHERE Email='" + customer.Email + "'",

                    Connection = connectionCheck
                };



                cmd.ExecuteNonQuery();


            }
        }

        public static void LoadDocsOld() // NOT USED - FOR TESTING
        {
            using (FtpClient ftp = new FtpClient("ftp.site4now.net", "luxasscze-001", "Kasumi2Goto"))
            {
                ftp.Connect();
                ftp.CompareFile(_TCTextFile, "", FtpCompareOption.Auto);
                ftp.DownloadDirectory(_DOCSPath, "/lukasslivka/ProjectJoe/DOC/", FtpFolderSyncMode.Update, FtpLocalExists.Overwrite);
                ftp.Disconnect();

            }
        }

        public static void DownloadDocs() // NOT USED - FOR TESTING
        {
            using (var ftp = new FtpClient("ftp.site4now.net", "luxasscze-001", "Kasumi2Goto"))
            {
                ftp.Connect();


                ftp.DownloadFiles(_DOCSPath,
                    new[] {
                        @"/lukasslivka/ProjectJoe/DOC/TC.txt",
                        @"/lukasslivka/ProjectJoe/DOC/GDPR.txt",
                        @"/lukasslivka/ProjectJoe/DOC/COMPDESC.txt"
                    }, FtpLocalExists.Overwrite);

            }
        }

        public static async Task DownloadDocsAsync() 
        {
            var token = new CancellationToken();
            using (var ftp = new FtpClient("ftp.site4now.net", "luxasscze-001", "Kasumi2Goto"))
            {
                ftp.ConnectTimeout = 60000;
                await ftp.ConnectAsync(token);             
                                
                await ftp.DownloadFilesAsync(_DOCSPath,
                    new[] {
                        @"/lukasslivka/ProjectJoe/DOC/TC.txt",
                        @"/lukasslivka/ProjectJoe/DOC/GDPR.txt",
                        @"/lukasslivka/ProjectJoe/DOC/COMPDESC.txt"
                    }, FtpLocalExists.Skip);

            }
        }

        public static async Task GetChecksumAsync() // NOT USED - FOR TESTING
        {
            var token = new CancellationToken();

            using (var conn = new FtpClient("ftp.site4now.net", "luxasscze-001", "Kasumi2Goto"))
            {
                await conn.ConnectAsync(token);

                
                FtpHash hash = await conn.GetChecksumAsync(@"/lukasslivka/ProjectJoe/DOC/TC.txt", token);

                
                if (hash.IsValid)
                {
                    if (hash.Verify(_TCTextFile))
                    {
                        _ResultOfCheckSum = true;
                    }
                    else
                    {
                        _ResultOfCheckSum = false;
                    }
                }
            }

        }



    }
        
}
