using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.ViewModel
{
	public class ProvideMedicineVM
	{
        public int PrescriptionId { get; set; }
        public string PrescriptionCode { get; set; }
        public string PatientName { get; set; }

        public int patientID { get; set; }
        public List<ProvideMedicineRowVM> Medicines { get; set; }
    }
}