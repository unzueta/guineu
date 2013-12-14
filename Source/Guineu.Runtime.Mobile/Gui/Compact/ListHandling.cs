using System;
using System.Collections;

namespace Guineu.Gui.Compact
{
	class ListHandling
	{
		static public void LoadListData(IList items, String rowSource, Int32 rowSourceType)
		{
			switch (rowSourceType)
			{
				case 0:
					// do nothing
					break;
				case 1:
					LoadListValue(items, rowSource);
					break;
			}
		}

		/// <summary>
		/// Loads the list with the values stored in RowSource.
		/// </summary>
		static void LoadListValue(IList items, String rowSource)
		{
			if (!String.IsNullOrEmpty(rowSource))
			{
				items.Clear();
				var rs = rowSource.TrimEnd(new[] { ',', ' ' });
				foreach (var s in rs.Split(','))
					items.Add(s);
			}
		}

	}
}
