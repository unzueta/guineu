using System;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu.Commands
{
	abstract class ScopeClause
	{
		static public Boolean Follows(Token nextToken)
		{
			switch (nextToken)
			{
				case Token.NEXT:
				case Token.ALL:
					case Token.REST:
				case Token.RECORD:
					return true;
			}
			return false;
		}

		public static ScopeClause Create(Token nextToken, CodeBlock code)
		{
			switch (nextToken)
			{
				case Token.NEXT:
					return new ScopeClauseNext(code);
				case Token.ALL:
					return new ScopeClauseAll();
				case Token.REST:
					return new ScopeClauseRest();
					case Token.RECORD:
					return new ScopeClauseRecord(code);
				default:
					throw new ErrorException(ErrorCodes.UnrecognizedKeyword);
			}
		}

		public abstract Scope GetScope(CallingContext ctx);
	}

	
	class ScopeClauseNext : ScopeClause
	{
		readonly ExpressionBase Count;

		public ScopeClauseNext(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			Count = comp.GetCompiledExpression();
		}

		public override Scope GetScope(CallingContext ctx)
		{
			return new ScopeNext(Count.GetInt(ctx));
		}
	}

	class ScopeClauseAll : ScopeClause
	{
		public override Scope GetScope(CallingContext ctx)
		{
			return new ScopeAll();
		}
	}

	class ScopeClauseRest : ScopeClause
	{
		public override Scope GetScope(CallingContext ctx)
		{
			return new ScopeRest();
		}
	}

	class ScopeClauseRecord : ScopeClause
	{
		readonly ExpressionBase Record;

		public ScopeClauseRecord(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			Record = comp.GetCompiledExpression();
		}

		public override Scope GetScope(CallingContext ctx)
		{
			return new ScopeRecord(Record.GetInt(ctx));
		}
	}

}
