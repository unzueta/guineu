using System;
using Guineu.Expression;

namespace Guineu.Gui
{
	public interface IControl
	{
		void SetVariant(KnownNti nti, Variant value);
		Variant GetVariant(KnownNti nti);

		Variant CallMethod(KnownNti name, ParameterCollection parms);

		event Action<EventData> EventHandler;
	}

	public static class ControlInterfaceExtension
	{
		public static Variant CallMethod(this IControl obj, KnownNti nti)
		{
			return obj.CallMethod(nti, new ParameterCollection());
		}

		public static Boolean RaiseEvent(this IControl obj, Action<EventData> proc, KnownNti name, ParameterCollection parms)
		{
			if (proc == null)
				return true;
			var e = CallEventHandler(proc, name, parms);
			return e.ExecuteNativeBehavior;
		}

		public static Variant CallEvent(this IControl obj, Action<EventData> proc, KnownNti name, ParameterCollection parms)
		{
			if (proc == null)
				return new Variant(true);
			var e = CallEventHandler(proc, name, parms);
			return e.ReturnValue;
		}
		public static Variant CallEvent(this IControl obj, Action<EventData> proc, KnownNti name)
		{
			return CallEvent(obj, proc, name, null);
		}
		public static Boolean HandleEvent(this IControl obj, Action<EventData> proc, KnownNti name, ParameterCollection parms)
		{
			if (proc == null)
				return true;
			var e = CallEventHandler(proc, name, parms);
			return e.ExecuteNativeBehavior;
		}

		static EventData CallEventHandler(Action<EventData> proc, KnownNti name, ParameterCollection parms)
		{
			var e = new EventData(name, parms ?? new ParameterCollection());
			proc(e);
			return e;
		}
	}

	public class EventData
	{
		public readonly ParameterCollection Parameters;
		public Variant ReturnValue { get; set; }
		public readonly KnownNti Event;
		public Boolean ExecuteNativeBehavior { get; set; }

		public EventData(KnownNti name, ParameterCollection parms)
		{
			Parameters = parms;
			Event = name;
			ExecuteNativeBehavior = true;
			ReturnValue = new Variant(true);
		}
	}
}
