using System;
using System.Collections.Generic;
using System.Globalization;
using Guineu.Expression;

namespace Guineu.Functions
{
	partial class SECONDS : ExpressionBase
	{
		static readonly TickProvider Time;
		static readonly uint StartUpTicks;

		static SECONDS()
		{
			Time = new TickProvider();
			var curTime = DateTime.Now;
			StartUpTicks = Time.Ticks - (uint)curTime.TimeOfDay.TotalMilliseconds;
		}

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal double GetDouble(CallingContext exec)
		{
			var ticksSinceMidnight = Time.Ticks - StartUpTicks;
			return ((double)(ticksSinceMidnight % 86400000)) / 1000;
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			return new Variant(GetDouble(exec), 10, 3);
		}

		internal override string GetString(CallingContext exec)
		{
			return GetDouble(exec).ToString(NumberFormatInfo.CurrentInfo);
		}

	}

}