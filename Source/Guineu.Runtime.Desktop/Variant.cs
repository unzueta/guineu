using System;
using System.Text;
using Guineu.Classes;
using Guineu.Expression;
using Guineu.ObjectEngine;
using Guineu.Properties;

namespace Guineu
{

	/// <summary>
	/// In memory type as it is used for variables and non-field expressions
	/// </summary>
	/// <value>Null</value>
	/// <remarks>
	/// Some expressions can be actually NULL. That is, VARTYPE() returns "X" for them
	/// no matter whether the parameter to reveil the actual type is .T. or .F. This
	/// is the case, for instance, with ICASE() when there is no matching value and
	/// no default. Assigning the result of ICASE() to a variable, however, changes the
	/// type from "X" to "L".
	/// </remarks>
	public enum VariantType
	{
		Unknown = 0,
		Integer,
		Logical,
		Character,
		Number,
		Object,
		Date,
		DateTime,
		Null,
		Binary,
		Native
	}


	/// <summary>
	/// Contains any type of value including variables, fields and properties
	/// </summary>
	public partial struct Variant
	{
		readonly VariantType type;

		readonly double doubleValue;
		readonly bool boolValue;
		readonly int intValue;
		readonly DateTime dateTime;

		readonly int length;
		readonly int decimals;
		readonly StringBuilder strValue;
		readonly ObjectBase objectValue;
		readonly Object nativeValue;

		readonly Boolean isNull;

		#region Constructors

		public Variant(Byte[] b)
			: this(VariantType.Character)
		{
			Char[] c = GuineuInstance.CurrentCp.GetChars(b);
			var str = new String(c);
			isNull = false;
			length = str.Length;
			strValue = new StringBuilder(str);
		}

		Variant(VariantType type)
		{
			this.type = type;

			boolValue = false;
			dateTime = new DateTime(0);
			decimals = 0;
			doubleValue = 0;
			intValue = 0;
			length = 0;
			objectValue = null;
			strValue = null;
			nativeValue = null;
			isNull = false;
		}

		public Variant(bool val)
			: this(VariantType.Logical)
		{
			boolValue = val;
		}

		public Variant(int val, int length)
			: this(VariantType.Integer)
		{
			this.length = length;
			intValue = val;
		}

		public Variant(double val, int length, Int32 decimals)
			: this(VariantType.Number)
		{
			this.length = length;
			this.decimals = decimals;
			doubleValue = val;
		}

		public Variant(ObjectBase val)
			: this(VariantType.Object)
		{
			objectValue = val;
		}

		// TODO: This catches every wrong declaration (new Variant(0))
		public Variant(Object val)
			: this(VariantType.Native)
		{
			nativeValue = val;
		}

		public Variant(DateTime val)
			: this(VariantType.DateTime)
		{
			dateTime = val;
		}

		public Variant(DateTime val, VariantType type)
			: this(val)
		{
			if (type != VariantType.DateTime && type != VariantType.Date)
			{
				throw new ErrorException(ErrorCodes.DataTypeMismatch);
			}
			this.type = type;
			// We do not remove hours, etc for date values since VFP knows about date values
			// with a date time portion: DATE()+2.1
		}

		public Variant(string str)
			: this(VariantType.Character)
		{
			if (str == null)
			{
				isNull = true;
				strValue = null;
				length = 0;
			}
			else
			{
				isNull = false;
				length = str.Length;
				strValue = new StringBuilder(str);
			}
		}

		public Variant(Variant var)
		{
			type = var.type;
			intValue = var.intValue;
			length = var.length;
			decimals = var.decimals;
			doubleValue = var.doubleValue;
			boolValue = var.boolValue;
			dateTime = var.dateTime;
			isNull = var.isNull;
			objectValue = var.objectValue;  // (...) handle reference handling
			nativeValue = var.nativeValue;

			if (isNull || var.strValue == null)
				strValue = null;
			else
				//TODO: Optimize code here
				strValue = new StringBuilder(var.strValue.ToString());
		}

		public Variant(VariantType type, Boolean isNull)
			: this(type)
		{
			this.isNull = isNull;
		}

