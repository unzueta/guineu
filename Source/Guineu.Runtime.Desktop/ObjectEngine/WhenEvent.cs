using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class WhenEvent : MethodMember
	{
		public WhenEvent(ObjectBase obj)
			: base(obj)
		{
		}

		void WhenHandler(EventData e)
		{
			if (e.Event == KnownNti.When)
			{
				ObjectBase obj = Object.GetCodeBase(KnownNti.When);
				if (obj != null)
				{
					MethodImplementation m = obj.GetMethod(KnownNti.When);
					using (var ctx = new CallingContext(GuineuInstance.Context, Object))
					{
						var parms = new ParameterCollection();
						e.ReturnValue = ctx.Context.ExecuteInNewContext(m.Code, parms, Object);
					}
				}
			}
		}

		public void Bind(IControl ctrl)
		{
			ctrl.EventHandler += WhenHandler;
		}
	}
}
