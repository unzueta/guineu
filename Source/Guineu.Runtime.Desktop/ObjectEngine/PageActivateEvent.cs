using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class PageActivateEvent : MethodMember
	{
		public PageActivateEvent(ObjectBase obj)
			: base(obj)
		{
		}

		public void Run()
		{
			GuiActivateHandler(null, null);
		}

		void GuiActivateHandler(object sender, EventArgs e)
		{

			ObjectBase obj = Object.GetCodeBase(KnownNti.Activate);
			if (obj != null)
			{
				MethodImplementation m = obj.GetMethod(KnownNti.Activate);
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
			IGuiPage page = (IGuiPage)ctrl.View;
			page.GuiActivate += new EventHandler(GuiActivateHandler);
		}
	}
}
