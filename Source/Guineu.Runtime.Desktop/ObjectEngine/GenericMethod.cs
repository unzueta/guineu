using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class GenericMethod : MethodMember
	{
		readonly Nti method;
		readonly IControl control;

		public GenericMethod(ObjectBase obj, KnownNti name)
			: this(obj, name, null)
		{ }

		public GenericMethod(ObjectBase obj, KnownNti name, IControl owner)
			: base(obj)
		{
			method = name;
			control = owner;
		}

		internal override Variant ExecuteNative(CallingContext cc, ParameterCollection param)
		{
			return control.CallMethod(method, param);
		}
	}
}
