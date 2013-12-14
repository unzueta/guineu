using System;
using System.Text;
using System.IO;
using Guineu.Data.Dbf;
using Guineu.Util;

namespace Guineu.Data.Engines.Dbf.Index
{
	/// <summary>
	/// A single index entry representing a specific value in a specific record.
	/// </summary>
	sealed public class KeyItem
	{
		#region public construction methods

		public KeyItem(Boolean isNullable, Encoding usedEncoding, Int32 length)
		{
			indexSupportsNull = isNullable;
			indexEncoding = usedEncoding;
			keyLength = length;
		}

		public KeyItem New(String val)
		{
			Byte[] convertedValue = StringToByte(val);
			return new KeyItem(convertedValue);
		}
		public KeyItem New(DateTime val)
		{
			Byte[] convertedValue = DateTimeToByte(val);
			return new KeyItem(convertedValue);
		}
		public KeyItem New(Double val)
		{
			Byte[] convertedValue = DoubleToByte(val);
			return new KeyItem(convertedValue);
		}
		public KeyItem New(Int32 value)
		{
			Byte[] convertedValue = Int32ToByte(value);
			return new KeyItem(convertedValue);
		}
		public KeyItem New(Variant value)
		{
			switch (value.Type)
			{
				case VariantType.Character:
					return New((String) value);
				case VariantType.Integer:
				case VariantType.Number:
					if (keyLength == 4)
						return New((Int32) value);
					else
						return New((Double) value);
				case VariantType.Date:
				case VariantType.DateTime:
					return New((DateTime) value);
				default:
					return null;
			}
		}
		
		public KeyItem New(String value, Int64 recNo)
		{
			Byte[] convertedValue = StringToByte(value);
			return new KeyItem(convertedValue, recNo);
		}
		public KeyItem New(DateTime value, Int64 recNo)
		{
			Byte[] convertedValue = DateTimeToByte(value);
			return new KeyItem(convertedValue, recNo);
		}
		public KeyItem New(Double value, Int64 recNo)
		{
			Byte[] convertedValue = DoubleToByte(value);
			return new KeyItem(convertedValue, recNo);
		}
		public KeyItem New(Int32 value, Int64 recNo)
		{
			Byte[] convertedValue = Int32ToByte(value);
			return new KeyItem(convertedValue,recNo);
		}
		public KeyItem New(Variant value,Int64 recNo)
		{
			switch (value.Type)
			{
				case VariantType.Character:
					return New((String) value,recNo);
				case VariantType.Integer:
				case VariantType.Number:
					if (keyLength == 4)
						return New((Int32) value,recNo);
					return New((Double) value,recNo);
				case VariantType.Date:
				case VariantType.DateTime:
					return New((DateTime) value,recNo);
				default:
					return null;
			}
		}
		
		public KeyItem New(IndexNodeExterior node, Int32 index)
		{
			if (index < 0)
				return New();
			return new KeyItem(node.GetKey(index), node.GetRecordNumber(index));
		}

		public KeyItem New()
		{
			return new KeyItem(new Byte[0]);
		}

		#endregion

		#region Information retrieval

		/// <summary>
		/// Returns the key value as a byte array. The byte array should not be modified.
		/// </summary>
		/// <returns></returns>
		// TODO: Replace with a function that takes a byte array and returns true
		//       if this is a match.
		internal Byte[] GetBytes()
		{
			return value;
		}
		public Int64 GetRecord()
		{
			return record;
		}
		#endregion

		#region operations
		public Boolean Matches(KeyItem otherItem)
		{
			Boolean retVal = Matches(otherItem, true);
			return retVal;
		}
		/// <summary>
		/// Determines wether the key value matches the one in another item. 
		/// The record number is ignored.
		/// </summary>
		/// <param name="otherItem">A KeyItem which is compared with this instance</param>
		/// <param name="exact">
		/// true: Both values must match exactly.
		/// false: Compare up to the length of the shorter value. \0 at the end is ignored.
		/// </param>
		/// <returns></returns>
		public Boolean Matches(KeyItem otherItem, Boolean exact)
		{
			Boolean retVal = (BufferUtil.Compare(value, otherItem.value, exact) == 0);
			return retVal;
		}

		#endregion

		#region private Constructors
		KeyItem(Byte[] newValue, Int64 newRecord)
		{
			value = newValue;
			record = newRecord;
		}
		KeyItem(Byte[] newValue)
			: this(newValue, 0)
		{
		}

		#endregion
		#region private methods
		Byte[] StringToByte(String value)
		{
			var buf = new MemoryStream();
			using (var bw = new BinaryWriter(buf))
			{
				// If the index supports NULL values we need to add "_" for 
				// any value that is not .NULL.
				if ((value != null) && indexSupportsNull)
				{
					bw.Write((Byte)0x80);
				}

				// find the last character that is not a blank. Blanks at the end of
				// a key are automatically ignored.
				Int32 length = value.Length;
				while ((length > 0) && (value[length - 1] == ' '))
					length--;

				// Convert all remaining characters to bytes
				if (length > 0)
					bw.Write(indexEncoding.GetBytes(value.Substring(0, length)));

				// TODO: convert strings according to index collate
				// TOFO: Check if string needs to be converted according to current CP
				//       or index's CP.
				return buf.ToArray();
			}
		}
		/// <summary>
		/// Returns an 8 byte array representing a DateTime value in an index. 
		/// </summary>
		/// <param Name="value"></param>
		/// <returns></returns>
		Byte[] DateTimeToByte(DateTime value)
		{
			Int32 iDate = Util.Date.ToJulian(value);
			Byte[] bdata = DoubleToByte(iDate);
			return bdata;
		}
		/// <summary>
		/// Returns an 8-byte array representing a double value in an index.
		/// </summary>
		/// <param Name="value"></param>
		/// <returns></returns>
		Byte[] DoubleToByte(Double value)
		{
			Byte[] bdata = new Byte[8];
			BinaryWriter br = new BinaryWriter(new MemoryStream(bdata));
			br.Write(-1 * value);
			Byte[] newData = new Byte[8];
			for (Int32 t = 0; t < 8; t++)
				newData[t] = bdata[7 - t];
			return newData;
		}
		/// <summary>
		/// Returns a 4-byte array representing an Int32 value in an index.
		/// </summary>
		/// <param Name="value"></param>
		/// <returns></returns>
		Byte[] Int32ToByte(Int32 value)
		{
			Byte[] bdata = new Byte[4];
			BinaryWriter br = new BinaryWriter(new MemoryStream(bdata));
			br.Write(value);
			Byte[] newData = new Byte[4];
			for (Int32 t = 0; t < 4; t++)
				newData[t] = bdata[3 - t];
			newData[0] ^= 0x80;
			return newData;
		}
		#endregion
		#region private members
		Byte[] value;
		Int64 record;
		Boolean indexSupportsNull;
		Encoding indexEncoding;
		Int32 keyLength;
		#endregion
	}
}
