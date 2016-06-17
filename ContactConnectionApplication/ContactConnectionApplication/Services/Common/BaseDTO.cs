using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactsViewer.Services.Common
{
	public abstract class BaseDTO : IBaseDTO
	{
		#region Fields

		private int id;
		private string firstName;
		private string middleName;
		private string lastName;
		private string emailAddress;

		#endregion

		#region Properties

		[Key]
		public int ID
		{
			get { return this.id; }
			set { this.id = value; }
		}

		[Required]
		public string FirstName
		{
			get { return this.firstName; }
			set { this.firstName = value; }
		}

		public string MiddleName
		{
			get { return this.middleName; }
			set { this.middleName = value; }
		}

		[Required]
		public string LastName
		{
			get { return this.lastName; }
			set { this.lastName = value; }
		}

		[Required]
		public string EmailAddress
		{
			get { return this.emailAddress; }
			set { this.emailAddress = value; }
		}

		public bool IsNew { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// BaseDTO is the base data transfer object
		/// for Recruiters and Contacts.
		/// </summary>
		public BaseDTO()
		{
			this.ID = NullType.INT;
			this.FirstName = NullType.STRING;
			this.MiddleName = NullType.STRING;
			this.LastName = NullType.STRING;
			this.EmailAddress = NullType.STRING;
			IsNew = true;
		}

		public BaseDTO(int id)
		{
			this.ID = id;
			this.FirstName = NullType.STRING;
			this.MiddleName = NullType.STRING;
			this.LastName = NullType.STRING;
			this.EmailAddress = NullType.STRING;
			IsNew = true;
		}

		public BaseDTO(int id,
			string fName,
			string mName,
			string lName,
			string eMail)
		{
			this.ID = id;
			this.FirstName = fName;
			this.MiddleName = mName;
			this.LastName = lName;
			this.EmailAddress = eMail;
			IsNew = true;
		}

		#endregion
	}
}