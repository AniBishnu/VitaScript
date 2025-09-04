using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.DTOs
{
	public class CreatePrescriptionDTO
	{
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public List<PrescribedMedicineDTO> PrescribedMedicines { get; set; }
        public string MedicalHistoryDescription { get; set; }
    }
}