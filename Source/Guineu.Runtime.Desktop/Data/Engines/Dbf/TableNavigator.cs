using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Data;
using Guineu.Data.Engines.Dbf;
using Guineu.Expression;

namespace Guineu.Data.Dbf
{
	class TableNavigator : INavigator
	{
		readonly DbfTable table;
		Int64 currentRecord;
		ExpressionBase filter;

		public TableNavigator(DbfTable table)
		{
			this.table = table;
			currentRecord = 0;
		}

		public void GoBottom()
		{
			currentRecord = table.Header.GetRecCount();
			while (!RecordIsValid())
				SkipBackward();
		}
		public void GoTo(long recNo)
		{
			currentRecord = recNo;
		}
		public void GoTop()
		{
			currentRecord = 1;
			while (!RecordIsValid())
				SkipForward();
		}
		public void GoEof()
		{
			GoBottom();
			if (!IsEof)
				SkipForward();
		}

		public bool IsBof
		{
			get { return (currentRecord <= 0); }
		}
		public bool IsEof
		{
			get { return table.Header.GetRecCount() == 0 || currentRecord > table.Header.GetRecCount(); }
		}

		public long RecordNumber
		{
			get 
			{
				if (currentRecord <= 0)
					return 1;
				else
					return currentRecord;
			}
		}

		public void SkipBackward()
		{
			do
			{
				currentRecord = currentRecord - 1;
			} while (!RecordIsValid());
		}
		public void SkipForward()
		{
			do
			{
				currentRecord = currentRecord + 1;
			} while (!RecordIsValid());
		}

		public void SetFilter(Guineu.Expression.ExpressionBase filter)
		{
			this.filter = filter;
		}
		Boolean RecordIsValid()
		{
                if (filter == null)
				return true;
			else
				return filter.GetBool() || IsBof || IsEof;
		}
	}
}
