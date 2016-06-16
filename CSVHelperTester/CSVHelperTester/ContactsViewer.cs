using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CsvHelper;
using unirest_net;
using unirest_net.http;
using unirest_net.request;


namespace CSVHelperTester
{
	public static class ContactsViewer
	{
		// Constants.
		// // Validation values.
		public const string EMAIL_VALIDATION_URI = "https://pozzad-email-validator.p.mashape.com/emailvalidator/validateEmail/";
		public const string EMAIL_VALIDATION_API_KEY_TYPE = "X-Mashape-Key";
		public const string EMAIL_VALIDATION_API_KEY = "cmDL0lmrTbmsh8SbKkDD4LWEpCgNp1ONnCOjsn9xRzlbEk5Jrz";

		// // Search functions.
		public const string ID = "Id";
		public const string FIRST_NAME = "First Name";
		public const string MIDDLE_NAME = "Middle Name";
		public const string LAST_NAME = "Last Name";
		public const string EMAIL_ADDRESS = "Email Address";
		public const string ORGANIZATION = "Company";
		public const string OCCUPATION = "Job Title";
		public const string RECRUITER_ID = "Recruiter ID";
		public static readonly string[] CODES = { ID, FIRST_NAME, MIDDLE_NAME, LAST_NAME, EMAIL_ADDRESS, ORGANIZATION, OCCUPATION };

		public const string BLANK = "";

		// Attributes.
		private static bool initState; // True for initialized. False for unmanaged.
		private static RecruiterManager recruiters; // A storage wrapper for Recruiters.
		private static ContactManager contacts; // A storage wrapper for ALL Contacts.

		// Properties.
		public static bool IsInitialized
		{
			get { return initState; }
		}

		public static bool HasRecruiters
		{
			get { return recruiters.IsEmpty; }
		}

		public static bool HasContacts
		{
			get { return contacts.IsEmpty; }
		}

		public static int RecruiterCount
		{
			get { return recruiters.Count; }
		}

		public static int ContactCount
		{
			get { return contacts.Count; }
		}

		public static List<Recruiter> Recruiters
		{
			get { return GetRecruiters(); }
		}

		public static List<Contact> Contacts
		{
			get { return GetContacts(); }
		}

		// Fields.


		// Methods.
		public static void Initialize()
		{
			init();
			initState = true;
		}

		private static void init()
		{
			// Initalize functions go here.
			recruiters = new RecruiterManager();
			contacts = new ContactManager();
		}

		#region Validation Methods

