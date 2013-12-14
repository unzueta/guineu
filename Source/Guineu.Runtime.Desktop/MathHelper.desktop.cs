using System;
using System.Collections.Generic;

namespace Guineu
{
	static class MathHelper
	{
		public static Double Round(Double val)
		{
			return Math.Round(val, MidpointRounding.AwayFromZero);
		}
		public static Int32 RoundToInt(Double val)
		{
			return (Int32) Math.Round(val, MidpointRounding.AwayFromZero);
		}
	}
}