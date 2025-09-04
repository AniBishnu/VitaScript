using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class GenericMedicine
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public string Formula { get; set; }

        public virtual ICollection<BrandedMedicine> BrandedMedicines { get; set; }
        public virtual ICollection<PrescribedMedicine> PrescribedMedicines { get; set; }
    }
}