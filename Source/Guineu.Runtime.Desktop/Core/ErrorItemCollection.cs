using System.Collections.Generic;

namespace Guineu.Core
{
	public class ErrorItemCollection : List<ErrorItemBase>
	{
		public void Set(ErrorItemBase error)
		{
			Clear();
			Add(error);
		}
}
}
