using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu.Util
{
	static class EnumUtil
	{
		static public Boolean IsSet(Int32 flags, Int32 bit)
		{
			return ((flags & bit) == bit);
		}
	}

	static class Enum<T> where T : IConvertible
	{
		static public Boolean IsSet(T flags, T bit)
		{
			return ((flags.ToInt32(null) & bit.ToInt32(null)) == bit.ToInt32(null));
		}
	}
}
