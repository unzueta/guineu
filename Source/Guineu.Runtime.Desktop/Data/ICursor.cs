using System;
using System.Collections.Generic;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu.Data
{
	public interface ICursor
	{
		Int64 RecCount { get; }
		Int64 RecNo { get; }
		void GoTo(Int64 recNo);
		void GoToTop();
		void GoToBottom();
		Boolean Eof();
		Boolean Bof();
		void Skip(Int64 cnt);
		MemberList Fields { get; }
		Nti Alias { get; }
		void Append();
		void Delete();
		Boolean Deleted();
		void SetFilter(ExpressionBase filter);
		// TODO: Maybe remove setter. Is currently needed because of LOCATE.
		Boolean Found { get; set; }
		ExpressionBase LocateExpression { get; set; }
		void Zap();
		String Filename { get; }

		void SetField(Nti fieldName, Variant val);
		String GetFieldName(Int32 field);


		void Close();

		void FlushRecord();

		event EventHandler<EventArgs> RecordMoved;
		event EventHandler<EventArgs> CursorClosed;
		event EventHandler RecordCountChanged;

		IMemberList GetCurrentRecord();
		IMemberList GetEmptyRecord();
		List<ColumnDefinition> Columns { get; set; }
		ExpressionBase GetFilter();
	}

	public class ColumnDefinition
	{
		public Nti Name { get; set; }
		public ColumnType Type { get; set; }
	}

	public enum ColumnType
	{
		Blob,
		Character,
		Currency,
		Double,
		Date,
		DateTime,
		Float,
		General,
		Integer,
		Logical,
		Memo,
		Numeric,
		Picture,
		Varbinary,
		Varchar
	}
}