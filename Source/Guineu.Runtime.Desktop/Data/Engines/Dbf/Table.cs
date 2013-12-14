using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Guineu.Data.Engines.Dbf.Index;
using Guineu.Expression;
using Guineu.Data.Dbf;
using Guineu.ObjectEngine;


namespace Guineu.Data.Engines.Dbf
{
	public sealed class Table : ICursor, IDisposable
	{
		readonly DbfTable physicalTable;
		readonly Nti aliasName;
		readonly MemberList fieldList;
		IndexTag tag;
		ExpressionBase filter;

		INavigator navigator;

		// Currently loaded record
		Record recordBuffer;

		Boolean lastSearchResult;
		readonly String file;

		Boolean sourceChanged;


		/// <summary>
		/// Creates a cursor from the result of an SPT query
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="path"></param>
		/// <param name="exclusive"></param>
		public Table(String alias, String path, Boolean exclusive)
		{
			aliasName = new Nti(alias);
			file = Path.ChangeExtension(GuineuInstance.FileMgr.FullPath(path,false), "DBF");
			fieldList = new MemberList();
			columnsList = new List<ColumnDefinition>();

			physicalTable = new DbfTable(path, exclusive);
			navigator = new TableNavigator(physicalTable);
	
			navigator.GoTop();
			LoadCurrentRecord();
			OnRecordMoved();
		}

		public String Filename
		{
			get { return file; }
		}

		public void Close()
		{
			// (...) Eventuell nach Dispose verschieben, dann aber als Dispose Pattern
			OnCursorClosed();
			Dispose();
		}

		public void Dispose()
		{
			physicalTable.Close();
		}

		public Boolean Found
		{
			get { return lastSearchResult; }
			set { lastSearchResult = value; }
		}

		public ExpressionBase LocateExpression { get; set; }

		// Loads the current record in our member list.
		void LoadRecord()
		{
			Debug.Assert(RecCount > 0);
			Debug.Assert(navigator.RecordNumber != 0);

			recordBuffer = physicalTable.GetRecord(navigator.RecordNumber);
			LoadMemberList(fieldList, recordBuffer);
		}

		private void LoadMemberList(MemberList list, Record record)
		{

			FieldDefinitionEntry def;
			for (Int32 fld = 0; fld < physicalTable.Definition.Count; fld++)
			{
				def = physicalTable.Definition[fld + 1];
				var val = (ValueMember)list.Get(def.Nti);
				if (val == null)
				{
					val = new ValueMember();
					list.Add(new Nti(def.Name), val);
				}
				val.Set(record.AsVariant(fld));
			}
		}

		// Loads the current record in our member list.
		void LoadEmptyRecord()
		{
			recordBuffer = physicalTable.GetRecord(0);
			LoadMemberList(fieldList, recordBuffer);
		}


		#region ICursor Members

		public long RecCount
		{
			get
			{
				return physicalTable.GetRecCount();
			}
		}

		public void GoTo(long recNo)
		{
			navigator.GoTo(recNo);
			LoadCurrentRecord();
			OnRecordMoved();
		}

		private void LoadCurrentRecord()
		{
			if (Eof())
				LoadEmptyRecord();
			else
				LoadRecord();
		}

		public bool Eof()
		{
			if (RecCount == 0)
				return true;
			return navigator.IsEof;
		}

		public bool Bof()
		{
			if (RecCount == 0)
				return true;
			return navigator.IsBof;
		}

		public void Skip(long cnt)
		{
			if (cnt < 0)
				while (cnt < 0 && !Bof())
				{
					navigator.SkipBackward();
					if (GuineuInstance.Set.Deleted.Value)
						while (Deleted() && !Bof())
							navigator.SkipBackward();
					cnt = cnt + 1;
				}
			else
				while (cnt > 0 && !Eof())
				{
					navigator.SkipForward();
					if (GuineuInstance.Set.Deleted.Value)
						while (Deleted())
							navigator.SkipForward();
					cnt = cnt - 1;
				}
			LoadCurrentRecord();
			OnRecordMoved();
		}

		public MemberList Fields
		{
			get 
			{
				if (recordBuffer.RecordLoaded != navigator.RecordNumber)
					LoadCurrentRecord();

				return fieldList; 
			}
		}
		public Nti Alias
		{
			get { return aliasName; }
		}
		public long RecNo
		{
			get { return navigator.RecordNumber; }
		}


		public void Append()
		{
			Int64 newRec = physicalTable.Append();
			navigator.GoTo(newRec);
			LoadCurrentRecord();
			OnRecordCountChanged();
			OnRecordMoved();
		}

		public void Zap()
		{
			physicalTable.Zap();
			navigator.GoTop();
			LoadCurrentRecord();
			OnRecordMoved();
			OnRecordCountChanged();
		}

		public void SetField(Nti fieldName, Variant val)
		{
			recordBuffer.SetField(fieldName, val);
		}

