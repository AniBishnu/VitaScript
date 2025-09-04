using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vitascript.Models;

namespace Vitascript.ViewModel
{
	public class DrDrType
	{
		public User Doctor { get; set; }
        public UserType UserType { get; set; }
        public DoctorType DoctorType { get; set; }
    }
}