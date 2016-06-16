using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactsViewer.Models.ModelDTOs;

namespace ContactsViewer.Services.Common
{
	public class NullType
	{
		// Type nulls.
		public const int INT = int.MinValue;
		public const float FLOAT = float.MinValue;
		public const decimal DECIMAL = decimal.MinValue;
		public const double DOUBLE = double.MinValue;
		public const string STRING = null;

		// Custom object nulls.
		public static readonly RecruiterDTO RECRUITER
			= new RecruiterDTO();

		public static readonly ContactDTO CONTACT
			= new ContactDTO();

	}
}