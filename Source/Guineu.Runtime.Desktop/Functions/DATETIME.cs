using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class DATETIME : ExpressionBase
	{
		ExpressionBase day;
		ExpressionBase month;
		ExpressionBase year;

		ExpressionBase hour;
		ExpressionBase minute;
		ExpressionBase second;

		Boolean current;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					current = true;
					break;
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 3:
					year = param[0];
					month = param[1];
					day = param[2];
					break;
				case 4:
					year = param[0];
					month = param[1];
					day = param[2];
					hour = param[3];
					break;
				case 5:
					year = param[0];
					month = param[1];
					day = param[2];
					hour = param[3];
					minute = param[4];
					break;
				case 6:
					year = param[0];
					month = param[1];
					day = param[2];
					hour = param[3];
					minute = param[4];
					second = param[5];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (current)
				return new Variant(DateTime.Now);
			
			return new Variant(GetDateTime(context));
		}

		internal DateTime GetDateTime(CallingContext context)
		{
			int d, m, y;
			int h, min, s;

			if (year == null || year.GetVariant(context).IsNull)
				y = DateTime.Now.Year;
			else
				y = year.GetInt(context);

			if (month == null || month.GetVariant(context).IsNull)
				m = DateTime.Now.Month;
			else
				m = month.GetInt(context);

			if (day == null || day.GetVariant(context).IsNull)
				d = DateTime.Now.Day;
			else
				d = day.GetInt(context);

			if (hour == null || hour.GetVariant(context).IsNull)
				h = 0;
			else
				h = hour.GetInt(context);

			if (minute == null || minute.GetVariant(context).IsNull)
				min = 0;
			else
				min = minute.GetInt(context);

			if (second == null || second.GetVariant(context).IsNull)
				s = 0;
			else
				s = second.GetInt(context);

			return new DateTime(y, m, d, h, min, s);
		}
	}
}
