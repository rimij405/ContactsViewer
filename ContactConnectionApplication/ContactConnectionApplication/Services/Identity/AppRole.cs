using ContactsViewer.Services.Common;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactsViewer.Services.Identity
{
	public class AppRole : IdentityRole
	{
		public AppRole() : base() { }
		public AppRole(string name) : base(name) { }
	}
}