		#endregion Constructors

		#region General Purpose methods
		public VariantType Type
		{
			get { return type; }
		}

		/* 
		 TODO : change the datetime to date ... TTOD, DTOT ... or create a new variant?
										{
																		if (m_type == VariantType.DateTime && newtype == VariantType.Date)
																		{
																										if (m_dateTime != null)
																										{   // reset time part
																																		m_dateTime.AddHours(-m_dateTime.Hour);
																																		m_dateTime.AddMinutes(-m_dateTime.Minute);
																																		m_dateTime.AddSeconds(-m_dateTime.Second);
																																		m_dateTime.AddMilliseconds(-m_dateTime.Millisecond);
																										}
																		}
																		m_type=newType;
										}
		 */

		public int Length()
		{
			return length;
		}
		#endregion General

		#region Operators

		public Boolean IsNull
		{
			get { return isNull; }
		}

		public Boolean IsEmpty
		{
			get
			{
				if (isNull)
					return false;
				switch (type)
				{
					case VariantType.Integer:
						return intValue == 0;
					case VariantType.Logical:
						return !boolValue;
					case VariantType.Character:
						if (strValue == null)
							return true;
						return length == 0;
					case VariantType.Number:
						return doubleValue == 0;
					case VariantType.Object:
						return false;
					case VariantType.Date:
					case VariantType.DateTime:
						return dateTime.Ticks == 0;
					case VariantType.Null:
						return false;
					default:
						return false;
				}
			}
		}

		public static Variant operator +(Variant v1, Variant v2)
		{
			Variant retVal;
			if (v1.type == VariantType.Integer)
			{
				if (v2.type == VariantType.Integer)
				{
					retVal = new Variant(
									v1.intValue + v2.intValue,
									(v1.length > v2.length ? v1.length + 1 : v2.length + 1)
					);
					return retVal;
				}
				if (v2.type == VariantType.Number)
				{
					retVal = new Variant(
									v1.intValue + v2.doubleValue,
									(v1.length > v2.length ? v1.length + 1 : v2.length + 1),
									v2.decimals
					);
					return retVal;
				}
				if (v2.type == VariantType.Date)
				{
					// add a date to an int, should get a new datetime
					retVal = new Variant(v2.dateTime.AddDays(v1.intValue), VariantType.Date);
					return retVal;
				}
				if (v2.type == VariantType.DateTime)
				{
					retVal = new Variant(v2.dateTime.AddSeconds(Math.Round((Double)v1, 0)));
					return retVal;
				}
			}

			if (v1.type == VariantType.Number)
			{
				if (v2.type == VariantType.Integer)
				{
					retVal = new Variant(
									v1.doubleValue + v2.intValue,
									v1.length > v2.length ? v1.length + 1 : v2.length + 1,
									v1.decimals
					);
					return retVal;
				}
				if (v2.type == VariantType.Number)
				{
					retVal = new Variant(
									v1.doubleValue + v2.doubleValue,
									v1.length > v2.length ? v1.length + 1 : v2.length + 1,
									v1.decimals
					);
					return retVal;
				}
				if (v2.type == VariantType.Date)
				{
					retVal = new Variant(v2.dateTime.AddDays(v1.doubleValue), VariantType.Date);
					return retVal;
				}
				if (v2.type == VariantType.DateTime)
				{
					retVal = new Variant(v2.dateTime.AddSeconds(Math.Round(v1.doubleValue, 0)));
					return retVal;
				}
			}

			if (v1.type == VariantType.DateTime)
			{
				if (v2.type == VariantType.Number || v2.type == VariantType.Integer)
				{
					retVal = new Variant(v1.dateTime.AddSeconds(Math.Round((Double)v2, 0)));
					return retVal;
				}
			}

			if (v1.type == VariantType.Date)
			{
				if (v2.type == VariantType.Number || v2.type == VariantType.Integer)
				{
					retVal = new Variant(v1.dateTime.AddDays(v2), VariantType.Date);
					return retVal;
				}
			}

			//---------------------------------------------------------------------------------
			// Adds two strings. 
			//---------------------------------------------------------------------------------
			if (v1.type == VariantType.Character && v2.type == VariantType.Character)
			{
				retVal = new Variant(v1.strValue + v2.strValue.ToString());
				return retVal;
			}

			throw new ErrorException(ErrorCodes.Syntax);

		}

