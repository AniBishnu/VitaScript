using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vitascript.Models;

namespace Vitascript.ViewModel
{
	public class AddMedicineVM
	{
        
            public int GenericMedicineId { get; set; }
            public int BrandId { get; set; }
            public int BrandedMedicineId { get; set; }
            public int Quantity { get; set; }

            public IEnumerable<SelectListItem> GenericMedicines { get; set; }
            public IEnumerable<SelectListItem> Brands { get; set; }
            public IEnumerable<SelectListItem> BrandedMedicines { get; set; }
        
    }
}