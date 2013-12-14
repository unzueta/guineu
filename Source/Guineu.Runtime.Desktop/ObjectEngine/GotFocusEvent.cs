using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class GotFocusEvent : MethodMember
	{
		public GotFocusEvent(ObjectBase obj)
			: base(obj)
		{
		}

		public void Run()
		{
			ObjectBase obj = Object.GetCodeBase(KnownNti.GotFocus);
			if (obj != null)
			{
				MethodImplementation m = obj.GetMethod(KnownNti.GotFocus);
				using (var ctx = new CallingContext(GuineuInstance.Context, Object))
				{
					var parms = new ParameterCollection();
					ctx.Context.ExecuteInNewContext(m.Code, parms, Object);
				}
			}
		}

		void GotFocusHandler(EventData e)
		{
			if (e.Event == KnownNti.Click)
				Run();
		}
		public void Bind(IControl ctrl)
		{
			ctrl.EventHandler += GotFocusHandler;
		}

	}
}
