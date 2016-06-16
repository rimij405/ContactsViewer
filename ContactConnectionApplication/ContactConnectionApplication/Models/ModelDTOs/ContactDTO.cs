using ContactsViewer.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactsViewer.Models.ModelDTOs
{
	public class ContactDTO : BaseDTO
	{
		#region Fields

		private string company;
		private string jobTitle;
		private int recruiterId;
		private RecruiterDTO recruiter;

		#endregion

		#region Properties

		public string Company
		{
			get { return this.company; }
			set { this.company = value; }
		}

		public string JobTitle
		{
			get { return this.jobTitle; }
			set { this.jobTitle = value; }
		}

		public int RecruiterID
		{
			get { return this.recruiterId; }
			set { this.recruiterId = value; }
		}
		public RecruiterDTO Recruiter
		{
			get { return this.recruiter; }
			set { this.recruiter = value; }
		}

		#endregion

		#region Constructors

		public ContactDTO() : base()
		{
			this.ID = NullType.INT;
			this.FirstName = NullType.STRING;
			this.MiddleName = NullType.STRING;
			this.LastName = NullType.STRING;
			this.EmailAddress = NullType.STRING;
			this.company = NullType.STRING;
			this.jobTitle = NullType.STRING;
			this.recruiterId = NullType.INT;
			this.recruiter = NullType.RECRUITER;
		}

		public ContactDTO(int id) : base(id)
		{
			this.ID = id;
			this.FirstName = NullType.STRING;
			this.MiddleName = NullType.STRING;
			this.LastName = NullType.STRING;
			this.EmailAddress = NullType.STRING;
			this.company = NullType.STRING;
			this.jobTitle = NullType.STRING;
			this.recruiterId = NullType.INT;
			this.recruiter = NullType.RECRUITER;
		}

		public ContactDTO(int id, string fName, string mName, string lName, string eMail, string com, string job, int recId, RecruiterDTO rec) 
			: base(id, fName, mName, lName, eMail)
		{
			this.ID = id;
			this.FirstName = fName;
			this.MiddleName = mName;
			this.LastName = lName;
			this.EmailAddress = eMail;
			this.company = com;
			this.jobTitle = job;
			this.recruiterId = recId;
			this.recruiter = rec;
			IsNew = true;
		}

		public ContactDTO(int id, string fName, string mName, string lName, string eMail, string com, string job, RecruiterDTO rec)
			: base(id, fName, mName, lName, eMail)
		{
			this.ID = id;
			this.FirstName = fName;
			this.MiddleName = mName;
			this.LastName = lName;
			this.EmailAddress = eMail;
			this.company = com;
			this.jobTitle = job;
			this.recruiterId = rec.ID;
			this.recruiter = rec;
			IsNew = true;
		}

		#endregion
	}
}