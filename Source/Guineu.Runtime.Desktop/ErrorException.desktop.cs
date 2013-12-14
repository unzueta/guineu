using System;
using System.Runtime.Serialization;

namespace Guineu
{
	partial class ErrorException : Exception
	{
		protected ErrorException(SerializationInfo info,
				 StreamingContext context)
			: base(info, context)
		{
		}
	}
}