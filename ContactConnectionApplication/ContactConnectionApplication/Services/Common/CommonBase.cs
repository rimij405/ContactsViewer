using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactsViewer.Services.Validation;

namespace ContactsViewer.Services.Common
{
	public abstract class CommonBase
	{
		// Constants
		// // Search functions.
		public const string ID_LABEL = "Id";
		public const string FIRST_NAME = "First Name";
		public const string MIDDLE_NAME = "Middle Name";
		public const string LAST_NAME = "Last Name";
		public const string EMAIL_ADDRESS = "Email Address";
		public const string ORGANIZATION = "Company";
		public const string OCCUPATION = "Job Title";
		public const string RECRUITER_ID = "Recruiter ID";
		public static readonly string[] CODES = { ID_LABEL, FIRST_NAME, MIDDLE_NAME, LAST_NAME, EMAIL_ADDRESS, ORGANIZATION, OCCUPATION };

		public const string BLANK = "";
		public const string STATUS_RECRUITER = "Recruiter";
		public const string STATUS_INTERN = "Intern";
		public const string STATUS_OTHER = "Other";

		// Fields
		private List<FieldError> errors;

		// Properties
		public BaseDTO Data { get; set; }

		public List<FieldError> FieldErrors
		{
			get
			{
				if (errors == null)
				{
					errors = new List<FieldError>();
				}

				return errors;
			}
			set
			{
				this.errors = value;
			}
		}

		// Constructor

		// Methods
		public abstract List<FieldError> Validate();
	}
}