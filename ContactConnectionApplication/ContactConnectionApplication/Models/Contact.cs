using ContactsViewer.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactsViewer.Services.Validation;
using ContactsViewer.Models.ModelDTOs;

namespace ContactConnectionApplication.Models
{
	public class Contact : CommonBase
	{
		// Properties.
		// Data object.
		public new ContactDTO Data { get; set; }
		
		#region Constructors

		public Contact()
		{
			this.Data = new ContactDTO();
		}

		public Contact(ContactDTO contactDTO)
		{
			this.Data = contactDTO;
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
			ValidateCompany();
			ValidateJobTitle();

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
				this.FieldErrors.Add(new FieldError("Contact.FirstName", "<FieldName> is required."));
				return false;
			}
			else if (Validator.ValidateTextField(this.Data.FirstName))
			{
				this.FieldErrors.Add(new FieldError("Contact.FirstName", "<FieldName> is too long. Keep it below " + Validator.MAX_TEXT_LENGTH + " characters."));
				return false;
			}
			else // No need to check if multiple contacts have the same first name.
			{
				return true;
			}
		}

		public bool ValidateMiddleName()
		{
			// Ensure the middle name is in the field.
			if (Validator.ValidateTextField(this.Data.MiddleName))
			{
				this.FieldErrors.Add(new FieldError("Contact.MiddleName", "<FieldName> is too long. Keep it below " + Validator.MAX_TEXT_LENGTH + " characters."));
				return false;
			}
			else // No need to check if multiple contacts have the same middle name.
			{
				return true;
			}
		}

		public bool ValidateLastName()
		{
			// Ensure the last name is in the field.
			if (this.Data.LastName == NullType.STRING || this.Data.LastName.Length <= 0) // The email is required.
			{
				this.FieldErrors.Add(new FieldError("Contact.LastName", "<FieldName> is required."));
				return false;
			}
			else if (Validator.ValidateTextField(this.Data.LastName))
			{
				this.FieldErrors.Add(new FieldError("Contact.LastName", "<FieldName> is too long. Keep it below " + Validator.MAX_TEXT_LENGTH + " characters."));
				return false;
			}
			else // No need to check if multiple contacts have the same last name.
			{
				return true;
			}
		}


		public bool ValidateEmail()
		{
			// Ensure the email is in the field.
			if (this.Data.EmailAddress == NullType.STRING) // The email is required.
			{
				this.FieldErrors.Add(new FieldError("Contact.EmailAddress", "<FieldName> is required."));
				return false;
			}
			else if (!Validator.ValidateEmail(this.Data.EmailAddress)) // The email is invalid.
			{
				this.FieldErrors.Add(new FieldError("Contact.EmailAddress", "<FieldName> must be a valid email address."));
				return false;
			}
			else // No need to check if multiple contacts have the same email.
			{
				return true;
			}
		}


		public bool ValidateCompany()
		{
			// All values are valid.
			return true;
		}


		public bool ValidateJobTitle()
		{
			// All values are valid.
			return true;
		}

		#endregion
	}
}