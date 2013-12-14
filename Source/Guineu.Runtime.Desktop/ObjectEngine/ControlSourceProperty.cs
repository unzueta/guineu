using System;
using Guineu.Commands;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	//TODO: This class should inherit from Member to avoid having to use new
	class ControlSourceProperty : StringProperty
	{
		String controlSource;
		UiControl owner;

		public ControlSourceProperty(String src)
		{
			controlSource = src;
			Nti = KnownNti.ControlSource;
		}

		public void AssignParent(UiControl parent)
		{
			owner = parent;
			owner.View.EventHandler += RefreshValue;
			LoadValue();
		}

		void RefreshValue(EventData e)
		{
			if (e.Event == KnownNti.GotFocus || e.Event == KnownNti.Refresh)
				LoadValue();
		}

		public void LoadValue()
		{
			if (HasControlSource())
			{
				Variant value = GetValueOnDisk();
				String newValue = value;
				newValue = value.Type == VariantType.Character ? newValue.TrimEnd(null) : newValue.Trim();
				owner.View.SetVariant(KnownNti.Value, new Variant(newValue));
			}
		}

		Variant GetValueOnDisk()
		{
			using (var context = new CallingContext(GuineuInstance.Context, owner))
			{
				ExpressionBase ex = GetExpression(context);
				Variant value = ex.GetVariant(context);
				return value;
			}
		}
		private ExpressionBase GetExpression(CallingContext context)
		{
			CodeBlock cb = Tokenizer.Expression(controlSource);
			var comp = new Compiler(context, cb);
			return comp.GetCompiledExpression();
		}

		private Boolean HasControlSource()
		{
			if (controlSource == null)
				return false;
			if (controlSource == String.Empty)
				return false;
			return true;
		}

		public override Variant Get()
		{
			return new Variant(controlSource);
		}

		public override void Set(Variant value)
		{
			controlSource = value;
			LoadValue();
		}

		public void SetValue(Variant value)
		{
			var ctx = GuineuInstance.Context;
			if (HasControlSource())
				using (var context = new CallingContext(ctx, owner))
				{
					ExpressionBase ex = GetExpression(context);
					var var = new VariableAssignment(ex, value);
					var.Do(ctx.CurrentContext);
				}
		}
	}
}
