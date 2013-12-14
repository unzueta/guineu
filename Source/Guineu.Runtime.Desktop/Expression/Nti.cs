using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Guineu.Expression
{
	public struct Nti : IComparable<Nti>
	{
		static readonly SortedList<String, Int32> Names;
		static Int32 nextKey;
		readonly Int32 index;

		static Nti()
		{
			Names = new SortedList<String, Int32>(StringComparer.InvariantCultureIgnoreCase);
			RegisterKnownNti();
			nextKey = Names.Count + 1;
		}
		public Nti(String name)
		{
			if (IsName(name))
			{
				Int32 i = Names.IndexOfKey(name);
				if (i == -1)
					lock (Names)
					{
						index = nextKey++;
						Names.Add(name, index);
					}
				else
					index = Names.Values[i];
				//Index = i;
			}
			else
				index = 0;
		}
		public Nti(Char[] name)
			: this(new String(name))
		{ }
		public Nti(KnownNti nti)
		{
			index = (Int32)nti;
		}

		static Boolean IsName(String name)
		{
			if (String.IsNullOrEmpty(name))
				return false;
			return true;
		}

		public override string ToString()
		{
			var s = ToCasedString().ToUpper(CultureInfo.InvariantCulture);
			return s;
		}

		public String ToCasedString()
		{
			if (index == 0)
				return "";

			var i = Names.IndexOfValue(index);
			return Names.Keys[i];
		}

		public KnownNti ToKnownNti()
		{
			return (KnownNti)index;
		}

		public static implicit operator KnownNti(Nti value)
		{
			return (KnownNti)value.index;
		}

		public Boolean Valid()
		{
			return index > 0;
		}

		public static Boolean operator ==(Nti v1, Nti v2)
		{
			Boolean eq = (v1.index == v2.index);
			return eq;
		}
		public static Boolean operator !=(Nti v1, Nti v2)
		{
			Boolean eq = (v1.index != v2.index);
			return eq;
		}
		public override int GetHashCode()
		{
			return index;
		}
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (!(obj is Nti))
				return false;
			Int32 i = ((Nti)obj).index;
			if (i == index)
				return true;
			return false;
		}

		public static implicit operator Nti(KnownNti nti)
		{
			return new Nti(nti);
		}

		private static void RegisterKnownNti()
		{
			FieldInfo[] fi = typeof(KnownNti).GetFields(BindingFlags.Public | BindingFlags.Static);
			foreach (FieldInfo f in fi)
				Names.Add(f.Name, (Int32)f.GetValue(null));
		}

		public int CompareTo(Nti other)
		{
			if (index < other.index)
				return -1;
			if (index == other.index)
				return 0;
			return 1;
		}
	}

	public enum KnownNti
	{
		Empty = 1,
		Signature,
		BorderColor,
		Value,
		PenWidth,
		BorderStyle,
		This,
		ThisForm,
		ThisFormSet,
		TabIndex,
		FontCharSet,
		KeyPress,
		ScrollBars,
		Get,
		Put,
		LogRecordGather,
		On,
		Off,
		Status,
		Current,
		Available,
		RowSource,
		RowSourceType,
		SetFocus,
		Activate,
		Click,
		GotFocus,
		LostFocus,
		AddItem,
		Clear,
		InteractiveChange,
		RemoveItem,
		Release,
		Show,
		Unload,
		DownPicture,
		WordWrap,
		Stretch,
		PictureVal,
		ActivePage,
		ListCount,
		ListIndex,
		DisplayValue,
		Refresh,
		Valid,
		When,
		Page,
		Caption,
		ControlSource,
		ForeColor,
		BackColor,
		Tag,
		Left,
		Top,
		Width,
		Height,
		Visible,
		Enabled,
		BackStyle,
		Picture,
		WindowType,
		PageCount,
		ColumnCount,
		FontBold,
		FontUnderline,
		FontStrikeThru,
		FontItalic,
		FontSize,
		FontName,
		Parent,
		ReadOnly,
		RecordSource,
		GridLineColor,
		Column,
		ButtonClickOnEnter,
		Timer,
		Init,
		Load,
		Interval,
		SelStart,
		SelLength,
		Textbox,
		Move,
		AddObject,
		AddMethod,
		Assert,
		Case,
		Form,
		CommandButton,
		ListBox,
		Shape,
		Image,
		Label,
		Container,
		EditBox,
		CheckBox,
		PageFrame,
		ComboBox,
		Spinner,
		Header,
		Grid,
		CurrentControl,
		ScanTimeout,
		Configuration,
		GoodRead,
		About,
		Poin,
		Point,
		ControlBox
	}
}