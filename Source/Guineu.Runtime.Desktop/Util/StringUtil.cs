using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu.Util
{
	static class StringUtil
	{
		static public String Upper(String value)
		{
			return value.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}
