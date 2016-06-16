using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactsViewer.Models.ModelDTOs;
using ContactsViewer.Services.Common;
using ContactsViewer.Services.Validation;

namespace ContactsViewer.Models
{
	public class Recruiter : CommonBase
	{
		// Properties.
		// Data object.
		public new RecruiterDTO Data { get; set; }

		#region Constructors

		public Recruiter()
		{
			this.Data = new RecruiterDTO();
		}

		public Recruiter(RecruiterDTO recruiterDTO)
		{
			this.Data = recruiterDTO;
		}

		#endregion

		#region Validation methods.
		public override List<FieldError> Validate()
		{
			// Call all validation functions.
			ValidateFirstName();
			ValidateMiddleName();
			ValidateLastName();
			ValidateEmail();

			// Return any errors.
			return this.FieldErrors;
		}

		// Here are the required fields:
		// First Name, Last Name, Email Address, 
		public bool ValidateFirstName()
		{
			// Ensure the first name is in the field.
			if (this.Data.FirstName == NullType.STRING || this.Data.FirstName.Length <= 0) // The email is required.
			{
				this.FieldErrors.Add(new FieldError("Recruiter.FirstName", "<FieldName> is required."));
				return false;
			}
			else if (Validator.ValidateTextField(this.Data.FirstName))
			{
				this.FieldErrors.Add(new FieldError("Recruiter.FirstName", "<FieldName> is too long. Keep it below " + Validator.MAX_TEXT_LENGTH + " characters."));
				return false;
			}
			else // No need to check if multiple recruiters have the same first name.
			{
				return true;
			}
		}

		public bool ValidateMiddleName()
		{
			// Ensure the middle name is in the field.
			if (Validator.ValidateTextField(this.Data.MiddleName))
			{
				this.FieldErrors.Add(new FieldError("Recruiter.MiddleName", "<FieldName> is too long. Keep it below " + Validator.MAX_TEXT_LENGTH + " characters."));
				return false;
			}
			else // No need to check if multiple recruiters have the same middle name.
			{
				return true;
			}
		}

		public bool ValidateLastName()
		{
			// Ensure the last name is in the field.
			if (this.Data.LastName == NullType.STRING || this.Data.LastName.Length <= 0) // The email is required.
			{
				this.FieldErrors.Add(new FieldError("Recruiter.LastName", "<FieldName> is required."));
				return false;
			}
			else if (Validator.ValidateTextField(this.Data.LastName))
			{
				this.FieldErrors.Add(new FieldError("Recruiter.LastName", "<FieldName> is too long. Keep it below " + Validator.MAX_TEXT_LENGTH + " characters."));
				return false;
			}
			else // No need to check if multiple recruiters have the same last name.
			{
				return true;
			}
		}


		public bool ValidateEmail()
		{
			// Ensure the email is in the field.
			if (this.Data.EmailAddress == NullType.STRING) // The email is required.
			{
				this.FieldErrors.Add(new FieldError("Recruiter.EmailAddress", "<FieldName> is required."));
				return false;
			}
			else if (!Validator.ValidateEmail(this.Data.EmailAddress)) // The email is invalid.
			{
				this.FieldErrors.Add(new FieldError("Recruiter.EmailAddress", "<FieldName> must be a valid email address."));
				return false;
			}
			else // No need to check if multiple recruiters have the same email.
			{
				return true;
			}
		}

		#endregion
		
	}
}