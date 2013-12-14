using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu.Data
{
	interface IRecordIterator
	{
		void Next(CallingContext ctx);
		Boolean HasMore(CallingContext ctx);
	}

	interface IIteratorCondtionChecker
	{
		void MoveToFirst(CallingContext ctx);
		Boolean Next(CallingContext ctx);
		Boolean IsValid(CallingContext ctx);
		Boolean HasMore(CallingContext ctx);
	}
}
