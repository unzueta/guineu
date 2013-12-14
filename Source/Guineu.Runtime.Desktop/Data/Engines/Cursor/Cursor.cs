using System;
using System.Collections.Generic;
using System.Data;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu.Data.Engines.Cursor
{
	public class Cursor : ICursor
	{
		readonly DataTable table;
		readonly Nti aliasName;
		Int32 current;
		readonly MemberList fieldList;

		/// <summary>
		/// Creates a cursor from the result of an SPT query
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="table"></param>
		public Cursor(String alias, DataTable table)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}

			aliasName = new Nti(alias);
			this.table = table;
			fieldList = new MemberList();
			//_Fields = new MemberList(table.Columns.Count);
			GoTo(1);
		}

		// TODO: Return an actual file Name here.
		public String Filename
		{
			get { return "12345678.TMP"; }
		}

		public void Close()
		{
			OnCursorClosed();
		}

		public Boolean Found { get; set; }

		public ExpressionBase LocateExpression { get; set; }

		// Loads the current record in our member list.
		void LoadRecord(MemberList list)
		{
			for (Int32 fld = 0; fld < table.Columns.Count; fld++)
			{
				var val = (ValueMember)list.Get(new Nti(table.Columns[fld].ColumnName));
				if (val == null)
				{
					val = new ValueMember();
					list.Add(new Nti(table.Columns[fld].ColumnName), val);
				}

				Variant var;
				if (table.Columns[fld].DataType == typeof(String))
					if (table.Rows[current - 1][fld] is DBNull)
						var = new Variant(VariantType.Character, true);
					else
					{
						var str = (String)table.Rows[current - 1][fld];
						if (table.Columns[fld].MaxLength > 0)
						{
							str = str.PadRight(table.Columns[fld].MaxLength);
						}
						var = new Variant(str);
					}
				
				else if (table.Columns[fld].DataType == typeof(Int32))
					if (table.Rows[current - 1][fld] is DBNull)
						var = new Variant(VariantType.Integer, true);
					else
						var = new Variant((Int32)table.Rows[current - 1][fld], 10);

				else if (table.Columns[fld].DataType == typeof(double))
					if (table.Rows[current - 1][fld] is DBNull)
						var = new Variant(VariantType.Number, true);
					else
						var = new Variant((double)table.Rows[current - 1][fld], 20, 10);

				else if (table.Columns[fld].DataType == typeof(decimal))
					if (table.Rows[current - 1][fld] is DBNull)
						var = new Variant(VariantType.Number, true);
					else
						var = new Variant((double)((decimal)table.Rows[current - 1][fld]), 20, 10);

				else if (table.Columns[fld].DataType == typeof(Boolean))
					if (table.Rows[current - 1][fld] is DBNull)
						var = new Variant(VariantType.Logical, true);
					else
						var = new Variant((Boolean)table.Rows[current - 1][fld]);

				else if (table.Columns[fld].DataType == typeof(DateTime))
					if (table.Rows[current - 1][fld] is DBNull)
						var = new Variant(VariantType.DateTime, true);
					else
						var = new Variant((DateTime)table.Rows[current - 1][fld], VariantType.DateTime);

				else
					if (table.Rows[current - 1][fld] is DBNull)
						var = new Variant(VariantType.Character, true);
					else
						var = new Variant(false);

				val.Set(var);
			}
		}

		public void Delete()
		{
			table.Rows.RemoveAt(current - 1);
		}

		public Boolean Deleted()
		{
			// Deleted is deleted
			return false;
		}

		// Loads the current record in our member list.
		void LoadEmptyRecord()
		{
			LoadEmptyRecord(fieldList);
		}

		void LoadEmptyRecord(MemberList list)
		{
			for (Int32 fld = 0; fld < table.Columns.Count; fld++)
			{
				var val = (ValueMember)list.Get(new Nti(table.Columns[fld].ColumnName));
				if (val == null)
				{
					val = new ValueMember();
					list.Add(new Nti(table.Columns[fld].ColumnName), val);
				}

				Variant var;
				if (table.Columns[fld].DataType == typeof(String))
				{
					var = new Variant("");
				}
				else if (table.Columns[fld].DataType == typeof(Int32))
				{
					var = new Variant(0, 10);
				}
				else if (table.Columns[fld].DataType == typeof(double))
				{
					var = new Variant(0, 20, 10);
				}
				else if (table.Columns[fld].DataType == typeof(decimal))
				{
					var = new Variant(0, 20, 10);
				}
				else
				{
					var = new Variant(false);
				}
				val.Set(var);
			}
		}


		#region ICursor Members

		public long RecCount
		{
			get { return table.Rows.Count; }
		}

		public void GoTo(long recNo)
		{
			current = (Int32)recNo;
			if (Eof())
			{
				LoadEmptyRecord();
			}
			else
			{
				LoadRecord(fieldList);
			}
			OnRecordMoved();
		}

		public bool Eof()
		{
			return current > table.Rows.Count;
		}
		public bool Bof()
		{
			return current < 1;
		}

		public void Skip(long cnt)
		{
			current = current + (Int32)cnt;

			if (Eof())
			{
				LoadEmptyRecord();
				current = table.Rows.Count + 1;
			}
			else
			{
				LoadRecord(fieldList);
			}
			OnRecordMoved();
		}

		public MemberList Fields
		{
			get { return fieldList; }
		}

		public Nti Alias
		{
			get
			{
				return aliasName;
			}
		}

		public long RecNo
		{
			get { return current; }
		}

		public void Zap()
		{
			table.Rows.Clear();
			current = 0;
			OnRecordCountChanged();
			OnRecordMoved();
		}
		public void Append()
		{
			var row = new object[table.Columns.Count];

			for (var fld = 0; fld < table.Columns.Count; fld++)
			{
				var dataType = table.Columns[fld].DataType;

				if (dataType == typeof(String))
					row[fld] = "";
				else if (dataType == typeof(Int32))
					row[fld] = 0;
				else if (dataType == typeof(double))
					row[fld] = 0;
			}

			table.Rows.Add(row);
			current = table.Rows.Count;
			OnRecordCountChanged();
			OnRecordMoved();
		}

		public void SetField(Nti name, Variant val)
		{
			String fieldName = name.ToString();
			if (table.Columns[fieldName].DataType == typeof(String))
			{
				table.Rows[current - 1][fieldName] = val.ToString(null);
			}
			else if (table.Columns[fieldName].DataType == typeof(Int32))
			{
				table.Rows[current - 1][fieldName] = (Int32)val;
			}
			else if (table.Columns[fieldName].DataType == typeof(double))
			{
				table.Rows[current - 1][fieldName] = (Double)val;
			}
			else if (table.Columns[fieldName].DataType == typeof(decimal))
			{
				table.Rows[current - 1][fieldName] = (decimal)val;
			}
		}

		public void FlushRecord()
		{
			LoadRecord(fieldList);
		}

		public void GoToTop()
		{
			current = 1;
			if (Eof())
			{
				LoadEmptyRecord();
				current = table.Rows.Count + 1;
			}
			else
			{
				LoadRecord(fieldList);
			}
			OnRecordMoved();
		}

		public void GoToBottom()
		{
			current = table.Rows.Count;
			if (Eof())
			{
				LoadEmptyRecord();
				current = table.Rows.Count + 1;
			}
			else
			{
				LoadRecord(fieldList);
			}
			OnRecordMoved();
		}

		public string GetFieldName(int field)
		{
			if (field <= 0 || field >= table.Columns.Count)
				return String.Empty;
			return table.Columns[field - 1].ColumnName;
		}

		public void SetFilter(ExpressionBase filter)
		{
		}

		#endregion
		#region ICursor Members


		public event EventHandler<EventArgs> RecordMoved;
		public event EventHandler<EventArgs> CursorClosed;
		public event EventHandler RecordCountChanged;

		#endregion

		protected void OnRecordMoved()
		{
			if (RecordMoved != null)
				RecordMoved(this, new EventArgs());
		}

		protected void OnRecordCountChanged()
		{
			if (RecordCountChanged != null)
				RecordCountChanged(this, new EventArgs());
		}


		protected void OnCursorClosed()
		{
			if (CursorClosed != null)
				CursorClosed(this, new EventArgs());
		}

		public IMemberList GetCurrentRecord()
		{
			return Fields.Clone();
		}

		public IMemberList GetEmptyRecord()
		{
			var list = new MemberList();
			LoadEmptyRecord(list);
			return list;
		}

		readonly List<ColumnDefinition> columnsList = new List<ColumnDefinition>();
		public List<ColumnDefinition> Columns
		{
			get
			{
				if (columnsList.Count == 0)
					FillColumnList();
				return columnsList;
			}
			set { throw new InvalidOperationException(); }
		}

		public ExpressionBase GetFilter()
		{
			return GetFilter();
		}

		private void FillColumnList()
		{
			var coldef = table.Columns;
			for (var i = 0; i < coldef.Count; i++)
			{
				columnsList.Add(
					new ColumnDefinition
						{
							Name = new Nti(coldef[i].ColumnName),
							Type = GetDataType(coldef[i].DataType)
						}
					);
			}
		}

		private static ColumnType GetDataType(Type type)
		{
			if (type == typeof(String))
				return ColumnType.Character;
			if (type == typeof(decimal))
				return ColumnType.Numeric;
			if (type == typeof(Int32))
				return ColumnType.Integer;
			if (type == typeof(double))
				return ColumnType.Double;
			return ColumnType.Logical;
		}


	}
}