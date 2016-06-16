using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactsViewer.Services.Common;

namespace ContactsViewer.Services.Validation
{
	/// <summary>
	/// FieldError.cs implements IComparable
	/// and wraps the decision of whether or not a field's input
	/// is valid or not. It does not do the deciding, 
	/// instead, merely offering a container for the error.
	/// </summary>
	public class FieldError : IComparable
	{
		#region Fields.
		/// <summary>
		/// message (string) stores the error message.
		/// </summary>
		private string message; // The error message.
		#endregion

		#region Properties
		/// <summary>
		/// ErrorMessage takes a FieldName marker and replaces
		/// it with either the FieldName or FieldLabel depending
		/// on whether or not FieldLabel has a value.
		/// </summary>
		public string ErrorMessage
		{
			get
			{
				if (FieldLabel == NullType.STRING)
				{
					return message.Replace("<FieldName>", FieldName);
				}
				else
				{
					return message.Replace("<FieldName>", FieldLabel);
				}
			}
			set
			{
				this.message = value;
			}
		}
		
		/// <summary>
		/// FieldName (string) is the name of the field
		/// that triggered the validation error.
		/// </summary>
		public string FieldName	{ get; set; } // Field Name

		/// <summary>
		/// SortOrder (int) provides us with the order to sort this in.
		/// </summary>
		public int SortOrder { get; set; } // Order to be sorted.
		
		/// <summary>
		/// FieldLabel (string) is the label that the users
		/// will see this field as.
		/// </summary>
		public string FieldLabel { get; set; } // The label for the field name.
		#endregion

		#region Constructors
		/// <summary>
		/// FieldError is an empty constructor
		/// that fills its values with NullType values.
		/// </summary>
		public FieldError()
		{
			this.FieldName = NullType.STRING;
			this.FieldLabel = NullType.STRING;
			this.SortOrder = NullType.INT;
			this.ErrorMessage = NullType.STRING;
		}

		/// <summary>
		/// FieldError(string, string) takes
		/// the name of the field that threw the error
		/// and adds an additional error message
		/// describing the error.
		/// </summary>
		public FieldError(string field, string msg)
		{
			this.FieldName = field;
			this.ErrorMessage = msg;
			this.FieldLabel = NullType.STRING;
			this.SortOrder = int.MaxValue;
		}
		#endregion

		#region Service Methods
		int IComparable.CompareTo(object obj)
		{
			FieldError comparator = (FieldError)obj;
			return FieldName.CompareTo(comparator.FieldName);
		}
		#endregion
	}
}