﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PLMS.Models.ModelViews
{
	public class LOViewModel
	{
        public int ApplicationId { get; set; }
        public string ApplicantName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }

    }
}