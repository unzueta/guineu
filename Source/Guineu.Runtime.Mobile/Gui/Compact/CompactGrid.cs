using System;
using System.ComponentModel;
using System.Windows.Forms;
using Guineu.Data;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Gui.Compact
{
	class CompactGrid : DataGrid, IControl, IGuiGrid, IRecordSourceViewBuilder
	{
		DataGridTableStyle ts;
		RecordSourceCollection rsc;
		Boolean readOnly;

		public void GuiPreInit()
		{
			ts = new DataGridTableStyle { MappingName = "_" + Guid.NewGuid() };
		}

		public void GuiPostInit()
		{
			// The model added view objects for all contained controls. Now it's time
			// to turn those into view objects (aka GridColumnStyles)
			for (var i = 0; i < columns.Count; i++)
			{
				CompactColumn ctrl = columns[i];
				DataGridColumnStyle style;

				if (ctrl.CurrentControl is CompactImage)
					style = new DataGridImageColumn(ctrl.CurrentControl as CompactImage);
				else if (ctrl.CurrentControl is CompactButton)
					style = new DataGridButtonColumn(ctrl.CurrentControl as CompactButton);
				else
					style = new DataGridTextBoxColumn();

				style.MappingName = "column"+i;
				style.HeaderText = ctrl.Header.Caption;
				style.Width = ctrl.Width;

				ts.GridColumnStyles.Add(style);
				ctrl.LinkToGrid(this, i);
			}

			// replace current view in grid with new columns and link to data.
			if (recordSource != null)
			{
				rsc = new RecordSourceCollection(this, ts.MappingName);
				FillRecordSourceCollection();
				TableStyles.Clear();
				TableStyles.Add(ts);
				DataSource = rsc;
			}

			isInitialized = true;
		}

		internal DataGridColumnStyle GridColumnStyles(Int32 col)
		{
			return ts.GridColumnStyles[col];
		}


		private void FillRecordSourceCollection()
		{
			StopEvent();
			Int64 recNo = recordSource.RecNo;
			recordSource.GoToTop();
			Int32 curPos = 0;
			while (!recordSource.Eof())
			{
				var item = new RecordSource(this, recordSource, recordSource.RecNo);
				if (curPos < rsc.Count)
					rsc[curPos] = item;
				else
					rsc.Add(item);
				recordSource.Skip(+1);
				curPos++;
			}
			recordSource.GoTo(recNo);
			while (rsc.Count > curPos)
				rsc.RemoveAt(rsc.Count - 1);
			StartEvent();
		}


		ICursor recordSource;
		public ICursor GuiRecordSource
		{
			get { return recordSource; }
			set
			{
				StopEvent();
				recordSource = value;
				if(isInitialized)
					GuiPostInit();
				StartEvent();
			}
		}

		internal void StopEvent()
		{
			if (recordSource != null)
			{
				recordSource.RecordMoved -= RecordMovedHandler;
				recordSource.CursorClosed -= CursorClosedHandler;
				recordSource.RecordCountChanged -= RecordCountChangedHandler;
			}
		}

		internal void StartEvent()
		{
			if (recordSource != null)
			{
				recordSource.RecordMoved += RecordMovedHandler;
				recordSource.CursorClosed += CursorClosedHandler;
				recordSource.RecordCountChanged += RecordCountChangedHandler;
			}
		}

		void RecordCountChangedHandler(object sender, EventArgs e)
		{
			FillRecordSourceCollection();
		}

		void RecordMovedHandler(object sender, EventArgs e)
		{
			var cursor = (ICursor)sender;
			var column = CurrentCell.ColumnNumber;
			var row = -1;
			var recNo = (Int32)cursor.RecNo;
			for (var check = 0; check < rsc.Count; check++)
				if (rsc[check].RecNo == recNo)
				{
					row = check;
					break;
				}
			if (row >= rsc.Count)
				row = rsc.Count - 1;
			if (row >= 0)
				CurrentCell = new DataGridCell(row, column);
		}

		void CursorClosedHandler(object sender, EventArgs e)
		{
			DataSource = null;
		}

		protected override void OnCurrentCellChanged(EventArgs e)
		{
			Int64 recNo = rsc[CurrentCell.RowNumber].RecNo;
			if (recordSource.RecNo != recNo)
				if (recNo <= recordSource.RecCount)
				{
					StopEvent();
					recordSource.GoTo(recNo);
					StartEvent();
				}
			// (...) ChangeRowCol
			base.OnCurrentCellChanged(e);
		}

		static Int32 ScaleDown(Int32 hires)
		{
			var mgr = (CompactManager)GuineuInstance.WinMgr;
			var lores = (Int32)Math.Round(hires / mgr.Scale, 0);
			return lores;
		}

		static Int32 ScaleUp(Int32 lores)
		{
			var mgr = (CompactManager)GuineuInstance.WinMgr;
			var hires = (Int32)Math.Round(lores * mgr.Scale, 0);
			return hires;
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			RaiseorForwardEvent(KnownNti.Click);
		}
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			this.GotFocusEvent(this, EventHandler);
		}
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			this.LostFocusEvent(this, EventHandler, GetValue());
		}

		void RaiseorForwardEvent(KnownNti name)
		{
			var ctrl = columns[CurrentCell.ColumnNumber].CurrentControl as IGridHosted;
			if(ctrl == null)
				this.CallEvent(EventHandler, name);
			else
				ctrl.ForwardEvent(name, new ParameterCollection());	
		}

		public void BuildView(List<PropertyDescriptor> props)
		{
			for (var i = 0; i < columns.Count; i++)
			{
				CompactColumn col1 = columns[i];
				RecordSourceMethodDelegate del = p => p.GetString(col1);
				// (...) instead of typeof we need to get the actual column type.
				props.Add( new RecordSourceMethodDescriptor(
					ts.GridColumnStyles[i].MappingName,
					del,
					typeof(string)
				));
			}
		}

		public void SetVariant(KnownNti nti, Variant value)
		{
			switch (nti)
			{
				case KnownNti.BackColor:
					BackColor = new Color(value);
					break;

				case KnownNti.Enabled:
					Enabled = value;
					break;

				case KnownNti.ForeColor:
					ForeColor = new Color(value);
					break;

				case KnownNti.GridLineColor:
					GridLineColor = new Color(value);
					break;

				case KnownNti.Left:
					Left = ScaleUp(value);
					break;

				case KnownNti.Width:
					Width = ScaleUp(value);
					break;

				case KnownNti.Top:
					Top = ScaleUp(value);
					break;

				case KnownNti.Height:
					Height = ScaleUp(value);
					break;

				case KnownNti.ReadOnly:
					readOnly = value;
					break;

				case KnownNti.TabIndex:
					TabIndex = value;
					break;

				case KnownNti.Value:
					type = value.Type;
					Text = value;
					break;

				case KnownNti.Visible:
					Visible = value;
					break;

				default:
					if (FontHandling.Handles(nti))
						FontHandling.Set(this, nti, value);
					else
						throw new ErrorException(ErrorCodes.PropertyIsNotFound);
					break;
			}
		}

		public Variant GetVariant(KnownNti nti)
		{
			switch (nti)
			{
				case KnownNti.BackColor:
					return new Variant((Int32)(Color)BackColor, 10);

				case KnownNti.Enabled:
					return new Variant(Enabled);

				case KnownNti.ForeColor:
					return new Variant((Int32)(Color)ForeColor, 10);

				case KnownNti.GridLineColor:
					return new Variant((Int32)(Color)GridLineColor, 10);

				case KnownNti.Left:
					return new Variant(ScaleDown(Left), 10);

				case KnownNti.Top:
					return new Variant(ScaleDown(Top), 10);

				case KnownNti.Width:
					return new Variant(ScaleDown(Width), 10);

				case KnownNti.Height:
					return new Variant(ScaleDown(Height), 10);

				case KnownNti.ReadOnly:
					return new Variant(readOnly);

				case KnownNti.TabIndex:
					return new Variant(TabIndex, 10);

				case KnownNti.Value:
					return GetValue();

				case KnownNti.Visible:
					return new Variant(Visible);

				default:
					if (FontHandling.Handles(nti))
						return FontHandling.Get(this, nti);

					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		VariantType type;
		private Variant GetValue()
		{
			switch (type)
			{
				//case VariantType.Integer:
				//  break;
				//case VariantType.Logical:
				//  break;
				case VariantType.Character:
					return new Variant(Text);
				//case VariantType.Number:
				//  break;
				//case VariantType.Object:
				//  break;
				case VariantType.Date:
					DateTime dt;
					try
					{
						dt = DateTime.Parse(Text);
					}
					catch
					{
						dt = new DateTime(0);
					}
					return new Variant(dt);
				//case VariantType.DateTime:
				//  break;
				//case VariantType.Null:
				//  break;
				//case VariantType.Unknown:
				//  break;
				default:
					return new Variant(Text);
			}
		}

		readonly List<CompactColumn> columns = new List<CompactColumn>();
		bool isInitialized;

		public Variant CallMethod(KnownNti name, ParameterCollection parms)
		{
			switch (name)
			{
				case KnownNti.AddObject:
					var ctrl = (CompactColumn)parms[0].Get().ToNative();
					columns.Add(ctrl);
					break;

				case KnownNti.SetFocus:
					Focus();
					break;

				case KnownNti.Move:
					this.MoveControl(parms);
					break;

				case KnownNti.Refresh:
					Refresh();
					break;

			}
			return new Variant(true);
		}

		public event Action<EventData> EventHandler;
	}


	// (...) Credentials!!!

	public interface IRecordSourceViewBuilder
	{
		void BuildView(List<PropertyDescriptor> props);
	}
	internal class RecordSourceCollection : BindingList<RecordSource>, ITypedList
	{
		readonly String identifier;
		readonly IRecordSourceViewBuilder builder;

		public RecordSourceCollection(IRecordSourceViewBuilder viewBuilder, String name)
		{
			identifier = name;
			builder = viewBuilder;
		}

		#region ITypedList Members

		protected PropertyDescriptorCollection Props;

		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return Props ?? (Props = GetView());
		}


		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return identifier;
		}

		#endregion

		PropertyDescriptorCollection GetView()
		{
			var props = new List<PropertyDescriptor>();
			builder.BuildView(props);

			var propArray = new PropertyDescriptor[props.Count];
			props.CopyTo(propArray);

			return new PropertyDescriptorCollection(propArray);
		}
	}

	internal delegate object RecordSourceMethodDelegate(RecordSource source);

	internal class RecordSourceMethodDescriptor : PropertyDescriptor
	{
		protected RecordSourceMethodDelegate Method;
		protected Type MethodReturnType;

		public RecordSourceMethodDescriptor(string name, RecordSourceMethodDelegate method,
		 Type methodReturnType)
			: base(name, null)
		{
			Method = method;
			MethodReturnType = methodReturnType;
		}

		public override object GetValue(object component)
		{
			var p = (RecordSource)component;
			return Method(p);
		}

		public override Type ComponentType
		{
			get { return typeof(RecordSource); }
		}

		public override Type PropertyType
		{
			get { return MethodReturnType; }
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override void ResetValue(object component) { }

		public override bool IsReadOnly
		{
			get { return true; }
		}

		public override void SetValue(object component, object value) { }

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}

	internal class RecordSource
	{
		readonly ICursor cursor;
		Int64 recNo;
		readonly CompactGrid grid;
		public RecordSource(CompactGrid grid, ICursor csr, Int64 recNo)
		{
			cursor = csr;
			this.recNo = recNo;
			this.grid = grid;
		}
		public Int64 RecNo
		{
			get { return recNo; }
			set { recNo = value; }
		}
		internal String GetString(CompactColumn column)
		{
			grid.StopEvent();
			// (...) Wie könnten wir das absichern gegen Fehler?
			Int64 curRec = cursor.RecNo;
			cursor.GoTo(recNo);
			column.RaiseEvent(KnownNti.Refresh);
			String result = column.GetVariant(KnownNti.Value);
			// (...) Warum kann curRec an dieser Stelle 0 sein?
			if (curRec != 0)
				cursor.GoTo(curRec);
			grid.StartEvent();
			return result;
		}

	}
}
