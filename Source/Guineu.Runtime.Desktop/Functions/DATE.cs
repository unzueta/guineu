using System;
using Guineu.Expression;

namespace Guineu.Functions
{
	class DATE : FunctionBase
	{
		ExpressionBase day;
		ExpressionBase month;
		ExpressionBase year;

		override internal void Compile(Compiler comp)
		{
			GetParameters(comp, 0, 3);
			if (Param.Count == 3)
			{
				year = Param[0];
				month = Param[1];
				day = Param[2];
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (Param.Count == 0)
				return new Variant(DateTime.Now, VariantType.Date);

			return new Variant(GetDateTime(context), VariantType.Date);
		}

		internal DateTime GetDateTime(CallingContext context)
		{
			int d, m, y;
			if (year.GetVariant(context).IsNull)
				y = DateTime.Now.Year;
			else
				y = year.GetInt(context);

			if (month.GetVariant(context).IsNull)
				m = DateTime.Now.Month;
			else
				m = month.GetInt(context);

			if (day.GetVariant(context).IsNull)
				d = DateTime.Now.Day;
			else
				d = day.GetInt(context);

			return new DateTime(y, m, d);
		}

	}
}
