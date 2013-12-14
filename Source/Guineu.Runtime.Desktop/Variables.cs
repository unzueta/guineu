using System;
using System.Collections.Generic;
using System.Diagnostics;
using Guineu.ObjectEngine;
using Guineu.Expression;

namespace Guineu
{

	abstract public class Member
	{
		internal abstract Member Clone();
	}

	// Used for variable, properties and array elements.
	public class ValueMember : Member
	{
		Variant value;

		public ValueMember(Variant value)
		{
			this.value = value;
		}

		public ValueMember()
		{
		}

		public Nti Nti { get; set; }

		virtual public Variant Get()
		{
			return value;
		}

		virtual public void Set(Variant val)
		{
			DoValidateValue(val);
			value = val;
		}

		/// <summary>
		/// Ensures that the passed value is valid
		/// </summary>
		virtual protected void DoValidateValue(Variant val)
		{
		}

		public VariantType Type
		{
			get { return Get().Type; }
		}

		virtual public void SetString(String val)
		{
			value = new Variant(val);
		}

		internal override Member Clone()
		{
			var newVal = new ValueMember();
			newVal.Set(new Variant(value));
			return newVal;
		}


	}

	//=====================================================================================
	partial class ArrayMember
	{
		ValueMember[] val;
		internal Byte Dimensions;
		internal Int64 Dimension1;
		internal Int64 Dimension2;

		internal ArrayMember(Int64 d1)
		{
			SetDimension(d1);
			val = new ValueMember[d1];
		}

		private void SetDimension(Int64 d1)
		{
			Dimensions = 1;
			Dimension1 = 1;
			Dimension2 = d1;
		}

		internal ArrayMember(Int64 d1, Int64 d2)
		{
			SetDimension(d1, d2);
			val = new ValueMember[d1 * d2];
		}

		private void SetDimension(Int64 d1, Int64 d2)
		{
			Dimensions = 2;
			Dimension1 = d1;
			Dimension2 = d2;
		}

		internal ValueMember Locate(Int64 row, Int64 col)
		{
			Int64 subscript = ASubscript(row, col);
			if (val[subscript] == null)
			{
				var defaultValue = new ValueMember();
				defaultValue.Set(new Variant(false));
				val[subscript] = defaultValue;
			}
			return val[subscript];
		}

		Int64 ASubscript(Int64 row, Int64 col)
		{
			if (row <= 0)
				throw new ErrorException(ErrorCodes.InvalidSubscript);
			if (col <= 0)
			{
				throw new ErrorException(ErrorCodes.InvalidSubscript);
			}
			Int64 subscript = (row - 1) * Dimension2 + col - 1;
			if (subscript > Dimension1 * Dimension2)
			{
				throw new ErrorException(ErrorCodes.InvalidSubscript);
			}
			return subscript;
		}

		//public override Variant Get()
		//{
		//  return Locate(1, 1).Get();
		//}

		public void ADelElement(Int64 element)
		{
			Debug.Assert(Dimensions == 1);
			if (element <= 0 || element > Dimension1 * Dimension2)
				throw new ErrorException(ErrorCodes.InvalidSubscript);
			for (Int64 t = element; t < Dimension1 * Dimension2; t++)
				val[t - 1] = val[t];
			val[(Dimension1 * Dimension2) - 1] = new ValueMember();
			val[(Dimension1 * Dimension2) - 1].Set(new Variant(false));
		}
		public void ADelRow(Int64 row)
		{
			Debug.Assert(Dimensions == 2);
			if (row <= 0)
				throw new ErrorException(ErrorCodes.InvalidSubscript);
			if (row > Dimension1)
				throw new ErrorException(ErrorCodes.InvalidSubscript);

			for (Int64 t = row * Dimension2; t < Dimension1 * Dimension2; t++)
				val[t - Dimension2] = val[t];
			for (Int64 t = 0; t < Dimension2; t++)
			{
				val[(Dimension1 * Dimension2) - 1 - t] = new ValueMember();
				val[(Dimension1 * Dimension2) - 1 - t].Set(new Variant(false));
			}
		}

		internal override Member Clone()
		{
			ArrayMember newVal;
			if (Dimensions == 1)
				newVal = new ArrayMember(Dimension2);
			else
				newVal = new ArrayMember(Dimension1, Dimension2);

			for (var i = 0; i < val.Length; i++)
				if (val[i] != null)
					newVal.val[i] = (ValueMember)val[i].Clone();
			return newVal;
		}
		// TODO: Implement an abstract interface for ValueMember that ArrayMember
		//       and ValueMember both implement.

	}

	//=====================================================================================
	public class MethodMember : Member
	{
		ObjectBase parentObj;

		internal ObjectBase Object
		{
			get { return parentObj; }
			set { parentObj = value; }
		}

		internal MethodMember(ObjectBase obj)
		{
			parentObj = obj;
		}

		internal override Member Clone()
		{
			var newMember = new MethodMember(Object);
			return newMember;
		}