		public bool IsEqual(Variant val)
		{
			// expressions with NULL always return NULL. 
			if(IsNull ||  val.IsNull)
				return false;

			if ((type == VariantType.Integer || type == VariantType.Number) && (val.type == VariantType.Integer || val.type == VariantType.Number))
			{
                // TODO: Fix this. 0 = 0.1 (Wrong)
				return this == (Int32)val;
			}

			if (Type == VariantType.Character && val.Type == VariantType.Character)
			{
				return StringBuilderHelper.Compare(strValue, val.strValue) == 0;
			}

			if ((type == VariantType.DateTime || type == VariantType.Date)
													&&
													(val.type == VariantType.DateTime || val.type == VariantType.Date))
				return dateTime.Equals(val.dateTime);

			if ((type == VariantType.Logical) && (val.type == VariantType.Logical))
			{
				return (Boolean)this == val;
			}
			return false;
		}
		public bool IsNotEqual(Variant val)
		{
			if (IsNull)
				return false;
			Boolean retVal = !IsEqual(val);
			return retVal;
		}

		public bool IsEqualBinary(Variant val)
		{
			if (Type == VariantType.Character && val.Type == VariantType.Character)
				if (IsNull || val.IsNull)
					return false;
				else
					return StringBuilderHelper.Compare(strValue, val.strValue) == 0;

			return IsEqual(val);
		}

		public Variant OperatorEquals(Variant val)
		{
			return new Variant(IsEqual(val));
		}

		public bool IsGreaterThan(Variant val)
		{
			if (type == VariantType.Integer)
			{
				if (val.type == VariantType.Integer)
				{
					return (intValue > val.intValue);
				}
				if (val.type == VariantType.Number)
				{
					return (intValue > val.doubleValue);
				}
			}

			if (type == VariantType.Number)
			{
				if (val.type == VariantType.Integer)
				{
					return (doubleValue > val.intValue);
				}
				if (val.type == VariantType.Number)
				{
					return (doubleValue > val.doubleValue);
				}
			}

			if (Type == VariantType.Character && val.Type == VariantType.Character)
			{
				bool isGreaterThan = StringBuilderHelper.Compare(strValue, val.strValue) > 0;
				return isGreaterThan;
			}

			if ((type == VariantType.DateTime || type == VariantType.Date)
													&&
													(val.type == VariantType.DateTime || val.type == VariantType.Date))
			{
				return dateTime.CompareTo(val.dateTime) > 0;
			}
			return false;
		}

		public bool IsLessThan(Variant val)
		{
			if (type == VariantType.Integer)
			{
				if (val.type == VariantType.Integer)
				{
					return (intValue < val.intValue);
				}
				if (val.type == VariantType.Number)
				{
					return (intValue < val.doubleValue);
				}
			}

			if (type == VariantType.Number)
			{
				if (val.type == VariantType.Integer)
				{
					return (doubleValue < val.intValue);
				}
				if (val.type == VariantType.Number)
				{
					return (doubleValue < val.doubleValue);
				}
			}

			if (Type == VariantType.Character && val.Type == VariantType.Character)
			{
				bool isLessThan = StringBuilderHelper.Compare(strValue, val.strValue) < 0;
				return isLessThan;
			}

			if ((type == VariantType.DateTime || type == VariantType.Date)
													&&
													(val.type == VariantType.DateTime || val.type == VariantType.Date))
			{
				return dateTime.CompareTo(val.dateTime) < 0;
			}

			return false;
		}

