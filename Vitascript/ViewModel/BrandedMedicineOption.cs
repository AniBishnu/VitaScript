using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.ViewModel
{
	public class BrandedMedicineOption
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
        public decimal Price { get; set; }
        public int QuantityAvailable { get; set; }
    }
}