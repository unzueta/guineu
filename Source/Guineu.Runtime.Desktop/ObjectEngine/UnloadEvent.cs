using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class UnloadEvent : MethodMember
	{
		public UnloadEvent(ObjectBase obj)
			: base(obj)
		{
		}

		public void Run()
		{
			GuiUnloadEventHandler(null, null);
		}

		void GuiUnloadEventHandler(object sender, EventArgs e)
		{

			ObjectBase obj = Object.GetCodeBase(KnownNti.Unload);
			if (obj != null)
			{
				MethodImplementation m = obj.GetMethod(KnownNti.Unload);
				using (CallingContext ctx = new CallingContext(GuineuInstance.Context, Object))
				{
					ParameterCollection parms = new ParameterCollection();
					ctx.Context.ExecuteInNewContext(m.Code, parms, Object);
				}
			}
		}

		public void Bind()
		{
			UiControl ctrl = (UiControl)Object;
			((IGuiForm)ctrl.View).GuiUnload += new EventHandler(GuiUnloadEventHandler);
		}
	}
}
