using System;
using System.Runtime.InteropServices;

namespace Guineu.Functions
{
	partial class SYS8013
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern int SetLocalTime(ref SystemTime lpSystemTime);

		struct SystemTime
		{
			// ReSharper disable UnaccessedField.Local
#pragma warning disable 169
			public short Year;
			public short Month;
			public short DayOfWeek;
			public short Day;
			public short Hour;
			public short Minute;
			public short Second;
			public short Milliseconds;
			// ReSharper restore UnaccessedField.Local
#pragma warning restore 169
		}

		static partial void SetTime(DateTime time)
		{
			var systNew = new SystemTime
											{
												Day = (short)time.Day,
												Month = (short)time.Month,
												Year = (short)time.Year,
												Hour = (short)time.Hour,
												Minute = (short)time.Minute,
												Second = (short)time.Second
											};
			SetLocalTime(ref systNew);
		}
	}
}
