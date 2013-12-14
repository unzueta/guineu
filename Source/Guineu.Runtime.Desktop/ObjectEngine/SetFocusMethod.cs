using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class SetFocusMethod : MethodMember
	{
		public SetFocusMethod(ObjectBase obj)
			: base(obj)
		{
		}

		internal override Variant ExecuteNative(CallingContext cc, ParameterCollection param)
		{
			switch (param.Count)
			{
				case 0:
					var ctrl = (UiControl)Object;
			        ctrl.View.CallMethod(KnownNti.SetFocus);
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			return new Variant(true);
		}

	}
}
