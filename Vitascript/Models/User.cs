using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class User
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }

        public int UserTypeId { get; set; }
        public virtual UserType UserType { get; set; }

        public int? DoctorTypeId { get; set; }
        public virtual DoctorType DoctorType { get; set; }

        public int? PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        public virtual ICollection<Prescription> PrescriptionsGiven { get; set; } // If doctor
        public virtual ICollection<Prescription> PrescriptionsReceived { get; set; } // If patient
        public virtual ICollection<MedicalHistory> MedicalHistories { get; set; }
        public virtual ICollection<MedicineTransaction> MedicineTransactions { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}