using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class MedicalHistory
	{
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DateRecorded { get; set; }

        public int PatientId { get; set; }
        public virtual User Patient { get; set; }
    }
}