using System;

namespace Guineu.Data
{
	abstract class Scope
	{
		public abstract void MoveToFirst(ICursor cursor);
		public abstract Boolean StillValid();
		public abstract void IncrementUsage();
	}

	class ScopeNext : Scope
	{
		public readonly Int64 Count;
		Int64 Visited;
		public ScopeNext(Int64 count)
		{
			Count = count;
		}
		public override void MoveToFirst(ICursor cursor)
		{
			Visited = 1;
		}
		public override bool StillValid()
		{
			return Visited <= Count;
		}
		public override void IncrementUsage()
		{
			Visited++;
		}
	}

	class ScopeAll : Scope
	{
		public override void MoveToFirst(ICursor cursor)
		{
			cursor.GoToTop();
		}
		public override bool StillValid()
		{
			return true;
		}
		public override void IncrementUsage() {}
	}

	class ScopeRest : Scope
	{
		public override void MoveToFirst(ICursor cursor) { }
		public override bool StillValid()
		{
			return true;
		}
		public override void IncrementUsage() { }
	}

	class ScopeRecord : Scope
	{
		readonly public Int64 Record;
		Boolean MovedAway;
		public ScopeRecord(Int64 rec)
		{
			Record = rec;
		}

		public override void MoveToFirst(ICursor cursor)
		{
			cursor.GoTo(Record);
			MovedAway = false;
		}
		public override bool StillValid()
		{
			return !MovedAway;
		}
		public override void IncrementUsage()
		{
			MovedAway = true;
		}
	}
}