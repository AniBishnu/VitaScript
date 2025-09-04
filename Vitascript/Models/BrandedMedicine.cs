using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class BrandedMedicine
	{
        public int Id { get; set; }
        public string MedicineName { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal MRP { get; set; }
        public int QuantityAvailable { get; set; }

        public int GenericMedicineId { get; set; }
        public virtual GenericMedicine GenericMedicine { get; set; }

        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; }

        public virtual ICollection<PharmacyInventory> PharmacyInventories { get; set; }
    }
}