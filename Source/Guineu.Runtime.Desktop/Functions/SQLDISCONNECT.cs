using System;
using System.Collections.Generic;
using System.Text;

using Guineu.Expression;
using Guineu.Data;

namespace Guineu
{
	class SQLDISCONNECT : ExpressionBase
	{
		ExpressionBase _Handle;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					_Handle = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant retVal = new Variant(GetInt(context), 10);
			return retVal;
		}

		internal override int GetInt(CallingContext context)
		{
			Int32 handle = _Handle.GetInt(context);
			if (handle == 0)
			{
				CloseAllConnections();
			}
			else
			{
				CloseConnection(handle);
			}
			return 0;
		}

		private static void CloseAllConnections()
		{
			for (Int32 handle = 1; handle < GuineuInstance.Connections.Length; handle++)
			{
				if (GuineuInstance.Connections.IsValid(handle))
				{
					ISptEngine engine = GuineuInstance.Connections[handle].Engine;
					engine.Disconnect(handle);
					GuineuInstance.Connections[handle] = null;
				}
			}
		}

		private static void CloseConnection(Int32 handle)
		{
			ISptEngine engine = GuineuInstance.Connections[handle].Engine;
			engine.Disconnect(handle);
			GuineuInstance.Connections[handle] = null;
		}
	}
}