using System.Globalization;
using Guineu.Commands;

namespace Guineu
{
	public class Settings
	{
		public Settings()
		{
			Bell = true;
			Notify = new SetNotifyValue();
			Decimals = new SetDecimalsValue();
			Exclusive = new SetExclusiveValue();
			Path = new SetPathValue();
			Sysformats = new SetSysformatsValue();
			Deleted = new SetDeletedValue();
			Notify = new SetNotifyValue();
			Exact = new SetExactValue();
			NullDisplay = new SetNullDisplayValue();
			Safety = new SetSafetyValue();
			MemoWidth = new SetMemoWidthValue();
			Status = new SetStatusValue();
			StatusBar = new SetStatusBarValue();
			Talk = new SetTalkValue();
			Procedure = new SetProcedureValue();
			ClassLib = new SetClassLibValue();
			Point = new SetPointValue();
		}

		public SetDecimalsValue Decimals { get; private set; }
		public SetPathValue Path { get; private set; }
		public SetExclusiveValue Exclusive { get; private set; }
		public SetSysformatsValue Sysformats { get; private set; }
		public SetDeletedValue Deleted { get; private set; }
		public SetNotifyValue Notify { get; set; }
		public SetExactValue Exact { get; private set; }
		public SetNullDisplayValue NullDisplay { get; private set; }
		public SetSafetyValue Safety { get; private set; }
		public SetMemoWidthValue MemoWidth { get; private set; }
		public SetStatusValue Status { get; private set; }
		public SetStatusBarValue StatusBar { get; private set; }
		public SetTalkValue Talk { get; private set; }
		public SetProcedureValue Procedure { get; private set; }
		public SetClassLibValue ClassLib { get; private set; }
		public SetPointValue Point { get; private set; }

		public string BellFile { get; set; }
		public bool Bell { get; set; }

		CultureInfo culture;
		public CultureInfo CurrentCulture
		{
			get
			{
				if (culture == null)
					culture = new CultureInfo(CultureInfo.CurrentUICulture.Name);
				culture.NumberFormat.NumberDecimalSeparator = Point.Value;
				return culture;
			}
		}
	}
}
