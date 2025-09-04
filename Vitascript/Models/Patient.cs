using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class Patient
	{
        public int Id { get; set; }

        
        public int PatientId { get; set; }
        public virtual User PatientUser { get; set; }

        public int? AssignedDoctorId { get; set; }
        public virtual User AssignedDoctor { get; set; }
        public bool Prescribed { get; set; } = false;
    }
}