		public bool IsGreaterOrEqualThan(Variant val)
		{
			if (type == VariantType.Integer)
			{
				if (val.type == VariantType.Integer)
				{
					return (intValue >= val.intValue);
				}
				if (val.type == VariantType.Number)
				{
					return (intValue >= val.doubleValue);
				}
			}

			if (type == VariantType.Number)
			{
				if (val.type == VariantType.Integer)
				{
					return (doubleValue >= val.intValue);
				}
				if (val.type == VariantType.Number)
				{
					return (doubleValue >= val.doubleValue);
				}
			}

			if (Type == VariantType.Character && val.Type == VariantType.Character)
			{
				return StringBuilderHelper.Compare(strValue, val.strValue) >= 0;
			}

			if ((type == VariantType.DateTime || type == VariantType.Date)
													&&
													(val.type == VariantType.DateTime || val.type == VariantType.Date))
			{
				return dateTime.CompareTo(val.dateTime) >= 0;
			}

			return false;
		}

		public bool IsLessOrEqualThan(Variant val)
		{
			if (type == VariantType.Integer)
			{
				if (val.type == VariantType.Integer)
				{
					return (intValue <= val.intValue);
				}
				if (val.type == VariantType.Number)
				{
					return (intValue <= val.doubleValue);
				}
			}

			if (type == VariantType.Number)
			{
				if (val.type == VariantType.Integer)
				{
					return (doubleValue <= val.intValue);
				}
				if (val.type == VariantType.Number)
				{
					return (doubleValue <= val.doubleValue);
				}
			}

			if (Type == VariantType.Character && val.Type == VariantType.Character)
			{
				bool isLessThan = StringBuilderHelper.Compare(strValue, val.strValue) <= 0;
				return isLessThan;
			}

			if ((type == VariantType.DateTime || type == VariantType.Date)
													&&
													(val.type == VariantType.DateTime || val.type == VariantType.Date))
			{
				return dateTime.CompareTo(val.dateTime) <= 0;
			}

			return false;
		}

		public static Variant operator -(Variant v1, Variant v2)
		{
			Variant retVal;
			if (v1.type == VariantType.Integer)
			{
				if (v2.type == VariantType.Integer)
				{
					retVal = new Variant(v1.intValue - v2.intValue, v1.length);
					return retVal;
				}
				if (v2.type == VariantType.Number)
				{
					retVal = new Variant(v1.doubleValue - v2.doubleValue, v1.length, v1.decimals);
					return retVal;
				}
			}

			if (v1.type == VariantType.Number)
			{
				if (v2.type == VariantType.Integer)
				{
					retVal = new Variant(v1.doubleValue - v2.intValue, v1.length, v1.decimals);
					return retVal;
				}
				if (v2.type == VariantType.Number)
				{
					retVal = new Variant(v1.doubleValue - v2.doubleValue, v1.length, v1.decimals);
					return retVal;
				}
			}

			if (v1.type == VariantType.DateTime)
			{
				if (v2.type == VariantType.Number || v2.type == VariantType.Integer)
				{
					retVal = new Variant(v1.dateTime.AddSeconds(-Math.Round((Double)v2, 0)));
					return retVal;
				}
				if (v2.type == VariantType.DateTime || v2.type == VariantType.Date)
				{
					TimeSpan tp = v1.dateTime.Subtract(v2.dateTime);
					retVal = new Variant(MathHelper.RoundToInt(tp.TotalMilliseconds / 1000.0), 11);
					return retVal;
				}
			}

			if (v1.type == VariantType.Date)
			{
				if (v2.type == VariantType.Number || v2.type == VariantType.Integer)
				{
					retVal = new Variant(v1.dateTime.AddDays(-(Double)v2), VariantType.Date);
					return retVal;
				}
				if (v2.type == VariantType.DateTime || v2.type == VariantType.Date)
				{
					TimeSpan tp = v1.dateTime.Subtract(v2.dateTime);
					retVal = new Variant(MathHelper.RoundToInt(tp.TotalMilliseconds / 1000.0), 11);
					return retVal;
				}
			}

			if (v1.type == VariantType.Character && v2.type == VariantType.Character)
			{
				String base1 = ((String) v1).TrimEnd();
				return new Variant(base1 + v2 + "".PadRight(v1.Length() - base1.Length));
			}

			throw new ErrorException(ErrorCodes.Syntax);

		}

