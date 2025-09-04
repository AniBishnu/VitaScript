using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.DTOs
{
	public class PrescriptionViewDTO
	{
        public int Id { get; set; }
        public string PrescriptionCode { get; set; }
        public int DoctorID { get; set; }

        public string DoctorName { get; set; }
        public int PatientID { get; set; }
        public string PatientName { get; set; }

        public DateTime IssuedDate { get; set; }
        public List<PrescribedMedicineViewDTO> PrescribedMedicines { get; set; }
    }
}