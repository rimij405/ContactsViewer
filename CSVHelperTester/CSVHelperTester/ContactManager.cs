using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using CsvHelper;

namespace CSVHelperTester
{
	public class ContactManager
	{
		// Fields.
		private static int ID = 0;

		// Attributes.
		private Dictionary<string, IDictionary> referenceByCode;

		private Dictionary<int, Contact> contactsById;
		private Dictionary<string, List<Contact>> contactsByFirstName;
		private Dictionary<string, List<Contact>> contactsByMiddleName;
		private Dictionary<string, List<Contact>> contactsByLastName;
		private Dictionary<string, List<Contact>> contactsByEmailAddress;
		private Dictionary<string, List<Contact>> contactsByCompany;
		private Dictionary<string, List<Contact>> contactsByOccupation;
		private Dictionary<int, List<Contact>> contactsByRecruiterId;

		private List<Contact> contacts;

		// Properties.
		public int Count
		{
			get { return this.contacts.Count; }
		}

		public bool IsEmpty
		{
			get
			{
				return (this.Count <= 0);
			}
		}

		public List<Contact> Contacts
		{
			get { return this.contacts; }
		}

		// Constructor.
		public ContactManager()
		{
			// Create the contact list.
			contacts = new List<Contact>();

			// Create the indexes.
			contactsById = new Dictionary<int, Contact>();
			contactsByFirstName = new Dictionary<string, List<Contact>>();
			contactsByMiddleName = new Dictionary<string, List<Contact>>();
			contactsByLastName = new Dictionary<string, List<Contact>>();
			contactsByEmailAddress = new Dictionary<string, List<Contact>>();
			contactsByCompany = new Dictionary<string, List<Contact>>();
			contactsByOccupation = new Dictionary<string, List<Contact>>();
			contactsByRecruiterId = new Dictionary<int, List<Contact>>();

			// Create the code reference dictionary.
			referenceByCode = new Dictionary<string, IDictionary>();
			referenceByCode.Add(ContactsViewer.ID, contactsById);
			referenceByCode.Add(ContactsViewer.FIRST_NAME, contactsByFirstName);
			referenceByCode.Add(ContactsViewer.MIDDLE_NAME, contactsByMiddleName);
			referenceByCode.Add(ContactsViewer.LAST_NAME, contactsByLastName);
			referenceByCode.Add(ContactsViewer.EMAIL_ADDRESS, contactsByEmailAddress);
			referenceByCode.Add(ContactsViewer.ORGANIZATION, contactsByCompany);
			referenceByCode.Add(ContactsViewer.OCCUPATION, contactsByOccupation);
			referenceByCode.Add(ContactsViewer.RECRUITER_ID, contactsByRecruiterId);
		}

		// Storage Methods.
		public bool AddContact(
			string firstName,
			string middleName,
			string lastName,
			string emailAddress,
			string company,
			string jobTitle,
			Recruiter recruiter,
			out Contact insert)
		{
			// Operation success flag.
			bool opStatus = false;

			// Prepare id number.
			int currentId = 1;
			currentId += ContactManager.ID; // Don't add yet, in case operation is not successful.

			try
			{
				// Create new Contact.
				insert = new Contact(currentId, firstName, middleName, lastName, emailAddress, company, jobTitle, recruiter);

				// Insert it at end of list.
				contacts.Add(insert);

				// Set dictionary references.
				int commandIndex = 0;
				string[] commands = new string[8];

				// // Check if the insert owns a property.
				if (insert.HasID()) { commands[commandIndex] = ContactsViewer.ID; commandIndex++; }
				if (insert.HasFirstName()) { commands[commandIndex] = ContactsViewer.FIRST_NAME; commandIndex++; }
				if (insert.HasMiddleName()) { commands[commandIndex] = ContactsViewer.MIDDLE_NAME; commandIndex++; }
				if (insert.HasLastName()) { commands[commandIndex] = ContactsViewer.LAST_NAME; commandIndex++; }
				if (insert.HasEmailAddress()) { commands[commandIndex] = ContactsViewer.EMAIL_ADDRESS; commandIndex++; }
				if (insert.HasCompany()) { commands[commandIndex] = ContactsViewer.ORGANIZATION; commandIndex++; }
				if (insert.HasOccupation()) { commands[commandIndex] = ContactsViewer.OCCUPATION; commandIndex++; }
				if (insert.HasRecruiter()) { commands[commandIndex] = ContactsViewer.RECRUITER_ID; commandIndex++; }

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
					ContactManager.ID = contacts[(this.Count - 1)].Id;
				}
			}

			return opStatus;
		}

		#region Assign Reference Methods

