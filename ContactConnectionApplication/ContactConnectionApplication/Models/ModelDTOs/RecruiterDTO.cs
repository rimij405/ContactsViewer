using ContactsViewer.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactsViewer.Models.ModelDTOs
{
	public class RecruiterDTO : BaseDTO
	{
		#region Constructors

		public RecruiterDTO()
			: base()
		{
			this.ID = NullType.INT;
			this.FirstName = NullType.STRING;
			this.MiddleName = NullType.STRING;
			this.LastName = NullType.STRING;
			this.EmailAddress = NullType.STRING;
			IsNew = true;
		}

		public RecruiterDTO(int id)
			: base(id)
		{
			this.ID = id;
			this.FirstName = NullType.STRING;
			this.MiddleName = NullType.STRING;
			this.LastName = NullType.STRING;
			this.EmailAddress = NullType.STRING;
			IsNew = true;
		}

		public RecruiterDTO(int id, string fName, string mName, string lName, string eMail)
			: base(id, fName, mName, lName, eMail)
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