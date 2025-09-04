using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vitascript.Models;

namespace Vitascript.ViewModel
{
	public class PayBillVM
	{
        public List<PayBillItemVM> CartItems { get; set; }
        public int SelectedPaymentTypeId { get; set; }
        public string MobileNumber { get; set; }
        public decimal GrandTotal => CartItems?.Sum(x => x.TotalPrice) ?? 0;
        public List<PaymentType> PaymentOptions { get; set; }
        public int PatientId { get; set; }

        public int PrescriptionId { get; set; }
    }
}