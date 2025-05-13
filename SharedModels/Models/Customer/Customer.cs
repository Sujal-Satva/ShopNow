using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Customer
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; } // FK to AspNetUsers

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(255)]
        public string AddressLine1 { get; set; }

        [Required]
        [MaxLength(255)]
        public string AddressLine2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(100)]
        public string State { get; set; }

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; }

        public bool ShippingAddressIsSameAsBilling { get; set; } = false;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateLastUpdated { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
