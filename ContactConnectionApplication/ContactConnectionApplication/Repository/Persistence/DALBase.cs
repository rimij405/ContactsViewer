using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using ContactsViewer.Services.Common;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using ContactsViewer.Repository.Parsers;

namespace ContactsViewer.Repository.Persistence
{
	public abstract class DALBase
	{
		// Properties.
		protected static string ConnectionString
		{
			get { return ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString; }
		}

		#region Methods.
		protected static SqlParameter CreateNullParameter(string name, SqlDbType paramType)
		{
			SqlParameter parameter = new SqlParameter();
			parameter.SqlDbType = paramType;
			parameter.ParameterName = name;
			parameter.Value = null;
			parameter.Direction = ParameterDirection.Input;
			return parameter;
		}

		protected static SqlParameter CreateNullParameter(string name, SqlDbType paramType, int size)
		{
			SqlParameter parameter = new SqlParameter();
			parameter.SqlDbType = paramType;
			parameter.ParameterName = name;
			parameter.Size = size;
			parameter.Value = null;
			parameter.Direction = ParameterDirection.Input;
			return parameter;
		}

		protected static SqlParameter CreateOutputParameter(string name, SqlDbType paramType)
		{
			SqlParameter parameter = new SqlParameter();
			parameter.SqlDbType = paramType;
			parameter.ParameterName = name;
			parameter.Direction = ParameterDirection.Output;
			return parameter;
		}

		protected static SqlParameter CreateOutputParameter(string name, SqlDbType paramType, int size)
		{
			SqlParameter parameter = new SqlParameter();
			parameter.SqlDbType = paramType;
			parameter.Size = size;
			parameter.ParameterName = name;
			parameter.Direction = ParameterDirection.Output;
			return parameter;
		}

		protected static SqlParameter CreateParameter(string name, int value)
		{
			if (value.Equals((ContactsViewer.Services.Common.NullType.INT)))
			{
				return CreateNullParameter(name, SqlDbType.Int);
			}
			else
			{
				SqlParameter parameter = new SqlParameter();
				parameter.SqlDbType = SqlDbType.Int;
				parameter.ParameterName = name;
				parameter.Value = value;
				parameter.Direction = ParameterDirection.Input;
				return parameter;
			}
		}

		protected static SqlParameter CreateParameter(string name, string value, int size)
		{
			if (value == ContactsViewer.Services.Common.NullType.STRING)
			{
				return CreateNullParameter(name, SqlDbType.NVarChar);
			}
			else
			{
				SqlParameter parameter = new SqlParameter();
				parameter.SqlDbType = SqlDbType.NVarChar;
				parameter.Size = size;
				parameter.ParameterName = name;
				parameter.Value = value;
				parameter.Direction = ParameterDirection.Input;
				return parameter;
			}
		}

		/// <summary>
		/// GetDbConnection
		/// Gets the connection to the database.
		/// </summary>
		/// <returns>Returns a SQLConnection object.</returns>
		protected static SqlConnection GetDbConnection()
		{
			return new SqlConnection(ConnectionString);
		}

		/// <summary>
		/// GetDbSQLCommand
		/// Gets a SqlCommand object based on sqlQuery.
		/// </summary>
		/// <param name="sqlQuery">The SQL query.</param>
		/// <returns>Returns an SqlCommand object.</returns>
		public static SqlCommand GetDbSQLCommand(string sqlQuery)
		{
			SqlCommand command = new SqlCommand();
			command.Connection = GetDbConnection();
			command.CommandType = CommandType.Text;
			command.CommandText = sqlQuery;
			return command;
		}

		/// <summary>
		/// GetDbSprocCommand
		/// Returns a stored procedure.
		/// </summary>
		/// <param name="sprocName">The command to be used</param>
		/// <returns>Returns an SqlCommand object.</returns>
		public SqlCommand GetDbSprocCommand(string sprocName)
		{
			SqlCommand command = new SqlCommand(sprocName);
			command.Connection = GetDbConnection();
			command.CommandType = CommandType.StoredProcedure;
			return command;
		}

		/// <summary>
		/// GetSingleDTO takes a command object, opens its connection,
		/// reads it, tries to parse a DTO from it, and then returns it.
		/// As a generic method, it doesn't need to be rewritten everytime.
		/// </summary>
		/// <typeparam name="T">IBaseDTO generics.</typeparam>
		/// <param name="command">A command object that we'll get the DTO from.</param>
		/// <returns>This will return a parsed DTO object, or null.</returns>
		public static T GetSingleDTO<T>(ref SqlCommand command) where T : IBaseDTO
		{
			T dto = default(T);
			try
			{
				command.Connection.Open();
				SqlDataReader reader = command.ExecuteReader();
				if (reader.HasRows)
				{
					reader.Read();
					DTOParser parser = DTOParserFactory.GetParser(typeof(T));
					parser.PopulateOrdinals(reader);
					dto = (T) parser.PopulateDTO(reader);
					reader.Close();
				}
				else
				{
					// There's no data, so return this.
					dto = default(T);
				}
			}
			catch(Exception e)
			{
				throw new Exception("Error populating data: " + e.Message, e);
			}
			finally
			{
				command.Connection.Close();
				command.Connection.Dispose();
			}

			return dto;
		}
		

		public static List<T> GetDTOList<T>(ref SqlCommand command) where T : IBaseDTO
		{
			List<T> dtoList = new List<T>();
			try
			{
				command.Connection.Open();
				SqlDataReader reader = command.ExecuteReader();
				if (reader.HasRows)
				{
					reader.Read();
					DTOParser parser = DTOParserFactory.GetParser(typeof(T));
					parser.PopulateOrdinals(reader);
					while (reader.Read())
					{
						T dto = default(T);
						dto = (T) parser.PopulateDTO(reader);
						dtoList.Add(dto);
					}
					reader.Close();
				}
				else
				{
					// There's no data, so return this.
					dtoList = null;
				}
			}
			catch (Exception e)
			{
				throw new Exception("Error populating data: " + e.Message, e);
			}
			finally
			{
				command.Connection.Close();
				command.Connection.Dispose();
			}

			return dtoList;
		}


		#endregion

	}
}