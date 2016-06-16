using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactsViewer.Repository.Parsers
{
	public static class DTOParserFactory
	{
		public static DTOParser GetParser(System.Type DTOType)
		{
			switch (DTOType.Name)
			{
				case "RecruiterDTO":
					return new DTOParser_Recruiter();
				case "ContactDTO":
					return new DTOParser_Contact();
			}

			throw new Exception("Unknown Type");
		}



	}
}