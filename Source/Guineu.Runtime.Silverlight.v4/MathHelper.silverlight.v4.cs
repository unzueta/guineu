using System;
using System.Collections.Generic;

namespace Guineu
{
	static class MathHelper
	{
		public static Double Round(Double val)
		{
			// TODO: Check how the compact framework rounds values.
			return Math.Round(val);
		}
		public static Int32 RoundToInt(Double val)
		{
			return (Int32)Math.Round(val);
		}
	}
}