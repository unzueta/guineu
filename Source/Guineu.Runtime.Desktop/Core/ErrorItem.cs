using System;
using Guineu.Properties;

namespace Guineu.Core
{
	public abstract class ErrorItemBase
	{
		public ErrorCodes Code { get; private set; }
		public string Text { get; private set; }
		public string Parameter { get; private set; }

		protected ErrorItemBase(ErrorCodes newCode) : this(newCode, null) { }
       
		protected ErrorItemBase(ErrorCodes newCode, String newParameter)
		{
			Parameter = newParameter;
			Code = newCode;
			if (String.IsNullOrEmpty(newParameter))
				Text = GetErrorText("");
			else
				Text = GetErrorText("'" + newParameter + "'");
		}
		
		String GetErrorText(string newParameter)
		{
			String key = "Error_" + Code;
			String text = Resources.ResourceManager.GetString(key);
			if (String.IsNullOrEmpty(text))
				text = Code.ToString();
			else
				text = String.Format(text, newParameter);
			return text;
		}

		internal virtual void FillErrorArray(ArrayMember arr, Int32 row)
		{
			arr.Locate(row, 1).Set(new Variant((Int32)Code, 10));
			arr.Locate(row, 2).Set(new Variant(Text));
			arr.Locate(row, 3).SetString(new Variant(Parameter));
		}
	}

	//class ErrorItem : ErrorItemBase
	//{
	//  public int Workarea { get; private set; }
	//  public int Trigger { get; private set; }

	//  public ErrorItem(ErrorCodes newCode, String newParameter)
	//    : base(newCode, newParameter)
	//  {
	//  }
	//  internal override void FillErrorArray(ArrayMember arr, Int32 row)
	//  {
	//    base.FillErrorArray(arr, row);
	//    arr.Locate(row, 4).Set(new Variant(Workarea,10));
	//    arr.Locate(row, 5).Set(new Variant(Trigger,10));
	//  }
	//}

	class OdbcErrorItem : ErrorItemBase
	{
		public byte OdbcState { get; private set; }
		public int OdbcError { get; private set; }
		public int Handle { get; private set; }

		public OdbcErrorItem(String odbcMessage, Byte state, Int32 error, Int32 sptHandle)
			: base(ErrorCodes.ConnectivityError, odbcMessage)
		{
			OdbcState = state;
			OdbcError = error;
			Handle = sptHandle;
		}

		internal override void FillErrorArray(ArrayMember arr, Int32 row)
		{
			base.FillErrorArray(arr, row);
			arr.Locate(row, 4).Set(new Variant(OdbcState));
			arr.Locate(row, 5).Set(new Variant(OdbcError, 10));
			arr.Locate(row, 6).Set(new Variant(Handle, 10));
			arr.Locate(row, 7).Set(new Variant(VariantType.Character, true));
		}
	}

	//class OleErrorItem : ErrorItemBase
	//{
	//  public string OleMessage { get; private set; }
	//  public string Application { get; private set; }
	//  public string HelpFile { get; private set; }
	//  public string HelpContextId { get; private set; }

	//  public int OleError { get; private set; }

	//  internal override void FillErrorArray(ArrayMember arr, Int32 row)
	//  {
	//    base.FillErrorArray(arr, row);
	//    arr.Locate(row, 4).Set(new Variant(Application));
	//    arr.Locate(row, 5).Set(new Variant(HelpFile));
	//    arr.Locate(row, 6).Set(new Variant(HelpContextId));
	//    arr.Locate(row, 7).Set(new Variant(OleError, 10));
	//  }
	//  public OleErrorItem(ErrorCodes newCode, String newParameter)
	//    : base(newCode, newParameter)
	//  {
	//  }
	//}
}