		public static Variant operator /(Variant v1, Variant v2)
		{
			Variant retVal;
			if (v1.type == VariantType.Integer)
			{
				if (v2.type == VariantType.Integer)
				{
					//TODO: Längenermittlung paßt so noch nicht
					retVal = new Variant(v1.intValue / v2.intValue, v2.length + v2.length);
					return retVal;
				}
				if (v2.type == VariantType.Number)
				{
					retVal = new Variant(v1.intValue / v2.doubleValue, v2.length + v2.length, v2.decimals + 3);
					return retVal;
				}
			}

			if (v1.type == VariantType.Number)
			{
				if (v2.type == VariantType.Integer)
				{
					//TODO: Längenermittlung paßt so noch nicht
					retVal = new Variant(v1.doubleValue / v2.intValue, v2.length + v2.length, v2.decimals + 3);
					return retVal;
				}
				if (v2.type == VariantType.Number)
				{
					retVal = new Variant(v1.doubleValue / v2.doubleValue, v2.length + v2.length, v2.decimals + 3);
					return retVal;
				}
			}

			throw new ErrorException(ErrorCodes.Syntax);

		}


		public static Variant operator *(Variant v1, Variant v2)
		{
			Variant retVal;
			if (v1.type == VariantType.Integer)
			{
				if (v2.type == VariantType.Integer)
				{
					retVal = new Variant(v1.intValue * v2.intValue, v2.length + v2.length);
					return retVal;
				}
				if (v2.type == VariantType.Number)
				{
					retVal = new Variant(v1.intValue * v2.doubleValue, v2.length + v2.length, v2.decimals);
					return retVal;
				}
			}

			if (v1.type == VariantType.Number)
			{
				if (v2.type == VariantType.Integer)
				{
					retVal = new Variant(v1.doubleValue * v2.intValue, v2.length + v2.length, v1.decimals);
					return retVal;
				}
				if (v2.type == VariantType.Number)
				{
					retVal = new Variant(v1.doubleValue * v2.doubleValue, v2.length + v2.length, v1.decimals + v2.decimals);
					return retVal;
				}
			}

			throw new ErrorException(ErrorCodes.Syntax);

		}


		public static Variant operator ^(Variant v1, Variant v2)
		{
			Variant retVal;
			if (v1.type == VariantType.Integer)
			{
				if (v2.type == VariantType.Integer)
				{
					retVal = new Variant((int)Math.Pow(v2.intValue, v1.intValue), v2.length + v2.length);
					return retVal;
				}
				if (v2.type == VariantType.Number)
				{
					retVal = new Variant(Math.Pow(v2.doubleValue, v1.intValue), v2.length + v2.length, v2.decimals);
					return retVal;
				}
			}

			if (v1.type == VariantType.Number)
			{
				if (v2.type == VariantType.Integer)
				{
					retVal = new Variant(Math.Pow(v1.doubleValue, v2.intValue), v1.length + v1.length, v1.decimals);
					return retVal;
				}
				if (v2.type == VariantType.Number)
				{
					retVal = new Variant(Math.Pow(v1.doubleValue, v2.doubleValue), v2.length + v2.length, v2.decimals);
					return retVal;
				}
			}

			throw new ErrorException(ErrorCodes.Syntax);

		}
		public static Variant operator %(Variant v1, Variant v2)
		{

			// Here we have a minor difference with VFP. 11%7.5 is 3, whereas in VFP it's 4. 
			// We treat 7.5 as 8, VFP as 7.
			Variant retVal;
			if (v1.type == VariantType.Integer)
			{
				if (v2.type == VariantType.Integer)
				{
					retVal = new Variant(v1.intValue % v2.intValue, v1.length > v2.length ? v1.length : v2.length);
					return retVal;
				}
				if (v2.type == VariantType.Number)
				{
					double roundedValue = Math.Round(v2.doubleValue, 0);
					retVal = new Variant(v1.intValue % ((int)roundedValue), v1.length > v2.length ? v1.length : v2.length);
					return retVal;
				}
			}

			if (v1.type == VariantType.Number)
			{
				if (v2.type == VariantType.Integer)
				{
					retVal = new Variant(v1.doubleValue % v2.intValue, v1.length > v2.length ? v1.length : v2.length, v1.decimals);
					return retVal;
				}
				if (v2.type == VariantType.Number)
				{
					double roundedValue = Math.Round(v2.doubleValue, 0);
					retVal = new Variant(v1.doubleValue % ((int)roundedValue), v1.length > v2.length ? v1.length : v2.length, v1.decimals);
					return retVal;
				}
			}

			throw new ErrorException(ErrorCodes.InvalidArgument);

		}



