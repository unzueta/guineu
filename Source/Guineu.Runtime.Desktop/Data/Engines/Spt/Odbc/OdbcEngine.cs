using System;
using System.Data.Odbc;

namespace Guineu.Data.Engines.Spt.Odbc
{
	class OdbcEngine : ISptEngine
	{
		#region ISptEngine Members

		public int Exec(int handle, String command, String alias, CallingContext context)
		{
			var cmd = new OdbcCommand(command, GetConnection(handle));
			try
			{
				return SQLEXEC.ReadResultSets(context,alias, cmd.ExecuteReader());
			}
			catch (OdbcException)
			{
				return -1;
			}
		}

		public SptConnection StringConnect(string connStr)
		{
			var cn = new OdbcSptConnection(new OdbcConnection(connStr));
			try
			{
				cn.Connection.Open();
			}
			catch (OdbcException)
			{
				// Cannot connect to server
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
			OdbcConnection conn = GetConnection( handle);
			conn.Close();
			return 1;
		}

		public String Name
		{
			get { return "odbc"; }
		}

		#endregion

		private OdbcConnection GetConnection(Int32 handle)
		{
			if (!GuineuInstance.Connections.IsValid(handle))
			{
				throw new ErrorException(ErrorCodes.ConnectionHandleInvalid);
			}
			var conn = (OdbcSptConnection) GuineuInstance.Connections[handle];
			if (conn == null)
			{
				throw new ErrorException(ErrorCodes.ConnectionHandleInvalid);
			}
			return conn.Connection;
		}
	}
	
	class OdbcSptConnection : SptConnection
	{
		private readonly OdbcConnection connection;

		internal OdbcSptConnection(OdbcConnection conn)
		{
			connection = conn;
		}

		public OdbcConnection Connection
		{
			get { return connection; }
		}
	}
}
