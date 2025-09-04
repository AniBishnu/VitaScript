using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vitascript.Models;

namespace Vitascript.ViewModel
{
	public class PharmacyInventoryViewModel
	{
        public int Id { get; set; } // BrandedMedicine ID
        public string MedicineName { get; set; }
        public string BrandName { get; set; }
        public string GenericName { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal MRP { get; set; }
        public int QuantityAvailable { get; set; }
    }
}