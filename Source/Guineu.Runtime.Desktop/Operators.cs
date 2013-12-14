using System;
using Guineu.Expression;

namespace Guineu
{
	class OperatorAdd : ExpressionBase
	{
		ExpressionBase leftOperand;
		ExpressionBase rightOperand;

		override internal void Compile(Compiler comp)
		{
			rightOperand = comp.Stack.Pop();
			leftOperand = comp.Stack.Pop();
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant retVal = leftOperand.GetVariant(context) + rightOperand.GetVariant(context);
			return retVal;
		}

		public override string ToString()
		{
			return leftOperand + "+" + rightOperand;
		}
	}

	class OperatorMinus : ExpressionBase
	{
		ExpressionBase leftOperator;
		ExpressionBase rightOperator;

		override internal void Compile(Compiler comp)
		{
			rightOperator = comp.Stack.Pop();
			leftOperator = comp.Stack.Pop();
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant retVal = leftOperator.GetVariant(context) - rightOperator.GetVariant(context);
			return retVal;
		}

		public override string ToString()
		{
			return leftOperator + "-" + rightOperator;
		}
	}


	class OperatorPower : ExpressionBase
	{
		ExpressionBase leftOperand;
		ExpressionBase rightOperand;

		override internal void Compile(Compiler comp)
		{
			rightOperand = comp.Stack.Pop();
			leftOperand = comp.Stack.Pop();
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant retVal = leftOperand.GetVariant(context) ^ rightOperand.GetVariant(context);
			return retVal;
		}

		public override string ToString()
		{
			return leftOperand + "^" + rightOperand;
		}
	}

	class OperatorDivision : ExpressionBase
	{
		ExpressionBase leftOperand;
		ExpressionBase rightOperand;

		override internal void Compile(Compiler comp)
		{
			rightOperand = comp.Stack.Pop();
			leftOperand = comp.Stack.Pop();
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant retVal = leftOperand.GetVariant(context) / rightOperand.GetVariant(context);
			return retVal;
		}

		public override string ToString()
		{
			return leftOperand + "/" + rightOperand;
		}
	}

	class OperatorMultiply : ExpressionBase
	{
		ExpressionBase leftOperand;
		ExpressionBase rightOperand;

		override internal void Compile(Compiler comp)
		{
			rightOperand = comp.Stack.Pop();
			leftOperand = comp.Stack.Pop();
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant retVal = leftOperand.GetVariant(context) * rightOperand.GetVariant(context);
			return retVal;
		}

		public override string ToString()
		{
			return leftOperand + "*" + rightOperand;
		}
	}

	class OperatorGreaterThan : ExpressionBase
	{
		ExpressionBase value1;
		ExpressionBase value2;

		override internal void Compile(Compiler comp)
		{
			value2 = comp.Stack.Pop();
			value1 = comp.Stack.Pop();
		}

		internal override bool GetBool(CallingContext context)
		{
			bool retVal;
			if (value2.FixedInt || value1.FixedInt)
			{
				retVal = (value1.GetInt(context) > value2.GetInt(context));
			}
			else
			{
				retVal = value1.GetVariant(context).IsGreaterThan(value2.GetVariant(context));
			}
			return retVal;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetBool(context));
			return retVal;
		}
	
