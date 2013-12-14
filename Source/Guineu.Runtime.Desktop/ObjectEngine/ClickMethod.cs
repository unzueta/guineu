using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class ClickMethod : MethodMember
	{
		public ClickMethod(ObjectBase obj) : base(obj) { }

		internal override void ExecuteBefore(CallingContext cc, ParameterCollection param)
		{
			ExecuteMethod(KnownNti.Click, cc, param);
		}

		void ClickHandler(EventData e)
		{
			if (e.Event == KnownNti.Click)
				Execute(e.Parameters);
		}

		public void Bind(IControl ctrl)
		{
			ctrl.EventHandler += ClickHandler;
		}
	}
}
