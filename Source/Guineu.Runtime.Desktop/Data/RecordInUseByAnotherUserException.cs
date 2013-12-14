using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu.Data
{
	/// <summary>
	/// Raised when a record cannot be locked
	/// </summary>
	partial class RecordInUseByAnotherUserException : ApplicationException
	{
		public RecordInUseByAnotherUserException()
		{
		}
		public RecordInUseByAnotherUserException(string message)
			: base(message)
		{
		}
		public RecordInUseByAnotherUserException(string message, Exception innerException) :
			base(message, innerException)
		{
		}
	}
}
