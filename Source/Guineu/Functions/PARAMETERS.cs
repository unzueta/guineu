using System;
using Guineu.Expression;

namespace Guineu.Functions
{
	class PARAMETERS : ExpressionBase
	{
		override internal void Compile(Compiler comp) {}
		
		override internal Variant GetVariant(CallingContext context)
		{
			Int32 cnt = context.Stack.Parameters.Count;
			return new Variant(cnt, 3);
		}
	}
}