		public static bool ValidateEmailAddress(string emailQuery)
		{
			if ((emailQuery != null) && (emailQuery.Length >= 3) && (emailQuery.Contains("@"))) // a minimal of 3 characters is necessary for an email address.
			{
				string uri = "https://pozzad-email-validator.p.mashape.com/emailvalidator/validateEmail/" + emailQuery;
				string key_type = ContactsViewer.EMAIL_VALIDATION_API_KEY_TYPE;
				string api_key = ContactsViewer.EMAIL_VALIDATION_API_KEY;

				// Craft HttpRequest.
				HttpRequest emailValidation = Unirest.get(uri)
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

		#endregion

		#region Recruiter Access Methods
		public static Recruiter GetRecruiterByID(int recruiterId)
		{
			return recruiters.GetRecruiter(recruiterId);
		}

		public static List<Recruiter> GetRecruiterByIDs(params int[] recruiterIds)
		{
			return recruiters.GetRecruiters(recruiterIds);
		}
		
		public static List<Recruiter> FindRecruitersByQuery(string value)
		{
			return recruiters.FindRecruiters(value);
		}

		public static List<Recruiter> FindRecruitersByQueries(params string[] values)
		{
			return recruiters.FindRecruiters(values);
		}

		public static List<Recruiter> FindRecruitersByQuery(string code, string value)
		{
			return recruiters.FindRecruiters(value);
		}

		public static List<Recruiter> FindRecruitersByQueries(string code, params string[] values)
		{
			return recruiters.FindRecruiters(values);
		}

		public static Recruiter FindRecruiterByContact(int contactId)
		{
			return FindRecruiterByContact(GetContactByID(contactId));
		}

		public static Recruiter FindRecruiterByContact(Contact contactQuery)
		{
			return GetRecruiterByID(contactQuery.RecruiterId);
		}

		public static List<Recruiter> FindRecruitersByContacts(params int[] contactIds)
		{
			return FindRecruitersByContacts(GetContactsByIDs(contactIds).ToArray<Contact>());
		}

		public static List<Recruiter> FindRecruitersByContacts(params Contact[] contactQuery)
		{
			// Check array size.
			if (contactQuery == null || contactQuery.Length <= 0)
			{
				return null;
			}

			List<Recruiter> recruitersByContacts = new List<Recruiter>();

			foreach (Contact contact in contactQuery)
			{
				if (contact != null)
				{
					Recruiter temp = FindRecruiterByContact(contact);
					if(temp != null) { recruitersByContacts.Add(temp); }
				}
			}

			if (recruitersByContacts == null || recruitersByContacts.Count <= 0){ return null; }
			return recruitersByContacts;
		}

		public static List<Recruiter> GetRecruiters()
		{
			return recruiters.Recruiters;
		}
		#endregion

		#region Contact Access Methods
		public static Contact GetContactByID(int contactId)
		{
			return contacts.GetContact(contactId);
		}

		public static List<Contact> GetContactsByIDs(params int[] contactIds)
		{
			return contacts.GetContacts(contactIds);
		}

		public static List<Contact> FindContactsByQuery(string value)
		{
			return contacts.FindContacts(value);
		}

		public static List<Contact> FindContactsByQueries(params string[] values)
		{
			return contacts.FindContacts(values);
		}
		
		public static List<Contact> FindContactsByRecruiter(int recruiterId)
		{
			return FindContactsByRecruiter(GetRecruiterByID(recruiterId));
		}
		
		public static List<Contact> FindContactsByRecruiter(Recruiter recruiterQuery)
		{
			return contacts.GetContactsFor(recruiterQuery);
		}

		public static List<Contact> FindContactsByRecruiters(params int[] recruiterIds)
		{
			Recruiter[] recruiters = new Recruiter[recruiterIds.Length];

			for (int counter = 0; counter < recruiterIds.Length; counter++)
			{
				recruiters[counter] = GetRecruiterByID(recruiterIds[counter]);
			}

			return FindContactsByRecruiters(recruiters);
		}

		public static List<Contact> FindContactsByRecruiters(params Recruiter[] recruiterQuery)
		{
			return contacts.GetContactsFor(recruiterQuery);
		}

		public static List<Contact> GetContacts()
		{
			return contacts.Contacts;
		}
		#endregion

		#region Recruiter Insert Methods

		public static bool CreateNewRecruiter(
			string firstName,
			string middleName,
			string lastName,
			string emailAddress,
			out Recruiter insert)
		{
			return recruiters.AddRecruiter(firstName, middleName, lastName, emailAddress, out insert);
		}
				
		#endregion

		#region Contact Insert Methods

		public static bool CreateNewContact(
			string firstName, 
			string middleName,
			string lastName, 
			string emailAddress,
			string company, 
			string jobTitle, 
			int recruiterId,
			out Contact insert)
		{
			return contacts.AddContact(firstName, middleName, lastName, emailAddress, company, jobTitle, GetRecruiterByID(recruiterId), out insert);
		}

		#endregion


		public static bool Remove(Recruiter recruiter)
		{
			return recruiters.Remove(recruiter);
		}

		public static bool Remove(params Recruiter[] recruiters)
		{
			if(recruiters == null || recruiters.Length <= 0) { return false; }

			bool status = false;
			foreach (Recruiter toRemove in recruiters)
			{
				status = Remove(toRemove);
			}

			return status;
		}

		public static bool Remove(Contact contact)
		{
			return contacts.Remove(contact);
		}

		public static bool Remove(params Contact[] contacts)
		{
			if (contacts == null || contacts.Length <= 0) { return false; }

			bool status = false;
			foreach (Contact toRemove in contacts)
			{
				status = Remove(toRemove);
			}

			return status;
		}
	}

	/// <summary>
	/// ContactImporter is to import a set of *.csv files.
	/// </summary>
	public static class ContactImporter
	{

	}
	
	/// <summary>
	/// ContactUploader is to upload a set of *.csv files.
	/// </summary>
	public static class ContactUploader
	{
		// Private Fields
		private static string FILEPATH;
		private static string FILENAME;
		private static IEnumerable<Contact> contacts;
		
		// Properties
		public static string Path { get { return FILEPATH; } }
		public static string Name { get { return FILENAME; } }

		// Methods
		

		/*public bool HandleFile(Stream stream)
		{
			if (stream == null) { return false; }
			else
			{
				using (StreamReader sr = new StreamReader(stream))
				{
					using (CsvReader reader = new CsvReader(sr))
					{

						//TODO

					}
				}
			}
		}*/
		

	}
}
