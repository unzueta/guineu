using System;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu.Commands
{
	class InClause
	{
		ExpressionBase alias;

		public static Boolean Follows(Token nextToken)
		{
			return nextToken == Token.IN;
		}

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			alias = comp.GetCompiledExpression();
		}

		public OperateInWorkArea Get(CallingContext ctx)
		{
			return new OperateInWorkArea(ctx,alias);
		}
	}

	internal class OperateInWorkArea : IDisposable
	{
		readonly DataSession session;
		readonly Int32 workArea;
		readonly Int32 previousWorkArea;

		public ICursor Cursor
		{
			get { return session[workArea]; }
		}

		public OperateInWorkArea(CallingContext ctx, ExpressionBase area)
		{
			session = ctx.DataSession;
			previousWorkArea = session.Select();
			workArea = session.GetWorkArea(area, ctx);
			session.Select(workArea);
		}

		public void Dispose()
		{
			session.Select(previousWorkArea);
		}
	}
}