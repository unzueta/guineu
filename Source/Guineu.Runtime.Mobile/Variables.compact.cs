using System;

namespace Guineu
{
	partial class ArrayMember : ValueMember
	{
		internal void Dimension(Int64 d1)
		{
			SetDimension(d1);
			val = Resize(val, (Int32)d1);
		}

		internal void Dimension(Int64 d1, Int64 d2)
		{
			SetDimension(d1, d2);
			val = Resize(val, (Int32)(d1 * d2));
		}

		static ValueMember[] Resize( ValueMember[] oldArray, Int32 newSize)
		{
			var newArray = new ValueMember[newSize];
			int preserveLength = Math.Min(oldArray.Length, newSize);
			if (preserveLength > 0)
				Array.Copy(oldArray, newArray, preserveLength);
			return newArray;
		}
	}
}