using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	partial class ListInteractiveChangeMethod : MethodMember
	{
		public ListInteractiveChangeMethod(ObjectBase obj)
			: base(obj)
		{
		}

		public void Bind()
		{
			UiControl ctrl = (UiControl)Object;
			IGuiList lst = ctrl.View as IGuiList;
			lst.GuiListInteractiveChange += new EventHandler(lst_SelectedIndexChanged);
		}

		void lst_SelectedIndexChanged(object sender, EventArgs e)
		{
			// On mobile devices the listbox does not trigger the Click event.
			// TODO: Remove method and handle in Compact controls
			CallClickMethod();

			// TODO: Handle distinction between interactive and programmatic changes
			MethodImplementation m = Object.ClassObject.GetMethod(KnownNti.InteractiveChange);
			if (m != null)
			{
				if (m.Code != null)
				{
					using (CallingContext ctx = new CallingContext(GuineuInstance.Context, Object))
					{
						ParameterCollection parms = new ParameterCollection();
						ctx.Context.ExecuteInNewContext(m.Code, parms, Object);
					}
				}
			}
		}
	}
}
