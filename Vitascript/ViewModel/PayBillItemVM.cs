using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.ViewModel
{
	public class PayBillItemVM
	{
        public int PrescribedMedicineId { get; set; }
        public int BrandedMedicineId { get; set; } 

        public string BrandedName { get; set; }
        public string GenericName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}