using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ContactsViewer.Services.Common;
using ContactsViewer.Models.ModelDTOs;

namespace ContactsViewer.Repository.Parsers
{
	public class DTOParser_Recruiter : DTOParser
	{
		#region Fields

		private int o_recruiterId;
		private int o_firstName;
		private int o_middleName;
		private int o_lastName;
		private int o_emailAddress;

		#endregion
		
		#region Methods

		public override IBaseDTO PopulateDTO(SqlDataReader reader)
		{
			RecruiterDTO recruiter = new RecruiterDTO();

			// ID
			if (!IsNull(reader, o_recruiterId))
			{
				recruiter.ID = reader.GetInt32(o_recruiterId);
			}

			// Name
			// // First name.
			if (!IsNull(reader, o_firstName))
			{
				recruiter.FirstName = reader.GetString(o_firstName);
			}
			// // Middle name.
			if (!IsNull(reader, o_middleName))
			{
				recruiter.MiddleName = reader.GetString(o_middleName);
			}
			// // Last name.
			if (!IsNull(reader, o_lastName))
			{
				recruiter.LastName = reader.GetString(o_lastName);
			}

			// Email
			if (!IsNull(reader, o_emailAddress))
			{
				recruiter.EmailAddress = reader.GetString(o_emailAddress);
			}

			recruiter.IsNew = false; // We're retrieving this data from the database.

			return recruiter;
		}

		public override void PopulateOrdinals(SqlDataReader reader)
		{
			o_recruiterId = reader.GetOrdinal("recruiter_id");
			o_firstName = reader.GetOrdinal("recruiter_first_name");
			o_middleName = reader.GetOrdinal("recruiter_middle_name");
			o_lastName = reader.GetOrdinal("recruiter_last_name");
			o_emailAddress = reader.GetOrdinal("recruiter_email_address");
		}

		private bool IsNull(SqlDataReader reader, int ordinal)
		{
			return reader.IsDBNull(ordinal);
		}

		#endregion
	}
}