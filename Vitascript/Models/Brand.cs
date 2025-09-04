using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class Brand
	{
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BrandedMedicine> BrandedMedicines { get; set; }
    }
}