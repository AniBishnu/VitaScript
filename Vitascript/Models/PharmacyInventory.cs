using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class PharmacyInventory
	{
        public int Id { get; set; }
        public int AvailableQuantity { get; set; }

        public int PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        public int BrandedMedicineId { get; set; }
        public virtual BrandedMedicine BrandedMedicine { get; set; }
    }
}