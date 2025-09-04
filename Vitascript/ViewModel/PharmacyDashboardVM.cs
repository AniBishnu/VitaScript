using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vitascript.Models;

namespace Vitascript.ViewModel
{
	public class PharmacyDashboardVM
	{
        public User User { get; set; }

        public decimal TotalSales { get; set; }
        public int TotalMedicines { get; set; }
        public int TotalUsersInteracted { get; set; }

        public List<MostSoldMedicineVM> MostSoldMedicines { get; set; }
        public int TotalMonthlySales { get; set; }

        public List<DailySalesVM> DailySales { get; set; }
    }
}