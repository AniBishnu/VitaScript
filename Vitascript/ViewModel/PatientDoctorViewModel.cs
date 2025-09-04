using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.ViewModel
{
	public class PatientDoctorViewModel
	{
        public int PatientId { get; set; }
        public bool Prescribed { get; set; }

        // Doctor info (User table)
        public string DoctorName { get; set; }
        public string DoctorPhone { get; set; }
        public string DoctorEmail { get; set; }
    }
}