		public override string ToString()
		{
			return value1 + ">" + value2;
		}
	}

	class OperatorGreaterOrEqualThan : ExpressionBase
	{
		ExpressionBase value1;
		ExpressionBase value2;

		override internal void Compile(Compiler comp)
		{
			value2 = comp.Stack.Pop();
			value1 = comp.Stack.Pop();
		}

		internal override bool GetBool(CallingContext context)
		{
			bool retVal;
			if (value2.FixedInt || value1.FixedInt)
			{
				retVal = (value1.GetInt(context) >= value2.GetInt(context));
			}
			else
			{
				retVal = value1.GetVariant(context).IsGreaterOrEqualThan(value2.GetVariant(context));
			}
			return retVal;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetBool(context));
			return retVal;
		}

		public override string ToString()
		{
			return value1 + ">=" + value2;
		}
	}

	class OperatorLessThan : ExpressionBase
	{
		ExpressionBase value1;
		ExpressionBase value2;

		override internal void Compile(Compiler comp)
		{
			value2 = comp.Stack.Pop();
			value1 = comp.Stack.Pop();
		}

		internal override bool GetBool(CallingContext context)
		{
			bool retVal;
			if (value2.FixedInt || value1.FixedInt)
			{
				retVal = (value1.GetInt(context) < value2.GetInt(context));
			}
			else
			{
				retVal = value1.GetVariant(context).IsLessThan(value2.GetVariant(context));
			}
			return retVal;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetBool(context));
			return retVal;
		}

		public override string ToString()
		{
			return value1 + "<" + value2;
		}
	}

	class OperatorLessOrEqualThan : ExpressionBase
	{
		ExpressionBase value1;
		ExpressionBase value2;

		override internal void Compile(Compiler comp)
		{
			value2 = comp.Stack.Pop();
			value1 = comp.Stack.Pop();
		}

		internal override bool GetBool(CallingContext context)
		{
			bool retVal;
			if (value2.FixedInt || value1.FixedInt)
			{
				retVal = (value1.GetInt(context) <= value2.GetInt(context));
			}
			else
			{
				retVal = value1.GetVariant(context).IsLessOrEqualThan(value2.GetVariant(context));
			}
			return retVal;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetBool(context));
			return retVal;
		}

		public override string ToString()
		{
			return value1 + "<=" + value2;
		}
	}

	class OperatorModulo : ExpressionBase
	{
		ExpressionBase value1;
		ExpressionBase value2;

		override internal void Compile(Compiler comp)
		{
			value2 = comp.Stack.Pop();
			value1 = comp.Stack.Pop();

			FixedInt = true;
		}

		internal override int GetInt(CallingContext context)
		{
			int retVal;
			if (value2.FixedInt || value1.FixedInt)
			{
				retVal = (value1.GetInt(context) % value2.GetInt(context));
			}
			else
			{
				retVal = GetVariant(context);
			}
			return retVal;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant retVal = value1.GetVariant(context) % value2.GetVariant(context);
			return retVal;
		}

		public override string ToString()
		{
			return value1 + "%" + value2;
		}
	}

	class OperatorEquals : ExpressionBase
	{
		ExpressionBase value1;
		ExpressionBase value2;

		override internal void Compile(Compiler comp)
		{
			value2 = comp.Stack.Pop();
			value1 = comp.Stack.Pop();
		}

		internal override bool GetBool(CallingContext context)
		{
			bool retVal;
			if (value2.FixedInt || value1.FixedInt)
			{
				retVal = (value1.GetInt(context) == value2.GetInt(context));
			}
			else
			{
				retVal = value1.GetVariant(context).IsEqual(value2.GetVariant(context));
			}
			return retVal;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetBool(context));
			return retVal;
		}
	
		public override string ToString()
		{
			return value1 + "=" + value2;
		}
	}

	class OperatorNotEquals : ExpressionBase
	{
		ExpressionBase value1;
		ExpressionBase value2;

		override internal void Compile(Compiler comp)
		{
			value2 = comp.Stack.Pop();
			value1 = comp.Stack.Pop();
		}

		internal override bool GetBool(CallingContext context)
		{
			bool retVal;
			if (value2.FixedInt || value1.FixedInt)
			{
				retVal = (value1.GetInt(context) != value2.GetInt(context));
			}
			else
			{
				retVal = value1.GetVariant(context).IsNotEqual(value2.GetVariant(context));
			}
			return retVal;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetBool(context));
			return retVal;
		}
	
		public override string ToString()
		{
			// There are three operators (<>, != and #). VFP uses #.
			return value1 + "#" + value2;
		}
	}

	class OperatorEqualsBinary : ExpressionBase
	{
		ExpressionBase value1;
		ExpressionBase value2;

		override internal void Compile(Compiler comp)
		{
			value2 = comp.Stack.Pop();
			value1 = comp.Stack.Pop();
		}

		internal override bool GetBool(CallingContext context)
		{
			bool retVal;
			if (value2.FixedInt || value1.FixedInt)
			{
				retVal = (value1.GetInt(context) == value2.GetInt(context));
			}
			else
			{
				retVal = value1.GetVariant(context).IsEqualBinary(value2.GetVariant(context));
			}
			return retVal;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetBool(context));
			return retVal;
		}

		public override string ToString()
		{
			return value1 + "==" + value2;
		}
	}

	class OperatorNot : ExpressionBase
	{
		ExpressionBase operand;

		override internal void Compile(Compiler comp)
		{
			operand = comp.Stack.Pop();
		}

		override internal Variant GetVariant(CallingContext context)
		{
			bool result = GetBool(context);
			var retVal = new Variant(result);
			return retVal;
		}

		internal override bool GetBool(CallingContext context)
		{
			bool val = operand.GetBool(context);
			return !val;
		}

		public override string ToString()
		{
			return ".NOT." + operand;
		}
	}

	class OperatorAnd : ExpressionBase
	{
		ExpressionBase leftOperand;
		ExpressionBase rightOperand;

		override internal void Compile(Compiler comp)
		{
			rightOperand = comp.Stack.Pop();
			leftOperand = comp.Stack.Pop();
		}

		override internal Variant GetVariant(CallingContext context)
		{
			bool result = GetBool(context);
			var retVal = new Variant(result);
			return retVal;
		}

		internal override bool GetBool(CallingContext context)
		{
			bool val = leftOperand.GetBool(context);
			if (val)
			{
				bool val2 = rightOperand.GetBool(context);
				return (val2);
			}
			return false;
		}

		public override string ToString()
		{
			return leftOperand+ ".AND." + rightOperand;
		}
	}

	class OperatorOr : ExpressionBase
	{
		ExpressionBase leftOperand;
		ExpressionBase rightOperand;

		override internal void Compile(Compiler comp)
		{
			rightOperand = comp.Stack.Pop();
			leftOperand = comp.Stack.Pop();
		}

		override internal Variant GetVariant(CallingContext context)
		{
			bool result = GetBool(context);
			var retVal = new Variant(result);
			return retVal;
		}

		internal override bool GetBool(CallingContext context)
		{
			bool val = leftOperand.GetBool(context);
			if (val)
				return true;
			bool val2 = rightOperand.GetBool(context);
			return (val2);
		}
	
		public override string ToString()
		{
			return leftOperand + ".OR." + rightOperand;
		}
	}


	class OperatorContains : ExpressionBase
	{
		ExpressionBase value1;
		ExpressionBase value2;

		override internal void Compile(Compiler comp)
		{
			value2 = comp.Stack.Pop();
			value1 = comp.Stack.Pop();
		}

		internal override bool GetBool(CallingContext context)
		{
			String value = value2.GetString(context);
			String search = value1.GetString(context);

			return value.IndexOf(search) >= 0;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (value1.CheckString(context, false))
				return new Variant(VariantType.Logical, true);

			if (value2.CheckString(context, false))
				return new Variant(VariantType.Logical, true);

			var retVal = new Variant(GetBool(context));
			return retVal;
		}
	
		public override string ToString()
		{
			return value1+ "$" + value2;
		}
	}
}

