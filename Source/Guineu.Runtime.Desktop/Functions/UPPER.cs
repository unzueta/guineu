using Guineu.Expression;

namespace Guineu.Functions
{
	class UPPER : FunctionBase
	{
		override internal void Compile(Compiler comp)
		{
			GetParameters(comp, 1);
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (Param[0].CheckString(context, false))
				return new Variant(VariantType.Character, true);

			return new Variant(Param[0].GetString(context).ToUpper(System.Globalization.CultureInfo.InvariantCulture));
		}
	}
}