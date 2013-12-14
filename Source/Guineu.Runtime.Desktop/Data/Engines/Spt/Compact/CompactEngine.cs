using System;
using System.Data.SqlServerCe;
using Guineu.Core;

namespace Guineu.Data
{
	class CompactEngine : ISptEngine
	{
		public int Exec(int handle, String command, String alias, CallingContext context)
		{
			var cmd = new SqlCeCommand(command, GetConnection(handle));
			try
			{
				return SQLEXEC.ReadResultSets(context, alias, cmd.ExecuteReader());
			}
			catch (SqlCeException e)
			{
				ReportErrors(0, e);
				return -1;
			}
		}

		public SptConnection StringConnect(string connStr)
		{
			var cn = new CeSptConnection(new SqlCeConnection(connStr));
			try
			{
				cn.Connection.Open();
			}
			catch (SqlCeInvalidDatabaseFormatException e)
			{
				ReportErrors(0, e);
				return null;
			}
			catch (SqlCeException)
			{
				return null;
			}
			if (cn.Connection.State != System.Data.ConnectionState.Open)
			{
				return null;
			}
			return cn;
		}

		public int Disconnect(int handle)
		{
			SqlCeConnection conn = GetConnection(handle);
			conn.Close();
			return 1;
		}

		static void ReportErrors(int handle, SqlCeException e)
		{
			if (e.Errors.Count == 0)
				return;

			GuineuInstance.Errors.Clear();
			foreach (SqlCeException err in e.Errors)
				GuineuInstance.Errors.Add(new OdbcErrorItem(err.Message, 0, err.NativeError, handle));
		}

		public String Name
		{
			get { return "compact"; }
		}

		private SqlCeConnection GetConnection(Int32 handle)
		{
			if (!GuineuInstance.Connections.IsValid(handle))
			{
				throw new ErrorException(ErrorCodes.ConnectionHandleInvalid);
			}
			var conn = (CeSptConnection)GuineuInstance.Connections[handle];
			if (conn == null)
			{
				throw new ErrorException(ErrorCodes.ConnectionHandleInvalid);
			}
			return conn.Connection;
		}
	}


	class CeSptConnection : SptConnection
	{
		private readonly SqlCeConnection connection;

		internal CeSptConnection(SqlCeConnection conn)
		{
			connection = conn;
		}

		public SqlCeConnection Connection
		{
			get { return connection; }
		}
	}
}
