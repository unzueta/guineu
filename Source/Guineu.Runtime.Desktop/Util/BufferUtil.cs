using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu.Util
{
	static class BufferUtil
	{
		public static Int32 Compare(Byte[] left, Byte[] right, Boolean exact)
		{
			// The default value is returned when both arrays are identical
			// up to the length of the shorter one.
			Int32 defaultValue;
			Int32 Length;
			if (left.Length < right.Length)
			{
				defaultValue = -1;
				Length = left.Length;
			}
			else if (left.Length > right.Length)
			{
				// null bytes at the end of the right string are still ignored. 
				if (exact)
				{
					defaultValue = 1;
					Boolean justNull = true;
					for (Int32 i = right.Length; i < left.Length; i++)
					{
						if (left[i] != '\0')
						{
							justNull = false;
							break;
						}
					}
					if (justNull)
					{
						defaultValue = 0;
						Length = right.Length;
					}
				}
				else
					defaultValue = 0;
				Length = right.Length;
			}
			else
			{
				defaultValue = 0;
				Length = right.Length;
			}

			// Compare all bytes up to the length of the shorter array
			for( Int32 i=0; i<Length; i++)
			{
				if( left[i] < right[i])
					return -1;
				else if(left[i] > right[i])
					return 1;
			}

			return defaultValue;
		}
	}
}
