using ContactsViewer.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ContactsViewer.Repository.Parsers
{
	public abstract class DTOParser
	{
		// Methods.
		public abstract IBaseDTO PopulateDTO(SqlDataReader reader);
		public abstract void PopulateOrdinals(SqlDataReader reader);

	}
}