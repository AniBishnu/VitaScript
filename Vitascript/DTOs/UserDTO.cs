using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vitascript.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(100), DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(200)]
        public string Address { get; set; }

        [Required, StringLength(100)]
        public string City { get; set; }

        [Required]
        public int UserTypeId { get; set; }

        public int? DoctorTypeId { get; set; }

        public int? PharmacyId { get; set; }
    }

    public enum Location
    {
        Dhaka,
        Chittagong,
        Rajshahi,
        Khulna,
        Barisal,
        Sylhet,
        Rangpur,
        Mymensingh
    }
}