// Silverlight mockup layer

using System;

namespace Guineu
{
	public class ApplicationException : Exception
	{
		public ApplicationException(string message, Exception innerException)
			: base(message, innerException)
		{ }
		public ApplicationException(string message)
			: base(message)
		{ }
		public ApplicationException()
		{ }
	}
}