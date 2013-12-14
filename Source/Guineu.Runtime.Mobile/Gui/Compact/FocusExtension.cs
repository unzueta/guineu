using System;
using System.Windows.Forms;
using Guineu.Expression;

namespace Guineu.Gui.Compact
{
	static class FocusExtension
	{
		public static Boolean ValidEvent(this Control obj, IControl ctrl, Action<EventData> proc, Variant newValue)
		{
			Boolean isValid = ctrl.CallEvent(proc, KnownNti.Valid);
			if (isValid)
				ctrl.RaiseEvent(proc, KnownNti.ControlSource,
																				new ParameterCollection(new[] { newValue })
						);
			// TODO: what about positive return values from valid to move the focus?
			return isValid;
		}

		public static void GotFocusEvent(this Control obj, IControl ctrl, Action<EventData> onEventHandler)
		{
			if (ctrl.CallEvent(onEventHandler, KnownNti.When))
				ctrl.CallEvent(onEventHandler, KnownNti.GotFocus);
			else
				obj.Parent.SelectNextControl(obj, true, true, true, true);
		}

		public static void LostFocusEvent(this Control obj, IControl ctrl, Action<EventData> onEventHandler, Variant newValue)
		{
			if (obj.ValidEvent(ctrl, onEventHandler, newValue))
				ctrl.CallEvent(onEventHandler, KnownNti.LostFocus);
			else
				obj.Focus();
		}
	}
}
