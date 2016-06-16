using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace CSVHelperTester
{
	public sealed class ContactsMap : CsvClassMap<Contact>
	{
		public const string COL_ID = "Contact ID";
		public const string COL_FIRST_NAME = "First Name";
		public const string COL_MIDDLE_NAME = "Middle Name";
		public const string COL_LAST_NAME = "Last Name";
		public const string COL_EMAIL_ADDRESS = "E-mail Address";
		public const string COL_COMPANY = "Company";
		public const string COL_JOB_TITLE = "Job Title";
		public const string COL_RECRUITER_ID = "Recruiter ID";
		public const string COL_RECRUITER = "Recruiter";

		public ContactsMap()
		{
			Map(m => m.FirstName).Name(COL_FIRST_NAME).Default("");
			Map(m => m.MiddleName).Name(COL_MIDDLE_NAME).Default(""); 
			Map(m => m.LastName).Name(COL_LAST_NAME).Default("");
			Map(m => m.EmailAddress).Name(COL_EMAIL_ADDRESS).Default("");
			Map(m => m.Company).Name(COL_COMPANY).Default("");
			Map(m => m.JobTitle).Name(COL_JOB_TITLE).Default("");
		}
	}
}
