using System;

namespace Guineu
{
	public partial class IndexedList<TValue> where TValue : class
	{
		TValue[] list;
		const Int32 Increment = 10;

		public IndexedList()
		{
			list = new TValue[10];
		}

		/// <summary>
		/// Inserts an item into the list at the next available slot.
		/// </summary>
		/// <param name="item"></param>
		public Int32 Add(TValue item)
		{
			Int32 slot = GetNextFreeSlot();
			list[slot] = item;
			return slot;
		}

		public Int32 GetNextFreeSlot()
		{
			// Find an unallocated slot. We skip the first item, because this
			// list is one-based.
			Int32 slot = 0;
			for (Int32 check = 1; check < list.Length; check++)
			{
				if (list[check] == null)
				{
					slot = check;
					break;
				}
			}

			// Slot hasn't changed. The list is full.
			if (slot == 0)
			{
				slot = list.Length;
				ResizeList(slot);
			}
			return slot;
		}

		void CheckSize(Int32 minSize)
		{
			if (list.Length < minSize)
				ResizeList(minSize);
		}

		/// <summary>
		/// Inserts an item into the list in the specified slot
		/// </summary>
		/// <param name="item"></param>
		/// <param name="destSlot"></param>
		public void Add(TValue item, Int32 destSlot)
		{
			CheckSize(destSlot);
			list[destSlot] = item;
		}

		virtual public TValue this[Int32 item]
		{
			get { return list[item]; }
			set { list[item] = value; }
		}

		public Boolean IsValid(Int32 item)
		{
			if (item <= 0 || item >= list.Length)
			{
				return false;
			}
			if(list[item] == null)
			{
				return false;
			}
			return true;
		}

		public Int32 Length
		{
			get { return list.Length; }
		}
	}
}
