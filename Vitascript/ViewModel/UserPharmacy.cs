using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vitascript.Models;

namespace Vitascript.ViewModel
{
	public class UserPharmacy
	{
		public User Pharmacy{ get; set; }
        public UserType UserType { get; set; }
        public Pharmacy pharmacyName { get; set; }
    }

   

}