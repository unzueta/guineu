using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class LostFocusEvent : MethodMember
	{
		public LostFocusEvent(ObjectBase obj)
			: base(obj)
		{
		}

		public void Run()
		{
			ObjectBase obj = Object.GetCodeBase(KnownNti.LostFocus);
			if (obj != null)
			{
				MethodImplementation m = obj.GetMethod(KnownNti.LostFocus);
				using (var ctx = new CallingContext(GuineuInstance.Context, Object))
				{
					var parms = new ParameterCollection();
					ctx.Context.ExecuteInNewContext(m.Code, parms, Object);
				}
			}
		}

		void LostFocusHandler(EventData e)
		{
			if (e.Event == KnownNti.LostFocus)
				Run();
		}

		public void Bind(IControl ctrl)
		{
			ctrl.EventHandler += LostFocusHandler;
		}
	}
}
