using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class Payment
	{
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int Mobilenumber{ get; set; }
        public string TransactionId { get; set; }
        public int PatientId { get; set; }
        public virtual User Patient { get; set; }

        public int PrescriptionId { get; set; }
        public virtual Prescription Prescription { get; set; }

        public int PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        public int PaymentTypeId { get; set; }
        public virtual PaymentType PaymentType { get; set; }

        public virtual ICollection<MedicineTransaction> MedicineTransactions { get; set; }

        public Payment()
        {
            TransactionId = "VTP-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            PaymentDate = DateTime.Now;
        }
    }
}