using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	public class GenericEvent : MethodMember
	{
		readonly KnownNti eventName;

		public GenericEvent(ObjectBase obj, KnownNti name)
			: base(obj)
		{
			eventName = name;
		}

		internal override Variant ExecuteNative(CallingContext cc, ParameterCollection param)
		{
			return ExecuteMethod(eventName, cc, param);
		}
	}
}
