using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Guineu.Util
{
	public static class StreamUtil
	{
		static public Byte[] Read(Stream stream, Int32 count)
		{
			Int32 Offset = 0;
			Int32 BytesRead;
			Byte[] buffer = new Byte[count];
			do
			{
				try
				{
					BytesRead = stream.Read(buffer, Offset, count - Offset);
				}
				catch (IOException)
				{
					throw new ErrorException(ErrorCodes.ErrorReadingFile);
				}
				Offset = Offset + BytesRead;
			} while (Offset < count);
			return buffer;
		}
		static public Byte[] ReadBytes(this Stream stream, Int32 count)
		{
			return StreamUtil.Read(stream, count);
		}
	}

}
