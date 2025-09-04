using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class PaymentType
	{
        public int Id { get; set; }
        public string PaymentName { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}