		public Variant OperatorGreaterThan(Variant val)
		{
			var retVal = new Variant(IsGreaterThan(val));
			return retVal;
		}

		public Variant OperatorGreaterOrEqualThan(Variant val)
		{
			var retVal = new Variant(IsGreaterOrEqualThan(val));
			return retVal;
		}

		#endregion Operators

		#region Convertors

		public static implicit operator DateTime(Variant value)
		{
			if (value.Type == VariantType.DateTime || value.Type == VariantType.Date)
				return value.dateTime;
			throw new ErrorException(ErrorCodes.DataTypeInvalid);
		}

		public static implicit operator Int32(Variant value)
		{
			switch (value.type)
			{
				case VariantType.Integer:
					return value.intValue;

				case VariantType.Number:
					return (Int32)value.doubleValue;

				case VariantType.Logical:
					if (value.boolValue)
					{ return 1; }
					return 0;

				default:
					return 0;
			}
		}

		public static implicit operator Double(Variant value)
		{
			switch (value.type)
			{
				case VariantType.Number:
					return value.doubleValue;

				case VariantType.Integer:
					return value.intValue;

				case VariantType.Logical:
					if (value.boolValue)
					{ return 1; }
					return 0;

				default:
					return 0;

			}
		}

		public static implicit operator Boolean(Variant value)
		{
			switch (value.type)
			{
				case VariantType.Integer:
					return value.intValue != 0;

				case VariantType.Logical:
					return value.boolValue;

				default:
					throw new ErrorException(ErrorCodes.DataTypeMismatch);
			}
		}

		public static implicit operator ObjectBase(Variant value)
		{
			switch (value.type)
			{
				case VariantType.Object:
					return value.objectValue;

				default:
					return null;
			}
		}

		public override string ToString()
		{
			return this;
		}

		public static implicit operator String(Variant value)
		{
			if (value.type == VariantType.Character)
				return value.strValue.ToString();
			return value.ToString(GuineuInstance.Context);
		}

		public Object ToNative()
		{
			return nativeValue;
		}

		public Byte[] GetBytes()
		{
			String s = strValue.ToString();
			Byte[] data = GuineuInstance.CurrentCp.GetBytes(s);

			return data;
		}

		public string ToString(ExecutionPath ex)
		{
			if (isNull)
				return GuineuInstance.Set.NullDisplay.Value;
			switch (type)
			{
				case VariantType.Integer:
					// TODO: Deal with the formatting that ? and ?? use.
					return intValue.ToString();

				case VariantType.Logical:
					if (boolValue)
						return ".T.";
					return ".F.";

				case VariantType.Character:
					return strValue.ToString();

				case VariantType.Number:
					return doubleValue.ToString(GuineuInstance.Set.CurrentCulture);

				case VariantType.Date:
					if (dateTime.Ticks == 0)
					{
						const string dateSep = ".";
						return "  " + dateSep + "  " + dateSep + "  "; // this string depends on SET MARK TO and SET CENTURY
					}

					return dateTime.ToString("d");

				case VariantType.DateTime:
					if (dateTime.Ticks == 0)
					{
						var di = new System.Globalization.DateTimeFormatInfo();
						const string dateSep = ".";
						const string hourSep = ":";
						string dayMarker = di.AMDesignator;
						return "  " + dateSep + "  " + dateSep + "  " + " " + hourSep + "  " + hourSep + "  " + " " + dayMarker; // this string depend of SET MARK TO and SET CENTURY
					}
					return dateTime.ToString("G");

				default:
					throw new InvalidOperationException(Resources.Value_Is_Undefined);
			}
		}