		public Variant Execute(CallingContext cc, ParameterCollection param)
		{
			Variant retVal;
			if (parentObj == null)
				retVal = new Variant(true);
			else
			{
				// TODO: Deal with NODEFAULT
				ExecuteBefore(cc, param);
				retVal = ExecuteNative(cc, param);
			}
			return retVal;
		}

		public Variant Execute(ParameterCollection param)
		{
			return Execute(GuineuInstance.CallingContext, param);
		}

		internal virtual Variant ExecuteNative(CallingContext cc, ParameterCollection param)
		{
			return new Variant(true);
		}

		internal virtual void ExecuteBefore(CallingContext cc, ParameterCollection param)
		{
		}

		protected Variant ExecuteMethod(Nti name, CallingContext context, ParameterCollection param)
		{
			Variant ret = new Variant(true);
			ObjectBase obj = Object.GetCodeBase(name);
			if (obj != null)
			{
				MethodImplementation m = obj.GetMethod(name);
				using (var ctx = new CallingContext(GuineuInstance.Context, Object))
				{
					var parms = param ?? new ParameterCollection();
					ret = ctx.Context.ExecuteInNewContext(m.Code, parms, Object);
				}
			}
			return ret;
		}

	}

	//=====================================================================================
	delegate Variant NativeMethodCode(List<ValueMember> list);

	public class MemberList : SortedList<Nti, Member>, IMemberList
	{
		public MemberList Clone()
		{
			var list = new MemberList();
			for (int i = 0; i < Count; i++)
				list.Add(Keys[i], this[Keys[i]].Clone());
			return list;
		}

		public void AddProperty(Nti name, String value)
		{
			var property = new ValueMember();
			property.Set(new Variant(value));
			Add(name, property);
		}

		public void AddProperty(Nti name, Int32 value)
		{
			var property = new ValueMember();
			property.Set(new Variant(value, 10));
			Add(name, property);
		}

		public void AddProperty(Nti name, Variant value)
		{
			var property = new ValueMember();
			property.Set(value);
			Add(name, property);
		}

		public void AddProperty(Nti name, Boolean value)
		{
			var property = new ValueMember();
			property.Set(new Variant(value));
			Add(name, property);
		}

		internal Member Get(Nti name)
		{
			return GetMember(name);
		}

		public void Set(Nti name, Member value)
		{
			if (IndexOfKey(name) != -1)
				Remove(name);
			Add(name, value);
		}

		public Member GetMember(Nti name)
		{
			int index = IndexOfKey(name);
			if (index == -1)
				return null;

			return Values[index];
		}

		public Nti GetNameByPosition(int pos)
		{
			return GetMemberNameByPosition(pos);
		}
		public Nti GetMemberNameByPosition(int pos)
		{
			return Keys[pos];
		}

		internal void Release(Nti n, int p)
		{
			if (ContainsKey(n))
				Remove(n);
		}

		internal bool Exists(Nti name)
		{
			return IndexOfKey(name) >= 0;
		}

		public void Set(Nti n, Variant value)
		{
			ValueMember item;
			int index = IndexOfKey(n);
			if (index == -1)
			{
				item = new ValueMember();
				Add(n, item);
			}
			else
				item = (ValueMember)Values[index];
			item.Set(value);
		}

		internal void AddVariable(CallingContext ctx, ExpressionBase variable, ValueMember value)
		{
			var nti = variable.ToNti(ctx);
			var var = variable as Variable;
			var arr = variable as ArrayDefinition;
			if (var != null)
			{
				// TODO: Handle reference parameters
				if (value == null)
				{
					value = new ValueMember();
					value.Set(new Variant(false));
				}
				if (!Exists(nti))
					Add(nti, value);
			}

				// Definition of an array
			else if (arr != null)
			{
				Release(nti, arr.Index);
				Add(nti, arr.CreateMember(ctx));
			}

				// The variable is given as an expression, e.g. LOCAL ("myVar"). The expression
			// can be a variable Name or an array definition.
			//
			// TODO: Handle array definitions here.
			else if (!Exists(nti))
				Add(nti, new ValueMember());
		}

	}

	//=====================================================================================
	public class PropertyMember : ValueMember
	{
		Boolean isDefaultField = true;

		public PropertyMember() { }
		public PropertyMember(Variant value)
		{
			base.Set(value);
		}

		public Boolean IsDefault
		{
			get { return isDefaultField; }
		}

		override public void Set(Variant val)
		{
			isDefaultField = false;
			base.Set(val);
		}

	}



	public class ControlMember : ValueMember
	{
		readonly ObjectBase control;

		public ControlMember(ObjectBase ctrl)
		{
			control = ctrl;
		}

		public override Variant Get()
		{
			return new Variant(control);
		}

		public override void Set(Variant val)
		{
			throw new ErrorException(ErrorCodes.IsMethodEventOrObject);
		}
	}
}