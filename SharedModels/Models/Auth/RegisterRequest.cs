using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Auth
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public SellerDetails? SellerDetails { get; set; }
        public CustomerDetails? CustomerDetails { get; set; }
    }


    public class SellerDetails
    {
        public string ContactPersonName { get; set; }           // Required
        public string SellerCompanyName { get; set; }           // Required
        public string ProductCategory { get; set; }             // Required
        public string City { get; set; }                        // Required
        public bool IsUniqueCompanyInCity { get; set; } = false;
        // Contact info
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Address
        public string AddressLine { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }



    public class CustomerDetails
    {
        public string FirstName { get; set; }                  
        public string LastName { get; set; }                    
        public string Email { get; set; }                       
        public string PhoneNumber { get; set; }               
        public DateTime DateOfBirth { get; set; }               
        public string AddressLine1 { get; set; }                
        public string? AddressLine2 { get; set; }
        public string City { get; set; }                      
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public bool ShippingAddressIsSameAsBilling { get; set; }
    }




    public class RefreshTokenRequest
    {
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
