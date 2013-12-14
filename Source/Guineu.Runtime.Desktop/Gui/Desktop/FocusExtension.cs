using System;
using System.Windows.Forms;
using Guineu.Expression;

namespace Guineu.Gui.Desktop
{
    static class FocusExtension
    {
        public static void WhenEvent(this Control obj, IControl ctrl, Action<EventData> proc)
        {
            if (!ctrl.CallEvent(proc, KnownNti.When))
                obj.Parent.SelectNextControl(obj, true, true, true, true);
            // TODO: Prevent eternal loop
        }

        public static Boolean ValidEvent(this Control obj, IControl ctrl, Action<EventData> proc, Variant newValue)
        {
            Boolean isValid = ctrl.CallEvent(proc, KnownNti.Valid);
            if (isValid)
                ctrl.RaiseEvent(proc, KnownNti.ControlSource,
                                                new ParameterCollection(new[] { newValue })
                    );
            return isValid;
        }

        public static Boolean ValidEvent(this Control obj, IControl ctrl, Action<EventData> onEventHandler)
        {
            return ctrl.CallEvent(onEventHandler, KnownNti.Valid);
        }
    }
}
