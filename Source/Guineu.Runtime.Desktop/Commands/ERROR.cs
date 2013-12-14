using System;
using Guineu.Expression;

namespace Guineu
{

	class ERROR : ICommand
	{
		ExpressionBase[] ex;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);

			// TODO : Explain to Fred how params of command are working as this
			// Expressions is always 0 ...
			// i tried to follow the PRINT command...
			// Determine the number of expressions
			int expressions = comp.ReadInt();
			ex = new ExpressionBase[expressions];

			// Print all expressions in a new line
			for (int i = 0; i < expressions; i++)
			{
				ex[i] = comp.GetCompiledExpression();
				code.Reader.ReadByte();
			}
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			try
			{
				ErrorException er;
				if (ex.Length == 0)
				{
					// Command is missing required clause.
					throw new ErrorException(ErrorCodes.CommandIsMissingRequiredClause);
				}
				if (ex.Length == 1)
				{
				    Variant exValue = ex[0].GetVariant(exec);
				    if (exValue.Type == VariantType.Character)
					{
						er = new ErrorException(ErrorCodes.UserDefinedError, exValue);
						throw er;
					}
				    if (exValue.Type == VariantType.Integer)
				    {
				        er = new ErrorException((ErrorCodes)(Int32)exValue);
				        throw er;
				    }
				    throw new ErrorException(ErrorCodes.DataTypeMismatch);
				}
			    if (ex.Length == 2)
			    {
			        Variant ex1 = ex[0].GetVariant(exec);
			        Variant ex2 = ex[1].GetVariant(exec);

			        if (ex1.Type == VariantType.Integer && ex2.Type == VariantType.Character)
			        {
			            er = new ErrorException((ErrorCodes)(Int32)ex1, ex2);
			            throw er;
			        }
			        throw new ErrorException(ErrorCodes.DataTypeMismatch);
			    }
			    // Syntax Error
			    throw new ErrorException(ErrorCodes.Syntax);
			}
			catch (Exception)
			{
				throw new ErrorException(ErrorCodes.ErrorCodeInvalid);
			}
		}
	}

}