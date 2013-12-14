using System;
using System.Collections.Generic;
using System.Data;
using Guineu.Data.Engines.Cursor;
using Guineu.Expression;
using Guineu.Data;

namespace Guineu
{
	class SQLEXEC : ExpressionBase
	{
		ExpressionBase handle;
		ExpressionBase command;
		ExpressionBase alias;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					handle = param[0];
					command = param[1];
					break;
				case 3:
					handle = param[0];
					command = param[1];
					alias = param[2];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetInt(context), 10);
			return retVal;
		}

		internal override int GetInt(CallingContext context)
		{
			Int32 handleValue = handle.GetInt(context);
			String result;
			if (alias == null)
				result = "SQLRESULT";
			else
				result = alias.GetString(context);

			var area = context.DataSession.Select(new Nti(result));
			if(area != 0)
				context.DataSession.Close(area);

			ISptEngine engine = GuineuInstance.Connections[handleValue].Engine;
			return engine.Exec(handleValue, command.GetString(context), result, context);
		}

		public static Int32 ReadResultSets(CallingContext context, String alias, IDataReader reader)
		{
			Int32 cnt = 0;
			do
			{
				cnt++;
				ReadResultSet(context, alias, reader);
			} while (reader.NextResult());
			return cnt;
		}

		private static void ReadResultSet(CallingContext context, String alias, IDataReader reader)
		{
			var tbl = new DataTable();
			var val = new object[reader.FieldCount];

			for (Int32 nCol = 0; nCol < reader.FieldCount; nCol++)
			{
				var col = new DataColumn
				          	{
				          		DataType = reader.GetFieldType(nCol),
				          		ColumnName = reader.GetName(nCol).Replace(' ', '_')
				          	};
				tbl.Columns.Add(col);
			}

			while (reader.Read())
			{
				reader.GetValues(val);
				tbl.Rows.Add(val);
			}
			tbl.AcceptChanges();

			var result = new Cursor(alias, tbl);
			context.DataSession.Add(result,context.DataSession.GetNextFreeWorkarea());

		}

	}
}