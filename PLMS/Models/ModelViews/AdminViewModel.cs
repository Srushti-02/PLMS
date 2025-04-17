using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PLMS.Models.ModelViews
{
	public class AdminViewModel
	{
        [Key]
        public int userId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "User already exists")]
        public string username { get; set; }

        public string userpass { get; set; }
        [Required]
        [StringLength(100)]
        public string fullName { get; set; }
        [Required]
        [StringLength(15)]
        public string phoneNum { get; set; }

        [Required]
        [StringLength(20)]
        public string role { get; set; }

        [StringLength(10)]
        public string access { get; set; }
    }
}