using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.DTOs
{
	public class PrescribedMedicineViewDTO
	{
        public string GenericMedicineName { get; set; }
        public string Dose { get; set; }
        public int TotalQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public int Duration { get; set; }
        public string Notes { get; set; }
    }
}