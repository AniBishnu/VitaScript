using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vitascript.DTOs
{
	public class UserTypeDTO
	{
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string UserTypeName { get; set; }
    }
}