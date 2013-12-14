using System;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class ValidEvent : MethodMember
	{
		public ValidEvent(ObjectBase obj)
			: base(obj)
		{
		}

		void ValidHandler(EventData e)
		{
			if (e.Event == KnownNti.Valid)
				e.ReturnValue = new Variant(Validate() != 0);
		}

		public Int32 Validate()
		{
			ObjectBase obj = Object.GetCodeBase(KnownNti.Valid);
			if (obj != null)
			{
				MethodImplementation m = obj.GetMethod(KnownNti.Valid);
				using (var ctx = new CallingContext(GuineuInstance.Context, Object))
				{
					var parms = new ParameterCollection();
					Variant result = ctx.Context.ExecuteInNewContext(m.Code, parms, Object);
					if (result.Type == VariantType.Logical)
						if (!result)
						{
							// TODO: Show validation error message
							return 0;
						}
						else if (result.Type == VariantType.Number)
							if (result == 0)
								return 0;
							else if (result.Type == VariantType.Integer)
								if (result == 0)
									return 0;
								else
									return result;
				}
			}
			return 1;
		}

		public void Bind(IControl ctrl)
		{
			ctrl.EventHandler += ValidHandler;
		}
	}
}
