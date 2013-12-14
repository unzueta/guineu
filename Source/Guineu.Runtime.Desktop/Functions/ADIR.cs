using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;
using Guineu.Util;

namespace Guineu
{
	/// <summary>
	/// ADIR()
	/// </summary>
	partial class ADIR
	{
		WriteArray array;
		ExpressionBase fileSkeleton;
		ExpressionBase attribute;
		ExpressionBase flags;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					array = (WriteArray)param[0];
					break;
				case 2:
					array = (WriteArray)param[0];
					fileSkeleton = param[1];
					break;
				case 3:
					array = (WriteArray)param[0];
					fileSkeleton = param[1];
					attribute = param[2];
					break;
				case 4:
					array = (WriteArray)param[0];
					fileSkeleton = param[1];
					attribute = param[2];
					flags = param[3];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			// Get the path
			String pattern;
			if (fileSkeleton == null)
			{
				pattern = "*.*";
			}
			else
			{
				pattern = fileSkeleton.GetString(context);
			}

			String filter;
			if (attribute == null)
				filter = "";
			else
				filter = attribute.GetString(context).ToUpper(System.Globalization.CultureInfo.InvariantCulture);

			string currentDirectory = GetCurrentDirectory();
			String path = Path.Combine(currentDirectory, pattern);

			// Get a list of all files
			var di = new DirectoryInfo(Path.GetDirectoryName(path));
			FileSystemInfo[] fi;
			if (filter.IndexOf("D") >= 0)
				fi = di.GetFileSystemInfos(Path.GetFileName(path));
			else
				fi = di.GetFiles(Path.GetFileName(path));

			FileSystemInfo curFile;
			Int32 totalFiles = fi.Length;
			for (Int32 item = 0; item < fi.Length; item++)
			{
				curFile = fi[item];
				if (EnumUtil.IsSet((Int32)curFile.Attributes, (Int32)FileAttributes.Hidden))
					if (filter.IndexOf("H") < 0)
					{
						totalFiles--;
						fi[item] = null;
						continue;
					}
				if (EnumUtil.IsSet((Int32)curFile.Attributes, (Int32)FileAttributes.System))
					if (filter.IndexOf("S") < 0)
					{
						totalFiles--;
						fi[item] = null;
						continue;
					}
			}

			ArrayMember arr = array.GetArray(context);
			if (arr == null)
			{
				throw new ErrorException(ErrorCodes.NotAnArray, array.GetName(context).ToUpper(System.Globalization.CultureInfo.InvariantCulture));
			}

			// Copy file info into array
			ChangeArraySize(arr, totalFiles);

			FileInfo info;
			Int32 pos = 0;
			for (int i = 1; i <= fi.Length; i++)
			{
				if (fi[i - 1] != null)
				{
					pos++;
					arr.Locate(pos, 1).SetString(fi[i - 1].Name);
					info = fi[i - 1] as FileInfo;
					if (info != null)
					{
						var dt = info.LastWriteTime;
						var delta = TimeZone.CurrentTimeZone.GetDaylightChanges(dt.Year).Delta;
						if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(dt))
						{
							if (!TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now))
								dt = dt.Subtract(delta);
						}
						else if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now))
							dt = dt.Add(delta);
						arr.Locate(pos, 2).Set(new Variant((Int32)info.Length, 10));
						arr.Locate(pos, 3).Set(new Variant(dt, VariantType.Date));
						arr.Locate(pos, 4).SetString(dt.ToString("HH:mm:ss"));
						arr.Locate(pos, 5).SetString(GetAttributeString(info.Attributes));
					}
					else
					{
										var dir = fi[i - 1] as DirectoryInfo;
					if (dir != null)
					{
						var dt = dir.LastWriteTime;
						var delta = TimeZone.CurrentTimeZone.GetDaylightChanges(dt.Year).Delta;
						if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(dt))
						{
							if (!TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now))
								dt = dt.Subtract(delta);
						}
						else if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now))
							dt = dt.Add(delta);
						arr.Locate(pos, 2).Set(new Variant(0, 10));
						arr.Locate(pos, 3).Set(new Variant(dt, VariantType.Date));
						arr.Locate(pos, 4).SetString(dt.ToString("HH:mm:ss"));
						arr.Locate(pos, 5).SetString(GetAttributeString(dir.Attributes));
					}
				}
				}
			}
			return new Variant(pos, 10);
		}

		static string GetAttributeString(FileAttributes att)
		{
			var s =
				(Enum<FileAttributes>.IsSet(att, FileAttributes.ReadOnly) ? "R" : ".") +
				(Enum<FileAttributes>.IsSet(att, FileAttributes.Archive) ? "A" : ".") +
				(Enum<FileAttributes>.IsSet(att, FileAttributes.System) ? "S" : ".") +
				(Enum<FileAttributes>.IsSet(att, FileAttributes.Hidden) ? "H" : ".") +
				(Enum<FileAttributes>.IsSet(att, FileAttributes.Directory) ? "D" : ".");
			return s;
		}
	}

}
