using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class ListClearMethod : MethodMember
	{
		public ListClearMethod(ObjectBase obj)
			: base(obj)
		{
		}

		internal override Variant ExecuteNative(CallingContext cc, ParameterCollection param)
		{
			switch (param.Count)
			{
				case 0:
					UiControl ctrl = (UiControl)Object;
					IGuiList lst = ctrl.View as IGuiList;
                    lst.GuiClear();
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			return new Variant(true);
		}

	}
}