		public void AssignReference(Contact assignee, params string[] codes)
		{
			foreach (string code in codes)
			{
				AssignReference(code, assignee);
			}
		}

		public void AssignReference(string code, Contact assignee)
		{
			// Check code on switch.
			switch (code)
			{
				case ContactsViewer.ID:
					Dictionary<int, Contact> reference = contactsById;

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
				case ContactsViewer.ORGANIZATION:
				case ContactsViewer.OCCUPATION:
					Dictionary<string, List<Contact>> referenceDictionary = (Dictionary<string, List<Contact>>)referenceByCode[code];
					string key = assignee.GetProperty(code);

					if (key == null || key.Length <= 0)
					{
						return; // Return if the key is an invalid string. ie., if it doesn't have a certain property.
					}
					else if (!referenceDictionary.ContainsKey(key))
					{
						List<Contact> temp = new List<Contact>();
						temp.Add(assignee);

						referenceDictionary.Add(key, temp);
					}
					else
					{
						List<Contact> temp = referenceDictionary[key];
						temp.Add(assignee);
					}
					break;
				case ContactsViewer.RECRUITER_ID:
					Dictionary<int, List<Contact>> recruiterReference = (Dictionary<int, List<Contact>>)referenceByCode[code];
					int keyID = assignee.RecruiterId;

					if (keyID <= 0)
					{
						return; // Return if the key is an invalid value.
					}
					else if (!recruiterReference.ContainsKey(keyID))
					{
						List<Contact> temp = new List<Contact>();
						temp.Add(assignee);

						recruiterReference.Add(keyID, temp);
					}
					else
					{
						List<Contact> temp = recruiterReference[keyID];
						temp.Add(assignee);
					}
					break;
				default:
					return; // Queitly break reference assignment if it does not match.
			}
		}

		#endregion

		// Retrieval of Contact:
		public Contact GetContact(int id)
		{
			if (contactsById.ContainsKey(id))
			{
				return contactsById[id];
			}
			else
			{
				return null;
			}
		}

		public List<Contact> GetContacts(int[] ids)
		{
			// Check array size.
			if (ids.Length <= 0) { return null; }

			// Create the response package.
			List<Contact> foundContacts = new List<Contact>();

			// Check each id to see if it exists.
			foreach (int id in ids)
			{
				Contact temp = GetContact(id);
				if (temp != null) { foundContacts.Add(temp); }
			}

			if (foundContacts.Count <= 0) { return null; }
			return foundContacts;
		}

		public List<Contact> FindContacts(string value)
		{
			// Check if value is empty.
			if (value == null || value == "") { return null; }

			// Response
			List<Contact> response = new List<Contact>();

			// Check ALL codes.
			foreach (string code in ContactsViewer.CODES)
			{
				response.Concat(FindContacts(code, value));
			}

			if (response == null || response.Count <= 0) { return null; }
			return response;
		}

		public List<Contact> FindContacts(string[] values)
		{
			// Check size of values.
			if (values.Length <= 0) { return null; }

			// Check ALL codes for ALL values.
			List<Contact> response = new List<Contact>();

			// Investigate among each value.
			foreach (string value in values)
			{
				List<Contact> temp = FindContacts(value);
				if (temp != null) { response.Concat(temp); } // If list is not null, add. 
			}

			if (response == null || response.Count <= 0) { return null; }
			return response;
		}

