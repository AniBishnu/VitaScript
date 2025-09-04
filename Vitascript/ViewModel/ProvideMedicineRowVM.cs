using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.ViewModel
{
	public class ProvideMedicineRowVM
	{
        public int PrescribedMedicineId { get; set; }
        public string GenericName { get; set; }
        public int RemainingQuantity { get; set; }
        public List<BrandedMedicineOption> BrandedOptions { get; set; }
    }
}