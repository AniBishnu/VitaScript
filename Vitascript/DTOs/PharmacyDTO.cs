using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vitascript.DTOs
{
	public class PharmacyDTO
	{
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string PharmacyName { get; set; }
    }
}