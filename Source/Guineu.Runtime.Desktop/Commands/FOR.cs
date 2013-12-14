using System;
using Guineu.Expression;

namespace Guineu.Commands
{

	class LineInfoFor : LineInfo
	{
		internal Double EndValue;
		internal Double Step;
	}

	class FOR : ICommand
	{
		ExpressionBase loopVar;
		ExpressionBase startValue;
		ExpressionBase endValue;
		ExpressionBase step;
		int jumpEndOfLoop;
		int lineInfo;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			loopVar = comp.GetCompiledExpression();
			code.Reader.ReadToken(); // skip "="
			startValue = comp.GetCompiledExpression();
			code.Reader.ReadToken(); // skip "TO"
			endValue = comp.GetCompiledExpression();

			if (code.Reader.PeekToken() == Token.STEP)
			{
				code.Reader.ReadToken();
				step = comp.GetCompiledExpression();
			}
			jumpEndOfLoop = code.GetLineAtPosition(comp.ReadInt());

			lineInfo = code.CurrentLine;
			code.ControlFlowStack.Push(lineInfo, FlowControlEntry.Types.For, jumpEndOfLoop);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			var info = context.GetLineInfo(lineInfo) as LineInfoFor;
			Double curLoop;
			MemberList locals = context.Locals;
			string destName = loopVar.GetName(context);
			var dest = (ValueMember)locals.Get(new Nti(destName));

			// First call
			if (info == null)
			{
				info = new LineInfoFor
				           {
				               EndValue = endValue.GetDouble(context)
				           };
			    if (step == null)
				{
					info.Step = 1;
				}
				else
				{
					info.Step = step.GetDouble(context);
				}
				context.SetLineInfo(lineInfo, info);
				if (dest == null)
				{
					dest = new ValueMember();
					context.Locals.Add( new Nti(destName), dest);
					//context.Locals.Add(DestName, dest, var.Index);
				}
				dest.Set(startValue.GetVariant(context));
				curLoop = dest.Get();
			}

			// Further calls. We need to increment the loop counter
			else
			{
				curLoop = dest.Get() + info.Step;
			}

			// After incrementing the value, we test if the value is still within the range.
			if (info.Step >= 0 && curLoop <= info.EndValue)
				dest.Set(new Variant(curLoop, 10, 2));
			else if (info.Step < 0 && curLoop >= info.EndValue)
				dest.Set(new Variant(curLoop, 10, 2));
			else
			{
				context.SetLineInfo(lineInfo, null);
				nextLine = jumpEndOfLoop;
			}
		}

	}

}
