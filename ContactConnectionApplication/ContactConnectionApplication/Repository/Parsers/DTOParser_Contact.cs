using ContactsViewer.Models.ModelDTOs;
using ContactsViewer.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ContactsViewer.Repository.Parsers
{
	public class DTOParser_Contact : DTOParser
	{
		#region Fields

		private int o_contactid;
		private int o_firstName;
		private int o_middleName;
		private int o_lastName;
		private int o_emailAddress;
		private int o_company;
		private int o_job;
		private int o_contactRecruiter;

		#endregion
		
		#region Methods

		public override IBaseDTO PopulateDTO(SqlDataReader reader)
		{
			ContactDTO contact = new ContactDTO();

			// ID
			if (!IsNull(reader, o_contactid))
			{
				contact.ID = reader.GetInt32(o_contactid);
			}

			// Name
			// // First name.
			if (!IsNull(reader, o_firstName))
			{
				contact.FirstName = reader.GetString(o_firstName);
			}
			// // Middle name.
			if (!IsNull(reader, o_middleName))
			{
				contact.MiddleName = reader.GetString(o_middleName);
			}
			// // Last name.
			if (!IsNull(reader, o_lastName))
			{
				contact.LastName = reader.GetString(o_lastName);
			}

			// Email
			if (!IsNull(reader, o_emailAddress))
			{
				contact.EmailAddress = reader.GetString(o_emailAddress);
			}

			// Company
			if (!IsNull(reader, o_company))
			{
				contact.Company = reader.GetString(o_company);
			}

			// Job Title
			if (!IsNull(reader, o_job))
			{
				contact.JobTitle = reader.GetString(o_job);
			}

			// Recruiter ID
			if (!IsNull(reader, o_contactRecruiter))
			{
				contact.RecruiterID = reader.GetInt32(o_contactRecruiter);
			}

			contact.IsNew = false; // We're retrieving this data from the database.

			return contact;
		}

		public override void PopulateOrdinals(SqlDataReader reader)
		{
			o_contactid = reader.GetOrdinal("contact_id");
			o_firstName = reader.GetOrdinal("contact_first_name");
			o_middleName = reader.GetOrdinal("contact_middle_name");
			o_lastName = reader.GetOrdinal("contact_last_name");
			o_emailAddress = reader.GetOrdinal("contact_email_address");
			o_company = reader.GetOrdinal("contact_company");
			o_job = reader.GetOrdinal("contact_job_title");
			o_contactRecruiter = reader.GetOrdinal("contact_recruiter_id");
		}

		private bool IsNull(SqlDataReader reader, int ordinal)
		{
			return reader.IsDBNull(ordinal);
		}

		#endregion

	}
}