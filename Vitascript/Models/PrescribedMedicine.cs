using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class PrescribedMedicine
	{
        public int Id { get; set; }

        public string Dose { get; set; }
        public int TotalQuantity { get; set; }
        public int RemainingQuantity { get; set; } = 0;
        public int Duration { get; set; } // in days or custom unit
        public string Notes { get; set; }

        public int PrescriptionId { get; set; }
        public virtual Prescription Prescription { get; set; }

        public int GenericMedicineId { get; set; }
        public virtual GenericMedicine GenericMedicine { get; set; }

        public virtual ICollection<MedicineTransaction> MedicineTransactions { get; set; }

    }
}