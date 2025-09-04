using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vitascript.DTOs;
using Vitascript.Models;

namespace Vitascript.ViewModel
{
	public class PatientDetailsViewModel
	{
        public User PatientUser { get; set; }
        public List<MedicalHistoryDTO> MedicalHistories { get; set; }
        public List<PrescriptionViewDTO> Prescriptions { get; set; }
    }
}