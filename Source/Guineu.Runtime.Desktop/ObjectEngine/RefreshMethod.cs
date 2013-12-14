using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	class RefreshMethod : MethodMember
	{
		public RefreshMethod(ObjectBase obj)
			: base(obj)
		{
		}

		internal override Variant ExecuteNative(CallingContext cc, ParameterCollection param)
		{
			switch (param.Count)
			{
				case 0:
					var ctrl = (UiControl)Object;
					ctrl.Refresh();
					//foreach (ObjectBase obj in this.Object.Controls)
					//  obj.CallMethod(cc.Context, "REFRESH", param);
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			return new Variant(true);
		}

		internal override void ExecuteBefore(CallingContext cc, ParameterCollection param)
		{
			ExecuteMethod(KnownNti.Refresh, cc, param);
		}
	}
}
