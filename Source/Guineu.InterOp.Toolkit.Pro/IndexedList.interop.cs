using System;

namespace Guineu
{
	public partial class IndexedList<TValue> where TValue : class
	{
		private void ResizeList(Int32 slot)
		{
			var newList = new TValue[slot + Increment];
			Array.Copy( list, newList, list.Length );
			list = newList;
		}
	}
}
