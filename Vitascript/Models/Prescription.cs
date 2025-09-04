using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class Prescription
	{
        public int Id { get; set; }
        public string PrescriptionCode { get; set; } // Auto-generated
        public DateTime IssuedDate { get; set; }

        public int DoctorId { get; set; }
        public virtual User Doctor { get; set; }

        public int PatientId { get; set; }
        public virtual User Patient { get; set; }

        public virtual ICollection<PrescribedMedicine> PrescribedMedicines { get; set; }

        public Prescription()
        {
            PrescriptionCode = "HTB-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            IssuedDate = DateTime.Now;
        }
    }
}