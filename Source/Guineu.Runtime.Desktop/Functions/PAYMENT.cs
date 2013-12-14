using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class PAYMENT : ExpressionBase
	{
		ExpressionBase payment;
		ExpressionBase interestRate;
		ExpressionBase totalPayments;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
				case 1:
				case 2:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 3:
					payment=param[0];
					interestRate=param[1];
					totalPayments=param[2];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetDouble(context), 20, 10);
			return retVal;
		}

		internal override double GetDouble(CallingContext context)
		{
			double nPayments = payment.GetDouble(context);
			double nInterestRate = interestRate.GetDouble(context);
			double nTotalPayments = totalPayments.GetDouble(context);
			
			if (nInterestRate == 0.00)
				return 0.00;

			double nInterValue = nPayments * nInterestRate * nTotalPayments;
			return Math.Round(nInterValue, 2);
		}
	}

}
