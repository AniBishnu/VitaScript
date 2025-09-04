using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vitascript.Models
{
	public class UserType
	{
        public int Id { get; set; }
        public string UserTypeName { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}