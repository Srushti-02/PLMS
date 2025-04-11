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
        [StringLength(50)]
        public string username { get; set; }

        [Required]
        [StringLength(100)]
        public string userpass { get; set; }

        [StringLength(100)]
        public string fullName { get; set; }

        [StringLength(15)]
        public string phoneNum { get; set; }

        [StringLength(20)]
        public string role { get; set; }

        [StringLength(10)]
        public string access { get; set; }
    }
}