		public List<Contact> FindContacts(string code, string value)
		{
			// Check if value is empty.
			if (value == null || value == "") { return null; }

			List<Contact> response = new List<Contact>();

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
						Contact temp = GetContact(idValue);
						if (temp != null) { response.Add(temp); }
					}
					break;
				case ContactsViewer.FIRST_NAME:
					if (contactsByFirstName.ContainsKey(value))
					{
						List<Contact> temp = contactsByFirstName[value];
						if (temp != null && temp.Count > 0) { response.Concat(temp); }
					}
					break;
				case ContactsViewer.MIDDLE_NAME:
					if (contactsByMiddleName.ContainsKey(value))
					{
						List<Contact> temp = contactsByMiddleName[value];
						if (temp != null && temp.Count > 0) { response.Concat(temp); }
					}
					break;
				case ContactsViewer.LAST_NAME:
					if (contactsByLastName.ContainsKey(value))
					{
						List<Contact> temp = contactsByLastName[value];
						if (temp != null && temp.Count > 0) { response.Concat(temp); }
					}
					break;
				case ContactsViewer.EMAIL_ADDRESS:
					if (contactsByEmailAddress.ContainsKey(value))
					{
						List<Contact> temp = contactsByEmailAddress[value];
						if (temp != null && temp.Count > 0) { response.Concat(temp); }
					}
					break;
			}

			if (response.Count <= 0) { return null; }
			return response;
		}

		public List<Contact> FindContacts(string code, string[] values)
		{
			// Check size of values.
			if (values.Length <= 0) { return null; }

			// Check ALL codes for ALL values.
			List<Contact> response = new List<Contact>();

			// Investigate among each value.
			foreach (string value in values)
			{
				List<Contact> temp = FindContacts(code, value);
				if (temp != null) { response.Concat(temp); } // If list is not null, add. 
			}

			if (response == null || response.Count <= 0) { return null; }
			return response;
		}
		
		public List<Contact> GetContactsFor(Recruiter query)
		{
			int recId = query.Id;
			List<Contact> response = null;

			// If the recruiter has a list of contacts, retrieve it.
			if (contactsByRecruiterId.ContainsKey(recId))
			{
				response = contactsByRecruiterId[recId];
			}

			if (response == null || response.Count <= 0) { return null; }
			return response;
		}

		public List<Contact> GetContactsFor(Recruiter[] queries)
		{
			// Validate incoming parameter.
			if (queries == null || queries.Length <= 0) { return null; }

			// Set up list.
			List<Contact> response = new List<Contact>();

			// Set up to loop.
			foreach (Recruiter query in queries)
			{
				response.Concat(GetContactsFor(query));
			}

			if (response == null || response.Count <= 0) { return null; }
			return response;
		}

		public bool Remove(Contact contact)
		{
			return Remove(contact.Id);
		}

		public bool Remove(int contactId)
		{
			// Validate id.
			if (contactId <= 0) { return false; } // Doesn't exist. Return false.

			// Validate existence of Contact.
			if (this.IsEmpty || !contactsById.ContainsKey(contactId)) { return false; } // Doesn't exist. Return false.
			else
			{
				// Exists in the id reference dictionary.
				Contact toRemove = contactsById[contactId];

				// Check to see if the contact exists in the other reference directories.
				// REMOVE ALL REFERENCES IN REVERSE ORDER FROM HOW THEY WERE PLACED.
				List<Contact> survey;
				string key;
				int idKey;
				// // Remove Recruiter ID index.
				if (contactsByRecruiterId.ContainsKey(toRemove.RecruiterId))
				{
					if (toRemove.HasRecruiter())
					{
						idKey = toRemove.RecruiterId;
						survey = contactsByRecruiterId[idKey];
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { contactsByRecruiterId.Remove(idKey); }
					}
				}
				// // Remove Occupation index.
				if (contactsByOccupation.ContainsKey(toRemove.JobTitle))
				{
					if (toRemove.HasOccupation())
					{
						key = toRemove.JobTitle;
						survey = contactsByOccupation[key];
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { contactsByOccupation.Remove(key); }
					}
				}
				// // Remove Company index.
				if (contactsByCompany.ContainsKey(toRemove.Company))
				{
					if (toRemove.HasCompany())
					{
						key = toRemove.Company;
						survey = contactsByCompany[key];
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { contactsByCompany.Remove(key); }
					}
				}
				// // Remove Email Address index.
				if (contactsByEmailAddress.ContainsKey(toRemove.EmailAddress))
				{
					if (toRemove.HasEmailAddress())
					{
						key = toRemove.EmailAddress;
						survey = contactsByEmailAddress[key];
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { contactsByEmailAddress.Remove(key); }
					}
				}
				// // Remove Last Name index.
				if (contactsByLastName.ContainsKey(toRemove.LastName))
				{
					if (toRemove.HasLastName())
					{
						key = toRemove.LastName;
						survey = contactsByLastName[key];
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { contactsByLastName.Remove(key); }
					}
				}
				// // Remove Middle Name index.
				if (contactsByMiddleName.ContainsKey(toRemove.MiddleName))
				{
					if (toRemove.HasMiddleName())
					{
						key = toRemove.MiddleName;
						survey = contactsByMiddleName[key];
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { contactsByMiddleName.Remove(key); }
					}
				}
				// // Remove First Name index.
				if (contactsByFirstName.ContainsKey(toRemove.FirstName))
				{
					if (toRemove.HasFirstName())
					{
						key = toRemove.FirstName;
						survey = contactsByFirstName[key];
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { contactsByFirstName.Remove(key); }
					}
				}
				// // Remove ID index.
				if (contactsById.ContainsKey(toRemove.Id))
				{
					if (toRemove.HasID())
					{
						idKey = toRemove.Id;
						survey = contacts;
						if (survey.Contains(toRemove)) { survey.Remove(toRemove); }
						if (survey.Count <= 0) { contactsById.Remove(idKey); }
					}
				}

				return true;
			}
		}
	}
}
