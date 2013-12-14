using System;

namespace Guineu
{
	public partial class IndexedList<TValue> where TValue : class
	{
		private void ResizeList(Int32 slot)
		{
			Array.Resize<TValue>(ref list, slot + Increment);
		}
	}
}
