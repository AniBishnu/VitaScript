using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
    public class MedicineTransaction
    {
        public int Id { get; set; }
        public int QuantityPurchased { get; set; }
        public decimal Price { get; set; }
        public DateTime DatePurchased { get; set; }

        public int PrescribedMedicineId { get; set; }
        public virtual PrescribedMedicine PrescribedMedicine { get; set; }

        public int PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        public int PatientId { get; set; }
        public virtual User Patient { get; set; }

        public int? PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
        public MedicineTransaction()
        {
            DatePurchased = DateTime.Now;
        }

    }

}