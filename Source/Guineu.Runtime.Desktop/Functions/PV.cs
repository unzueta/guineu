using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class PV : ExpressionBase
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
					payment = param[0];
					interestRate = param[1];
					totalPayments = param[2];
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
			double nPayment = payment.GetDouble(context);
			double nInterestRate = interestRate.GetDouble(context);
			double nTotalPayments = totalPayments.GetDouble(context);

			double nInterValue = Math.Pow(1 + nInterestRate, nTotalPayments);

			if (nInterestRate == 0.00 || nInterValue==0.00)
				return 0.00;
			
			double pv = (nPayment * (nInterValue - 1) / nInterestRate) / nInterValue;

			return Math.Round(pv, 2);
		}
	}

}