using System;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu.Commands
{
	class IteratorClause
	{
		ExpressionBase ForClause;
		ExpressionBase WhileClause;
		ScopeClause Scope;
		Boolean Optimize = true;
		readonly Boolean AllRecords;

		public IteratorClause(Boolean allRecords)
			: this()
		{
			AllRecords = allRecords;
		}

		public IteratorClause() { }

		static public Boolean Follows(Token nextToken)
		{
			switch (nextToken)
			{
				case Token.FOR:
				case Token.NOOPTIMIZE:
				case Token.WHILE:
					return true;
			}
			if (ScopeClause.Follows(nextToken))
				return true;
			return false;
		}

		public void Compile(Token nextToken, CodeBlock code)
		{
			var comp = new Compiler(null, code);
			switch (nextToken)
			{
				case Token.FOR:
					ForClause = comp.GetCompiledExpression();
					break;
				case Token.NOOPTIMIZE:
					Optimize = false;
					break;
				case Token.WHILE:
					WhileClause = comp.GetCompiledExpression();
					break;
				default:
					Scope = ScopeClause.Create(nextToken, code);
					break;
			}
		}

		public IRecordIterator Get(CallingContext ctx)
		{
			Scope scope;
			if (Scope == null)
				scope = DefaultScope();
			else
			{
				scope = Scope.GetScope(ctx);
			}
			var condition = new EofConditionChecker(
				new ScopeConditionChecker(GetWhile(ctx), ctx, scope), ctx);
			return new RecordIterator(ctx, condition);
		}

		Scope DefaultScope()
		{
			if (WhileClause != null)
				return new ScopeRest();
			if (ForClause != null)
				return new ScopeAll();
			if (AllRecords)
				return new ScopeAll();
			return new ScopeNext(1);
		}

		IIteratorCondtionChecker GetFor(CallingContext ctx)
		{
			if (ForClause == null)
				return null;

			// TODO: Create optimizable FOR
			if (Optimize)
				return new ForConditionChecker(null, ctx, ForClause);

			return new ForConditionChecker(null, ctx, ForClause);
		}

		IIteratorCondtionChecker GetWhile(CallingContext ctx)
		{
			if (WhileClause == null)
				return GetFor(ctx);
			return new WhileConditionChecker(GetFor(ctx), ctx, WhileClause);
		}
	}
}
