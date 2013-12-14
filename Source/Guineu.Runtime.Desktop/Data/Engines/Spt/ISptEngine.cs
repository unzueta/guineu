using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu.Data
{
	/// <summary>
	/// Access a database using Guineu's SQL Pass-Through functions
	/// </summary>
	public interface ISptEngine
	{
		Int32 Exec(
			Int32 handle,
			String command,
			String alias,
			CallingContext context
		);

		SptConnection StringConnect(String conn);

		Int32 Disconnect(Int32 handle);

		String Name { get;}
	}
}
