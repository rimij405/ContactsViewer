using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft;

namespace CSVHelperTester
{
	/// <summary>
	/// Recruiter.cs represents the *.csv data for a particular recruiter.
	/// It allows retrieval, but not printing, of the data.
	/// </summary>
	public class Recruiter
	{
		// Attributes.
		private int id;
		private string firstName;
		private string middleName;
		private string lastName;
		private string emailAddress;

		// Properties.
		public int Id { get { return this.id; } }
		public string FirstName { get { return this.firstName; } }
		public string MiddleName { get { return this.middleName; } }
		public string LastName { get { return this.lastName; } }
		public string EmailAddress {
			get {
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

		// Constructor.
		public Recruiter(int idNum, string first, string middle, string last, string email)
		{
			this.id = idNum;
			this.firstName = first;
			this.middleName = middle;
			this.lastName = last;
			this.emailAddress = email;
		}

		// Methods
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
		
		public bool IsSame(Recruiter rec)
		{
			// Check first name and last name.
			if (((rec.FirstName == this.firstName))
				&& (rec.LastName == this.lastName))
			{
				// Assumption is that if there is no middle name,
				// It was a fault on the part of the file.
				// Confirmation is still necessary if a middle name is not present.
				// If both have a middle name, check for equality.
				if (rec.MiddleName == this.middleName)
				{
					return true;
				}
				else
				{
					return ((!rec.HasMiddleName()) || (!this.HasMiddleName()));
				}
			}
			else
			{
				return false;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Recruiter)
			{
				Recruiter temp = (Recruiter)obj;
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

			reply += "First Name: " + this.FirstName;
			reply += newline;
			reply += "Middle Name: " + this.MiddleName;
			reply += newline;
			reply += "Last Name: " + this.LastName;
			reply += newline;
			reply += "Email Address: " + this.emailAddress;
			reply += newline;
			reply += "ID: " + this.Id;
			reply += newline;

			return reply;			
		}

	}
}
