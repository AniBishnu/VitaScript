using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.DTOs
{
	public class MedicalHistoryDTO
	{
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DateRecorded { get; set; }
        public string PatientName { get; set; }
    }
}