		internal string ToName(ExecutionPath ex)
		{
			return ToString(ex);
			// (...) checked typeof. Must be a string of some sort. String must return a valid Name
		}

		internal Nti ToNti(ExecutionPath ex)
		{
			String s = ToString(ex);
			return new Nti(s);
		}

		#endregion Convertors

		#region Informational functions

		public Int32 Decimals
		{
			get { return decimals; }
		}
		#endregion

		public static Variant Create(object o)
		{
			if (o is ObjectBase)
			{
				return new Variant((ObjectBase)o);
			}

			if (o is String)
				return new Variant((String)o);
			if (o is Int32)
				return new Variant((Int32)o, 10);
			if (o is Byte)
				return new Variant((Byte)o, 10);
			if (o is DateTime)
				return new Variant((DateTime)o, VariantType.DateTime);
			if (o is Boolean)
				return new Variant((Boolean)o);
			if (o is Double)
				return new Variant((Double)o, 10, 2);
			if (o is decimal)
				return new Variant((Double)(Decimal)o, 10, 2);
			if (o is Enum)
			{
				var e = o as Enum;
				if (e.GetTypeCode() == TypeCode.Byte)
					return new Variant((Byte)o, 10);
				return new Variant((Int32)o, 10);
			}
			if(o == null)
				return new Variant(VariantType.Logical, true);

			return new Variant(new ControlClass(o));
		}

		public object ToType(Type target)
		{
			if (target == typeof(String))
				return (String)this;
			if (target == typeof(Int32))
				return (Int32)this;
			if (target == typeof(DateTime))
				return (DateTime)this;
			if (target == typeof(Boolean))
				return (Boolean)this;
			if (target == typeof(Double))
				return (Double)this;
			if (target.IsEnum)
				return CastBoxedValue((Int32)this, target);
			return this;
		}

		public Object ToValue()
		{
			switch (Type)
			{
				case VariantType.Integer:
					return (Int32) this;
				case VariantType.Logical:
					return (Boolean) this;
				case VariantType.Character:
					return (String) this;
				case VariantType.Number:
					return (Double) this;
				case VariantType.Object:
					if (objectValue is ControlClass)
						return ((ControlClass) objectValue).Native;
					return (ObjectBase) this;
				case VariantType.Date:
					return (DateTime)this;
				case VariantType.DateTime:
					return (DateTime) this;
				case VariantType.Null:
					return null;
			}
			return ToNative();
		}

		// http://stackoverflow.com/questions/2660194/converting-an-integer-to-a-boxed-enum-type-only-known-at-runtime
		static object CastBoxedValue(object value, Type destType)
		{
			if (value == null)
				return value;

			Type enumType = GetEnumType(destType);
			if (enumType != null)
				return Enum.ToObject(enumType, value);

			return value;
		}

		private static Type GetEnumType(Type type)
		{
			if (type.IsEnum)
				return type;

			if (type.IsGenericType)
			{
				var genericDef = type.GetGenericTypeDefinition();
				if (genericDef == typeof(Nullable<>))
				{
					var genericArgs = type.GetGenericArguments();
					return (genericArgs[0].IsEnum) ? genericArgs[0] : null;
				}
			}
			return null;
		}
	}

	static class StringBuilderHelper
	{
		/// <summary>
		/// Compares two StringBuilders.
		/// </summary>
		/// <param name="strA"></param>
		/// <param name="strB"></param>
		/// <returns></returns>
		public static int Compare(StringBuilder strA, StringBuilder strB)
		{
			// TODO : Set Exact
			// parameter because eg ascan can overide the set exact setting
			// the other way could be to return another value than 1 (enum?)
			// if strA.Length!=strB.Length and let the caller do what he wants to
			// or to implemente a CompareNotExact ... Chris, i let you decide
			for (int ch = 0; ch < strA.Length; ch++)
			{
				if (ch >= strB.Length)
					return 1;
				char chrA = strA[ch];
				char chrB = strB[ch];
				if (chrA < chrB)
					return -1;
				if (chrA > chrB)
					return 1;
			}
			if (strA.Length == strB.Length) // <= (less or equal than) for set exact off ...
				return 0;
			return 1;
		}
	}

}