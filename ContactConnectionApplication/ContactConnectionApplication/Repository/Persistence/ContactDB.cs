using ContactsViewer.Models.ModelDTOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ContactsViewer.Repository.Persistence
{
	public class ContactDB : DALBase
	{
		#region Methods

		public static ContactDTO GetRecruiterByID(int id)
		{
			SqlCommand command = GetDbSprocCommand("Recruiter_GetByID");
			command.Parameters.Add(CreateParameter("@RecruiterID", id));
			return GetSingleDTO<ContactDTO>(ref command);
		}
		
		#endregion


	}
}