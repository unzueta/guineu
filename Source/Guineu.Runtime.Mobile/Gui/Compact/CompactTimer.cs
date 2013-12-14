using System;
using System.Windows.Forms;
using Guineu.Expression;

namespace Guineu.Gui.Compact
{
	class CompactTimer : Timer, IControl
	{
		public CompactTimer()
		{
			Tick += TimerEvent;
		}

	  private Boolean isEnabled;

		public void SetVariant(KnownNti nti, Variant value)
		{
      lock (this)
      {
        switch (nti)
        {
          case KnownNti.Enabled:
            SetEnabled(value);
            break;
          case KnownNti.Interval:
            SetInterval(value);
            break;
          default:
            throw new ErrorException(ErrorCodes.PropertyIsNotFound);
        }
      }
		}

    /// <summary>
    /// Changes the ínterval of the timer
    /// </summary>
    /// <param name="value"></param>
    /// <remarks>
    /// The .NET Compact Framework does not stop raising timer events when
    /// the interval is 0. Therefore we need to disable the timer in this case
    /// even when the Guineu Enabled property is .T.
    /// </remarks>
	  private void SetInterval(Variant value)
	  {
	    var newInterval = value;
			if (newInterval == 0)
				Enabled = false;
			Interval = newInterval;
				Enabled = isEnabled;
		}

    /// <summary>
    /// Changes the Enabled state of the timer
    /// </summary>
    /// <param name="value"></param>
    /// <remarks>
    /// The .NET Compact Framework does not stop raising timer events when
    /// the interval is 0. Therefore we do not directly map the Guineu 
    /// Enabled property to the timer's Enabled property. Instead we keep
    /// track of the desired state in a separate value and change the control's
    /// property in arcodance with the Interval property
    /// </remarks>
    private void SetEnabled(Variant value)
    {
        isEnabled = value;
        Enabled = Interval != 0 && isEnabled;
    }

	    public Variant GetVariant(KnownNti nti)
		{
			switch (nti)
			{
				case KnownNti.Enabled:
					return new Variant(isEnabled);
				case KnownNti.Interval:
					return new Variant(Interval, 10);
				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public Variant CallMethod(KnownNti name, ParameterCollection parms)
		{
			switch (name)
			{
				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public event Action<EventData> EventHandler;

		#region Handle events for framework control

		void TimerEvent(Object source, EventArgs e)
		{
		    Enabled = false;
		    this.RaiseEvent(EventHandler, KnownNti.Timer, null);
		    Enabled = isEnabled;
		}

		#endregion
	}
}
