using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu
{
	public partial class IndexedList<TValue> where TValue : class
	{
		private void ResizeList(Int32 Slot)
		{
			TValue[] newList = new TValue[Slot + Increment];
			Array.Copy( list, newList, list.Length );
			list = newList;
		}
	}
}
