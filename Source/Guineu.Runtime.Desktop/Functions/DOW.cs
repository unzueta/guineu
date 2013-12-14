using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class DOW : ExpressionBase
	{
		ExpressionBase CurDate;
		ExpressionBase FirstDayOfWeek;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					CurDate = param[0];
					break;
				case 2:
					CurDate = param[0];
					FirstDayOfWeek = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			// TODO: Use parameter fdow
			// TODO: Check length of integer in VFP
			DateTime date = CurDate.GetVariant(context);
			Int32 fdow = FirstDayOfWeek.GetInt(context);
			switch (date.DayOfWeek)
			{
				case DayOfWeek.Sunday:
					return new Variant(7,10);
				case DayOfWeek.Monday:
					return new Variant(1, 10);
				case DayOfWeek.Tuesday:
					return new Variant(2, 10);
				case DayOfWeek.Wednesday:
					return new Variant(3, 10);
				case DayOfWeek.Thursday:
					return new Variant(4, 10);
				case DayOfWeek.Friday:
					return new Variant(5, 10);
				case DayOfWeek.Saturday:
					return new Variant(6, 10);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

	}

}