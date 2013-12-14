using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Common;
using System.Text;

namespace Guineu.Data
{
	public class ContextConnectionEngine : ISptEngine
	{
		#region ISptEngine Members

		public int Exec(int handle, String command, String alias, CallingContext context)
		{
			SqlCommand cmd = new SqlCommand(command, GetConnection(handle));
			try
			{
				return SQLEXEC.ReadResultSets(context, alias, cmd.ExecuteReader());
			}
			catch (System.Data.SqlClient.SqlException)
			{
				return -1;
			}
		}

		public SptConnection StringConnect(string connStr)
		{
			ContextSptConnection cn = new ContextSptConnection(new SqlConnection("context connection=true"));
			try
			{
				cn.Connection.Open();
			}
			catch (System.Data.SqlClient.SqlException)
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
			SqlConnection conn = GetConnection(handle);
			conn.Close();
			return 1;
		}

		public String Name
		{
			get { return "context"; }
		}

		#endregion

		private SqlConnection GetConnection(Int32 handle)
		{
			if (!GuineuInstance.Connections.IsValid(handle))
			{
				throw new ErrorException(ErrorCodes.ConnectionHandleInvalid);
			}
			ContextSptConnection conn = (ContextSptConnection)GuineuInstance.Connections[handle];
			if (conn == null)
			{
				throw new ErrorException(ErrorCodes.ConnectionHandleInvalid);
			}
			return conn.Connection;
		}
	}


	class ContextSptConnection : SptConnection
	{
		private SqlConnection _Connection;

		internal ContextSptConnection(SqlConnection conn)
		{
			_Connection = conn;
		}

		public SqlConnection Connection
		{
			get { return _Connection; }
		}
	}
}
