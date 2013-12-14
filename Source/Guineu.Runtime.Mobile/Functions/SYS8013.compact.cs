using System;
using OpenNETCF.WindowsCE;

namespace Guineu.Functions
{
	/// <summary>
	/// Set time
	/// </summary>
	 partial class SYS8013
	{
		static partial void SetTime(DateTime time)
		{
			DateTimeHelper.LocalTime = time;
		}
	}
}