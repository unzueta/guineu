using Guineu.Data;
using Guineu.Expression;

namespace Guineu.Functions
{
	class FILTER : FunctionBase
	{
		override internal void Compile(Compiler comp)
		{
			GetParameters(comp, 0, 1);
		}

		override internal Variant GetVariant(CallingContext context)
		{
			ICursor csr = context.DataSession.GetCursor(GetParam(0));
			ExpressionBase filter = csr.GetFilter();

			return new Variant(filter.ToString());
		}
	}
}