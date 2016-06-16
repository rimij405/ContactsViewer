using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVHelperTester
{
	public class RecruiterManager
	{
		// Fields.
		private static int ID = 0;

		// Attributes.
		private Dictionary<string, IDictionary> referenceByCode;

		private Dictionary<int, Recruiter> recruitersById;
		private Dictionary<string, List<Recruiter>> recruitersByFirstName;
		private Dictionary<string, List<Recruiter>> recruitersByMiddleName;
		private Dictionary<string, List<Recruiter>> recruitersByLastName;
		private Dictionary<string, List<Recruiter>> recruitersByEmailAddress;

		private List<Recruiter> recruiters;

		// Properties.
		public int Count
		{
			get { return this.recruiters.Count; }
		}

		public bool IsEmpty
		{
			get
			{
				return (this.Count <= 0);
			}
		}

		public List<Recruiter> Recruiters
		{
			get { return this.recruiters; }
		}

		// Constructor.
		public RecruiterManager()
		{
			// Create the recruiter list.
			recruiters = new List<Recruiter>();

			// Create the indexes.
			recruitersById = new Dictionary<int, Recruiter>();
			recruitersByFirstName = new Dictionary<string, List<Recruiter>>();
			recruitersByMiddleName = new Dictionary<string, List<Recruiter>>();
			recruitersByLastName = new Dictionary<string, List<Recruiter>>();

			// Create the code reference dictionary.
			referenceByCode = new Dictionary<string, IDictionary>();
			referenceByCode.Add(ContactsViewer.ID, recruitersById);
			referenceByCode.Add(ContactsViewer.FIRST_NAME, recruitersByFirstName);
			referenceByCode.Add(ContactsViewer.MIDDLE_NAME, recruitersByMiddleName);
			referenceByCode.Add(ContactsViewer.LAST_NAME, recruitersByLastName);
			referenceByCode.Add(ContactsViewer.EMAIL_ADDRESS, recruitersByEmailAddress);
		}

		// Storage Methods.
		public bool AddRecruiter(
			string firstName,
			string middleName,
			string lastName,
			string emailAddress,
			out Recruiter insert)
		{
			// Operation success flag.
			bool opStatus = false;

			// Prepare id number.
			int currentId = 1;
			currentId += RecruiterManager.ID; // Don't add yet, in case operation is not successful.

			try
			{
				// Create new Recruiter.
				insert = new Recruiter(currentId, firstName, middleName, lastName, emailAddress);

				// Insert it at end of list.
				recruiters.Add(insert);

				// Set dictionary references.
				int commandIndex = 0;
				string[] commands = new string[5];

				// // Check if the insert owns a property.
				if (insert.HasID()) { commands[commandIndex] = ContactsViewer.ID; commandIndex++; }
				if (insert.HasFirstName()) { commands[commandIndex] = ContactsViewer.FIRST_NAME; commandIndex++; }
				if (insert.HasMiddleName()) { commands[commandIndex] = ContactsViewer.MIDDLE_NAME; commandIndex++; }
				if (insert.HasLastName()) { commands[commandIndex] = ContactsViewer.LAST_NAME; commandIndex++; }
				if (insert.HasEmailAddress()) { commands[commandIndex] = ContactsViewer.EMAIL_ADDRESS; commandIndex++; }

				// // Call reference to the dictionary assigning method.
				AssignReference(insert, commands);

				opStatus = true;
			}
			catch
			{
				insert = null;
				opStatus = false;
			}
			finally
			{
				if (opStatus)
				{
					RecruiterManager.ID = recruiters[(this.Count - 1)].Id;
				}
			}

			return opStatus;
		}

		#region Assign Reference Methods

		public void AssignReference(Recruiter assignee, params string[] codes)
		{
			foreach (string code in codes)
			{
				AssignReference(code, assignee);
			}
		}

		public void AssignReference(string code, Recruiter assignee)
		{
			// Check code on switch.
			switch (code)
			{
				case ContactsViewer.ID:
					Dictionary<int, Recruiter> reference = recruitersById;

					if (!reference.ContainsKey(assignee.Id))
					{
						reference.Add(assignee.Id, assignee);
					}
					else
					{
						reference[assignee.Id] = assignee;
					}
					break;
				case ContactsViewer.FIRST_NAME:
				case ContactsViewer.MIDDLE_NAME:
				case ContactsViewer.LAST_NAME:
				case ContactsViewer.EMAIL_ADDRESS:
					Dictionary<string, List<Recruiter>> referenceDictionary = (Dictionary<string, List<Recruiter>>)referenceByCode[code];
					string key = assignee.GetProperty(code);

					if (key == null || key.Length <= 0)
					{
						return; // Return if the key is an invalid string. ie., if it doesn't have a certain property.
					}
					else if (!referenceDictionary.ContainsKey(key))
					{
						List<Recruiter> temp = new List<Recruiter>();
						temp.Add(assignee);

						referenceDictionary.Add(key, temp);
					}
					else
					{
						List<Recruiter> temp = referenceDictionary[key];
						temp.Add(assignee);
					}
					break;
				default:
					return; // Queitly break reference assignment if it does not match.
			}
		}

		#endregion

		// Retrieval of Recruiter:
		public Recruiter GetRecruiter(int id)
		{
			if (recruitersById.ContainsKey(id))
			{
				return recruitersById[id];
			}
			else
			{
				return null;
			}
		}

		public List<Recruiter> GetRecruiters(int[] ids)
		{
			// Check array size.
			if (ids.Length <= 0) { return null; }

			// Create the response package.
			List<Recruiter> foundRecruiters = new List<Recruiter>();

			// Check each id to see if it exists.
			foreach (int id in ids)
			{
				Recruiter temp = GetRecruiter(id);
				if (temp != null) { foundRecruiters.Add(temp); }
			}

			if (foundRecruiters.Count <= 0) { return null; }
			return foundRecruiters;
		}

		public List<Recruiter> FindRecruiters(string value)
		{
			// Check if value is empty.
			if (value == null || value == "") { return null; }

			// Response
			List<Recruiter> response = new List<Recruiter>();

			// Check ALL codes.
			foreach (string code in ContactsViewer.CODES)
			{
				response.Concat(FindRecruiters(code, value));
			}

			if (response == null || response.Count <= 0) { return null; }
			return response;
		}

		public List<Recruiter> FindRecruiters(string[] values)
		{
			// Check size of values.
			if (values.Length <= 0) { return null; }

			// Check ALL codes for ALL values.
			List<Recruiter> response = new List<Recruiter>();

			// Investigate among each value.
			foreach (string value in values)
			{
				List<Recruiter> temp = FindRecruiters(value);
				if (temp != null) { response.Concat(temp); } // If list is not null, add. 
			}

			if (response == null || response.Count <= 0) { return null; }
			return response;
		}

		public List<Recruiter> FindRecruiters(string code, string value)
		{
			// Check if value is empty.
			if (value == null || value == "") { return null; }

			List<Recruiter> response = new List<Recruiter>();

			switch (code)
			{
				case ContactsViewer.ID:
					int idValue;
					if (!Int32.TryParse(value, out idValue))
					{
						return null;
					}
					else
					{
						Recruiter temp = GetRecruiter(idValue);
						if (temp != null) { response.Add(temp); }
					}
					break;
				case ContactsViewer.FIRST_NAME:
					if (recruitersByFirstName.ContainsKey(value))
					{
						List<Recruiter> temp = recruitersByFirstName[value];
						if (temp != null && temp.Count > 0) { response.Concat(temp); }
					}
					break;
				case ContactsViewer.MIDDLE_NAME:
					if (recruitersByMiddleName.ContainsKey(value))
					{
						List<Recruiter> temp = recruitersByMiddleName[value];
						if (temp != null && temp.Count > 0) { response.Concat(temp); }
					}
					break;
				case ContactsViewer.LAST_NAME:
					if (recruitersByLastName.ContainsKey(value))
					{
						List<Recruiter> temp = recruitersByLastName[value];
						if (temp != null && temp.Count > 0) { response.Concat(temp); }
					}
					break;
				case ContactsViewer.EMAIL_ADDRESS:
					if (recruitersByEmailAddress.ContainsKey(value))
					{
						List<Recruiter> temp = recruitersByEmailAddress[value];
						if (temp != null && temp.Count > 0) { response.Concat(temp); }
					}
					break;
			}

			if (response.Count <= 0) { return null; }
			return response;
		}

		public List<Recruiter> FindRecruiters(string code, string[] values)
		{
			// Check size of values.
			if (values.Length <= 0) { return null; }

			// Check ALL codes for ALL values.
			List<Recruiter> response = new List<Recruiter>();

			// Investigate among each value.
			foreach (string value in values)
			{
				List<Recruiter> temp = FindRecruiters(code, value);
				if (temp != null) { response.Concat(temp); } // If list is not null, add. 
			}

			if (response == null || response.Count <= 0) { return null; }
			return response;
		}

		public bool Remove(Recruiter recruiter)
		{
			return Remove(recruiter.Id);
		}

		public bool Remove(int recruitId)
		{
			// Validate id.
			if(recruitId <= 0) { return false; } // Doesn't exist. Return false.

			// Validate existence of Recruiter.
			if (this.IsEmpty || !recruitersById.ContainsKey(recruitId)) { return false; } // Doesn't exist. Return false.
			else
			{
				// Exists in the id reference dictionary.
				Recruiter toRemove = recruitersById[recruitId];

				// Get list of associated contacts.
				List<Contact> contactsToRemove = ContactsViewer.FindContactsByRecruiter(toRemove);

				// Remove contacts.
				ContactsViewer.Remove(contactsToRemove.ToArray<Contact>());

				// Check to see if the recruiter exists in the other reference directories.
				List<Recruiter> survey;
				string key;
				int idKey;
				// // Remove Email Address index.
				if (recruitersByEmailAddress.ContainsKey(toRemove.EmailAddress))
				{
					if (toRemove.HasEmailAddress())
					{
						key = toRemove.EmailAddress;
						survey = recruitersByEmailAddress[key];
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { recruitersByEmailAddress.Remove(key); }
					}
				}
				// // Remove Last Name index.
				if (recruitersByLastName.ContainsKey(toRemove.LastName))
				{
					if (toRemove.HasLastName())
					{
						key = toRemove.LastName;
						survey = recruitersByLastName[key];
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { recruitersByLastName.Remove(key); }
					}
				}
				// // Remove Middle Name index.
				if (recruitersByMiddleName.ContainsKey(toRemove.MiddleName))
				{
					if (toRemove.HasMiddleName())
					{
						key = toRemove.MiddleName;
						survey = recruitersByMiddleName[key];
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { recruitersByMiddleName.Remove(key); }
					}
				}
				// // Remove First Name index.
				if (recruitersByFirstName.ContainsKey(toRemove.FirstName))
				{
					if (toRemove.HasFirstName())
					{
						key = toRemove.FirstName;
						survey = recruitersByFirstName[key];
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { recruitersByFirstName.Remove(key); }
					}
				}
				// // Remove ID index.
				if (recruitersById.ContainsKey(toRemove.Id))
				{
					if (toRemove.HasID())
					{
						idKey = toRemove.Id;
						survey = recruiters;
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { recruitersById.Remove(idKey); }
					}
				}

				return true;
			}
		}
	}
}
