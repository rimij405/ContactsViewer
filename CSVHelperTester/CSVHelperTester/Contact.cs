using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using unirest_net;
using unirest_net.http;
using unirest_net.request;
using Newtonsoft.Json;

namespace CSVHelperTester
{
	/// <summary>
	/// Contacts.cs defines the values for a given contact in the exported LinkedIn data.
	/// It mimics the properties of a *.csv file.
	/// Allows retrival of said data.
	/// </summary>
	public class Contact
	{
		// Attributes.
		private int id;
		private string firstName;
		private string middleName;
		private string lastName;
		private string emailAddress;
		private string company;
		private string jobTitle;
		private int recruiterId;
		private Recruiter recruiter;

		// Properties.
		public int Id { get { return this.id; } }
		public string FirstName { get { return this.firstName; } }
		public string MiddleName { get { return this.middleName; } }
		public string LastName { get { return this.lastName; } }
		public string EmailAddress
		{
			get
			{
				if (this.HasEmailAddress())
				{
					return this.emailAddress;
				}
				else
				{
					return ContactsViewer.BLANK;
				}
			}
		}
		public string Company
		{
			get
			{
				if (this.HasCompany())
				{
					return this.company;
				}
				else
				{
					return ContactsViewer.BLANK;
				}
			}
		}
		public string JobTitle
		{
			get
			{
				if (this.HasOccupation())
				{
					return this.jobTitle;
				}
				else
				{
					return ContactsViewer.BLANK;
				}
			}
		}
		public int RecruiterId { get { return this.recruiterId; } }
		public Recruiter Recruiter {
			get
			{
				return this.recruiter;
			}
			set
			{
				this.recruiterId = value.Id;
				this.recruiter = value;
			}
		}

		// Constructor.
		public Contact(int idNum, string first, string middle, string last,
			string email, string org, string job, Recruiter rc )
		{
			this.id = idNum;
			this.firstName = first;
			this.middleName = middle;
			this.lastName = last;
			this.emailAddress = email;
			this.company = org;
			this.jobTitle = job;
			SetRecruiter(rc);
		}

		// Methods
		public void SetRecruiter(Recruiter r)
		{
			this.recruiterId = r.Id;
			this.recruiter = r;
		}
		public string GetProperty(string flag)
		{
			if (HasProperty(flag))
			{
				switch (flag)
				{
					case ContactsViewer.FIRST_NAME:
						return this.FirstName;
					case ContactsViewer.MIDDLE_NAME:
						return this.MiddleName;
					case ContactsViewer.LAST_NAME:
						return this.LastName;
					case ContactsViewer.EMAIL_ADDRESS:
						return this.EmailAddress;
					case ContactsViewer.ORGANIZATION:
						return this.Company;
					case ContactsViewer.OCCUPATION:
						return this.JobTitle;
					default: // Not a property.
						return ContactsViewer.BLANK;
				}
			}

			return ContactsViewer.BLANK;
		}

		public bool HasProperty(string flag)
		{
			switch (flag)
			{
				case ContactsViewer.ID:
					return HasID();
				case ContactsViewer.FIRST_NAME:
					return HasFirstName();
				case ContactsViewer.MIDDLE_NAME:
					return HasMiddleName();
				case ContactsViewer.LAST_NAME:
					return HasLastName();
				case ContactsViewer.EMAIL_ADDRESS:
					return HasEmailAddress();
				case ContactsViewer.ORGANIZATION:
					return HasCompany();
				case ContactsViewer.OCCUPATION:
					return HasOccupation();
				default: // Not a property.
					return false;
			}
		}

		public bool HasID()
		{
			if (id > 0)
			{
				return true;
			}

			return false;
		}

		public bool HasFirstName()
		{
			return ((firstName != null) && (firstName.Length > 0));
		}

		public bool HasMiddleName()
		{
			return ((middleName != null) && (middleName.Length > 0));
		}

		public bool HasLastName()
		{
			return ((lastName != null) && (lastName.Length > 0));
		}

		public bool HasEmailAddress()
		{
			return ContactsViewer.ValidateEmailAddress(emailAddress);
		}

		public bool HasCompany()
		{
			return ((company != null) && (company.Length > 0));
		}

		public bool HasOccupation()
		{
			return ((jobTitle != null) && (jobTitle.Length > 0));
		}

		public bool HasRecruiter()
		{
			return ((recruiterId != 0) && (recruiter != null));
		}

		/// <summary>
		/// This particular method compares the contacts' current recruiter,
		/// and checks to see if it matches the input recruiter.
		/// </summary>
		/// <param name="queriedRecruiter">Recruiter to check if there is a match to.</param>
		/// <returns>True if there is a match.</returns>
		public bool HasRecruiter(Recruiter queriedRecruiter)
		{
			return (this.HasRecruiter() && queriedRecruiter.Equals(recruiter));
		}

		public bool IsSame(Contact contact)
		{
			// Check first name and last name.
			if (((contact.FirstName == this.firstName))
				&& (contact.LastName == this.lastName))
			{
				// Assumption is that if there is no middle name,
				// It was a fault on the part of the file.
				// Confirmation is still necessary if a middle name is not present.
				// If both have a middle name, check for equality.
				if (contact.MiddleName == this.middleName)
				{
					// Check occupational information.
					if (!contact.HasCompany() || !this.HasCompany())
					{
						return true;
					}
					else
					{
						if (!contact.HasOccupation() || !this.HasOccupation())
						{
							return true;
						}
						else
						{
							if (contact.Company == this.Company)
							{
								return true;
							}
							else
							{
								return false;
							}
						}
					}
				}
				else
				{
					if ((!contact.HasMiddleName()) || (!this.HasMiddleName()))
					{
						// If either one is missing (or both) are misssing compare email addresses.
						if (contact.EmailAddress == this.EmailAddress)
						{
							return true;
						}
						else
						{
							if ((!contact.HasEmailAddress()) || (!this.HasEmailAddress()))
							{
								// Check occupational information.
								if (!contact.HasCompany() || !this.HasCompany())
								{
									return true;
								}
								else
								{
									if (!contact.HasOccupation() || !this.HasOccupation())
									{
										return true;
									}
									else
									{
										if (contact.Company == this.Company)
										{
											return true;
										}
										else
										{
											return false;
										}
									}
								}
							}
							else
							{
								return false;
							}
						}
					}
					else
					{
						return false;
					}
				}
			}
			else
			{
				return false;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Contact)
			{
				Contact temp = (Contact)obj;
				return IsSame(temp);
			}
			else
			{
				return base.Equals(obj);
			}
		}

		public override string ToString()
		{
			string reply = "";
			string newline = "\n";

			reply += "ID: " + this.Id;
			reply += newline;
			reply += "First Name: " + this.FirstName;
			reply += newline;
			reply += "Middle Name: " + this.MiddleName;
			reply += newline;
			reply += "Last Name: " + this.LastName;
			reply += newline;
			reply += "Email Address: " + this.emailAddress;
			reply += newline;
			reply += "Valid Email?: " + ContactsViewer.ValidateEmailAddress(this.emailAddress);
			reply += newline;
			reply += "Organization: " + this.company;
			reply += newline;
			reply += "Occupation: " + this.jobTitle;
			reply += newline;
			reply += "Recruiter ID: " + this.recruiterId;
			reply += newline;
			reply += "Recruiter: " + this.recruiter.LastName + ", " + this.recruiter.FirstName;

			if (this.recruiter.HasMiddleName())	{ reply += " " + this.recruiter.MiddleName; }
			reply += newline;

			return reply;
		}

	}
}
