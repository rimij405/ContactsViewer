using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using unirest_net.http;
using unirest_net.request;

namespace ContactsViewer.Services.Validation
{
	/// <summary>
	/// Validator.cs
	/// This class aids the process of validating
	/// different fields, especially those requiring
	/// calls to outside APIs.
	/// </summary>
	public static class Validator
	{
		#region Constants

		// // Validation values.
		public const string EMAIL_VALIDATION_URI = "https://pozzad-email-validator.p.mashape.com/emailvalidator/validateEmail/";
		public const string EMAIL_VALIDATION_API_KEY_TYPE = "X-Mashape-Key";
		public const string EMAIL_VALIDATION_API_KEY = "cmDL0lmrTbmsh8SbKkDD4LWEpCgNp1ONnCOjsn9xRzlbEk5Jrz";
		public const int MAX_TEXT_LENGTH = 50;

		#endregion

		#region Methods

		/// <summary>
		/// ValidateEmail (string)
		/// This method takes an email address as a parameter
		/// and makes a call to the pozzad-email-validator
		/// API (free) through Mashape.
		/// </summary>
		/// <param name="email">Email address in the field.</param>
		/// <returns>True if valid, false if invalid.</returns>
		public static bool ValidateEmail(string email)
		{
			if ((email != null) && (email.Length >= 3) && (email.Contains("@"))) // a minimal of 3 characters is necessary for an email address.
			{
				string uri = EMAIL_VALIDATION_URI + email;
				string key_type = EMAIL_VALIDATION_API_KEY_TYPE;
				string api_key = EMAIL_VALIDATION_API_KEY;

				// Craft HttpRequest.
				unirest_net.request.HttpRequest emailValidation = Unirest.get(uri)
					.header(key_type, api_key)
					.header("accept", "application/json");

				// Get the response.
				HttpResponse<string> response = emailValidation.asJson<string>();
				dynamic dynObj = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Body.ToString());

				bool validity;
				var isValid = dynObj.isValid;

				if (isValid == null)
				{
					validity = false;
				}
				else
				{
					validity = true;
				}

				Boolean.TryParse((string)isValid, out validity);

				return validity;
			}
			else
			{
				return false;
			}
		}

		public static bool ValidatePassword(string password)
		{
			if (string.IsNullOrEmpty(password.Trim()))
			{
				return false;
			}
			else if (password.Trim().Length < 6)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		// Ensures the text field does not have too many characters
		// Nor too little.
		public static bool ValidateTextField(string entry)
		{
			if (entry.Trim().Length > MAX_TEXT_LENGTH)
			{
				return false;
			}

			return true;
		}

		#endregion






	}
}