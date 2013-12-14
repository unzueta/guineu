using System;
using Guineu.Expression;
using Guineu.Properties;

namespace Guineu
{
	partial class MessageboxFunction : ExpressionBase
	{
		ExpressionBase Text;
		ExpressionBase Caption;
		ExpressionBase Options;
		
		override internal void Compile(Compiler comp)
		{
			var param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					Text = param[0];
					break;
				case 2:
					Text = param[0];
					Options = param[1];
					break;
				case 3:
					Text = param[0];
					Options = param[1];
					Caption = param[2];
					break;
				case 4:
					Text = param[0];
					Options = param[1];
					Caption = param[2];
					// TODO: implement timeout 
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetInt(context),10);
			return retVal;
		}

		internal override int GetInt(CallingContext context)
		{
			string Message = Text.GetString(context);

			// Options
			Int32 options;
			if (Options == null)
				options = 0;
			else
				options = Options.GetInt(context);

			// Dialog caption
			String caption;
			if (Caption == null)
				caption = Resources.App_Title;
			else
				caption = Caption.GetString(context);

			var result = ShowDefault(Message, options, caption); 
			int retVal = (int)result;
			return retVal;
		}
	}
}