		public void FlushRecord()
		{
			Byte[] content;
			if (recordBuffer.RecordLoaded == 0)
				content = physicalTable.BlankRawRecord();
			else
				content = physicalTable.ReadRawRecord(recordBuffer.RecordLoaded);

			recordBuffer.Gather(content);
			physicalTable.WriteRawRecord(recordBuffer.RecordLoaded, content);
			LoadRecord();
			if (sourceChanged)
				OnRecordCountChanged();
		}

		public void SetOrder(String order)
		{
			if (order == null)
			{
				tag = null;
				SetNavigator(new TableNavigator(physicalTable));
			}
			else
			{
				tag = physicalTable.Cdx.GetTag(order.ToUpper(System.Globalization.CultureInfo.InvariantCulture));
				if (tag == null)
					throw new ErrorException(ErrorCodes.IndexTagIsNotFound, order);
				SetNavigator(new IndexNavigator(tag));
			}
		}

		// TODO: Index order applies to loaded indexes, not the index File. 99.9% of all
		//       VFP applications only use a single index File.
		public void SetOrder(Int32 order)
		{
			if (order == 0)
			{
				tag = null;
				SetNavigator(new TableNavigator(physicalTable));
			}
			else
			{
				tag = physicalTable.Cdx.GetTag("");
				SetNavigator(new IndexNavigator(tag));
			}
		}

		private void SetNavigator(INavigator newNavigator)
		{
			if(navigator.RecordNumber==0)
				newNavigator.GoTo(1);
			else
				newNavigator.GoTo(navigator.RecordNumber);
			newNavigator.SetFilter(filter);
			navigator = newNavigator;
			LoadCurrentRecord();
			OnRecordMoved();
		}

		public void Seek(Variant value)
		{
			if (tag == null)
				throw new ErrorException(ErrorCodes.NoIndexOrderSet);

			KeyItem key = tag.KeyItem.New(value);
			SeekResult res = tag.Seek(key, SeekOptions.None);
			// TODO: Do not move the record pointer, but specify index coordinates as well. 
			//       Otherwise SEEK also invalidates the index making it slow.
			if (res.Found)
			{
				navigator.GoTo(res.Record);
				if (GuineuInstance.Set.Deleted.Value)
					while (Deleted())
						navigator.SkipForward();
				lastSearchResult = !navigator.IsEof;
			}
			else
			{
				navigator.GoEof();
				lastSearchResult = false;
			}
			LoadCurrentRecord();
			OnRecordMoved();
		}

		public void GoToTop()
		{
			navigator.GoTop();
			LoadCurrentRecord();
			OnRecordMoved();
		}

		public void GoToBottom()
		{
			navigator.GoBottom();
			LoadCurrentRecord();
			OnRecordMoved();
		}

		public string GetFieldName(int field)
		{
			if (field <= 0 || field > physicalTable.Definition.Count)
				return String.Empty;
			return physicalTable.Definition[field].Name;
		}

		public void Delete()
		{
			recordBuffer.Delete();
			sourceChanged = true;
		}

		public Boolean Deleted()
		{
			LoadCurrentRecord();
			return recordBuffer.Deleted();
		}
		#endregion

		#region ICursor Members


		public void SetFilter(ExpressionBase newFilter)
		{
			filter = newFilter;
			navigator.SetFilter(newFilter);
			OnRecordCountChanged();
		}

		#endregion

		#region ICursor Members


		public event EventHandler<EventArgs> RecordMoved;
		public event EventHandler RecordCountChanged;

		public IMemberList GetCurrentRecord()
		{
			return Fields.Clone();
		}

		public IMemberList GetEmptyRecord()
		{
			var record = physicalTable.GetRecord(0);
			var list = new MemberList();
			LoadMemberList(list, record);
			return list;
		}

		readonly List<ColumnDefinition> columnsList;
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
			return filter;
		}

		private void FillColumnList()
		{
			var coldef = physicalTable.Definition;
			for (int i = 1; i <= coldef.Count; i++)
			{
				columnsList.Add(
					new ColumnDefinition
						{
							Name = coldef[i].Nti,
							Type = coldef[i].FType.ToColumnType()
						}
					);
			}
		}

		public event EventHandler<EventArgs> CursorClosed;

		#endregion

		// (...) Test for record number and fire only if actually moved?
		void OnRecordMoved()
		{
			if (RecordMoved != null)
				RecordMoved(this, new EventArgs());
		}

		void OnCursorClosed()
		{
			if (CursorClosed != null)
				CursorClosed(this, new EventArgs());
		}
		void OnRecordCountChanged()
		{
			var buffer = recordBuffer;
			Int64 pos = navigator.RecordNumber;
			recordBuffer = physicalTable.GetRecord(pos);

			if (RecordCountChanged != null)
				RecordCountChanged(this, new EventArgs());
			
			navigator.GoTo(pos);
			recordBuffer = buffer;
			sourceChanged = false;
		}
	}
}