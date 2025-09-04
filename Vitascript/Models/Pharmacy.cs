using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class Pharmacy
	{
        public int Id { get; set; }
        public string PharmacyName { get; set; }

        public virtual ICollection<User> Pharmacists { get; set; }
        public virtual ICollection<PharmacyInventory> Inventories { get; set; }
        public virtual ICollection<MedicineTransaction> Transactions { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}