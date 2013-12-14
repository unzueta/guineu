using System;
using System.Data.SqlClient;
using Guineu.Core;

namespace Guineu.Data.Engines.Spt.mssql
{
	class MssqlEngine : ISptEngine
	{
		public int Exec(int handle, String command, String alias, CallingContext context)
		{
			var cmd = new SqlCommand(command, GetConnection(handle));
			try
			{
				using (var reader = cmd.ExecuteReader())
					return SQLEXEC.ReadResultSets(context, alias, reader);
			}
			catch (SqlException e)
			{
				ReportErrors(handle, e);
				return -1;
			}
		}

		static void ReportErrors(int handle, SqlException e)
		{
			if(e.Errors.Count == 0)
				return;

			GuineuInstance.Errors.Clear();
			foreach (SqlError err in e.Errors)
				GuineuInstance.Errors.Add(new OdbcErrorItem(err.Message, err.State, err.Number, handle));
		}

		public SptConnection StringConnect(string connStr)
		{
			var cn = new SqlSptConnection(new SqlConnection(connStr));
			try
			{
				cn.Connection.Open();
			}
			catch (SqlException e)
			{
				ReportErrors(0, e);
				return null;
			}
			if (cn.Connection.State != System.Data.ConnectionState.Open)
				return null;
			return cn;
		}
	
		public int Disconnect(int handle)
		{
			SqlConnection conn = GetConnection( handle);
			conn.Close();
			return 1;
		}

		public String Name
		{
			get { return "odbc"; }
		}

		private static SqlConnection GetConnection(Int32 handle)
		{
			if (!GuineuInstance.Connections.IsValid(handle))
				throw new ErrorException(ErrorCodes.ConnectionHandleInvalid);
			
			var conn = (SqlSptConnection) GuineuInstance.Connections[handle];
			if (conn == null)
				throw new ErrorException(ErrorCodes.ConnectionHandleInvalid);
			
			return conn.Connection;
		}
	}

	
	class SqlSptConnection : SptConnection
	{
		private readonly SqlConnection connection;

		internal SqlSptConnection(SqlConnection conn)
		{
			connection = conn;
		}

		public SqlConnection Connection
		{
			get { return connection; }
		}
	}
}
