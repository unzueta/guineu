using Guineu.Expression;

namespace Guineu.Functions
{
	class USED : FunctionBase
	{
		override internal void Compile(Compiler comp)
		{
			GetParameters(comp, 0, 1);
		}

		override internal Variant GetVariant(CallingContext context)
		{
			return new Variant(context.GetCursor(GetParam(0)) != null);
		}
	}
}