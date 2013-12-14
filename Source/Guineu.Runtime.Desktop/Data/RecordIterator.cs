using System;
using Guineu.Expression;

namespace Guineu.Data
{
	abstract class IteratorCondtionChecker : IIteratorCondtionChecker
	{
		readonly protected IIteratorCondtionChecker NextCondition;
		readonly protected DataSession Session;
		readonly protected Int32 Area;

		protected IteratorCondtionChecker(IIteratorCondtionChecker next, CallingContext ctx)
		{
			NextCondition = next;
			Session = ctx.DataSession;
			Area = Session.Select();
		}

		public abstract void MoveToFirst(CallingContext ctx);
		public abstract Boolean Next(CallingContext ctx);
		public abstract Boolean HasMore(CallingContext ctx);
		public abstract Boolean IsValid(CallingContext ctx);
	}

	class ForConditionChecker : IteratorCondtionChecker
	{
		readonly ExpressionBase condition;

		public ForConditionChecker(IIteratorCondtionChecker next, CallingContext ctx, ExpressionBase cond)
			: base(next, ctx)
		{
			condition = cond;
		}

		public override void MoveToFirst(CallingContext ctx)
		{
			if (NextCondition != null)
				NextCondition.MoveToFirst(ctx);
		}

		public override Boolean Next(CallingContext ctx)
		{
			if (IsValid(ctx))
				if (NextCondition == null)
					return true;
				else
					return NextCondition.Next(ctx);
			return false;
		}

		public override bool IsValid(CallingContext ctx)
		{
			if (Session[Area].Eof())
				return false;
			Session.Select(Area);
			if (condition.GetBool(ctx))
				if (NextCondition == null)
					return true;
				else
					return NextCondition.IsValid(ctx);
			return false;
		}

		public override bool HasMore(CallingContext ctx)
		{
			return !Session[Area].Eof();
		}
	}

	class WhileConditionChecker : IteratorCondtionChecker
	{
		readonly ExpressionBase condition;
		Boolean stillValid;

		public WhileConditionChecker(IIteratorCondtionChecker next, CallingContext ctx, ExpressionBase cond)
			: base(next, ctx)
		{
			condition = cond;
		}

		public override void MoveToFirst(CallingContext ctx)
		{
			stillValid = true;
			if (NextCondition != null)
				NextCondition.MoveToFirst(ctx);
		}

		public override Boolean Next(CallingContext ctx)
		{
			if (IsValid(ctx))
				if (NextCondition == null)
					return true;
				else
					return NextCondition.Next(ctx);
			return false;
		}

		public override bool IsValid(CallingContext ctx)
		{
			Boolean valid;
			if (Session[Area].Eof())
				valid = false;
			else
			{
				Session.Select(Area);
				if (condition.GetBool(ctx))
					if (NextCondition == null)
						valid = true;
					else
						valid = NextCondition.IsValid(ctx);
				else
					valid = false;
			}
			if (!valid)
				stillValid = false;
			return valid;
		}

		public override bool HasMore(CallingContext ctx)
		{
			return stillValid && !Session[Area].Eof();
		}
	}

	class ScopeConditionChecker : IteratorCondtionChecker
	{
		readonly Scope scope;

		public ScopeConditionChecker(IIteratorCondtionChecker next, CallingContext ctx, Scope scope)
			: base(next, ctx)
		{
			this.scope = scope;
		}

		public override void MoveToFirst(CallingContext ctx)
		{
			scope.MoveToFirst(Session[Area]);
			if (NextCondition != null)
				NextCondition.MoveToFirst(ctx);
		}

		public override Boolean Next(CallingContext ctx)
		{
			scope.IncrementUsage();
			if (!scope.StillValid())
				return false;

			Session[Area].Skip(1);

			if (NextCondition == null)
				return true;
			return NextCondition.Next(ctx);
		}

		public override bool IsValid(CallingContext ctx)
		{
			if (scope.StillValid())
				if (NextCondition == null)
					return true;
				else
					return NextCondition.IsValid(ctx);
			return false;
		}

		public override Boolean HasMore(CallingContext ctx)
		{
			if (scope.StillValid())
				if (NextCondition == null)
					return true;
				else
					return NextCondition.HasMore(ctx);
			return false;
		}
	}

	class EofConditionChecker : IteratorCondtionChecker
	{
		public EofConditionChecker(IIteratorCondtionChecker next, CallingContext ctx)
			: base(next, ctx){	}

		public override void MoveToFirst(CallingContext ctx)
		{
			if (NextCondition != null)
				NextCondition.MoveToFirst(ctx);
		}

		public override Boolean Next(CallingContext ctx)
		{
			if (NextCondition == null)
				return true;
			return NextCondition.Next(ctx);
		}

		public override bool IsValid(CallingContext ctx)
		{
			if (NextCondition == null)
				return true;
			return NextCondition.IsValid(ctx);
		}

		public override Boolean HasMore(CallingContext ctx)
		{
			if (Session[Area].Eof())
				return false;

			if (NextCondition == null)
				return true;
			return NextCondition.HasMore(ctx);
		}
	}


	class RecordIterator : IRecordIterator
	{
		readonly IteratorCondtionChecker condition;

		public RecordIterator(CallingContext ctx, IteratorCondtionChecker conditions)
		{
			condition = conditions;
			MoveToFirst(ctx);
		}

		void MoveToFirst(CallingContext ctx)
		{
			condition.MoveToFirst(ctx);
			if (!condition.IsValid(ctx))
				Next(ctx);
		}

		public void Next(CallingContext ctx)
		{
			while (condition.HasMore(ctx))
			{
				if (condition.Next(ctx))
					break;
			}
		}

		public bool HasMore(CallingContext ctx)
		{
			return condition.HasMore(ctx);
		}
	}
}

/*

		public void Next(CallingContext ctx)
		{
			while (!Session[Area].Eof())
			{
				Scope.IncrementUsage();
				if (!Scope.StillValid())
					break;
				Session[Area].Skip(1);
				if (!Session[Area].Eof())
					if (CheckCondition(ctx))
						break;
			}
		}

 */