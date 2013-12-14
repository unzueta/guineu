using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Guineu.Data
{
	/// <summary>
	/// Raised when a record cannot be locked
	/// </summary>
	partial class RecordInUseByAnotherUserException : ApplicationException
	{
		protected RecordInUseByAnotherUserException(SerializationInfo info,
				StreamingContext context)
			: base(info, context)
		{
		}
	}
}

