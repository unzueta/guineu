using System;
using Guineu.Expression;

namespace Guineu.Data.Dbf
{
	interface INavigator
	{
		void GoBottom();
		void GoTo(long recNo);
		void GoTop();
		void GoEof();
		bool IsBof { get; }
		bool IsEof { get; }
		long RecordNumber { get; }
		void SkipBackward();
		void SkipForward();
		void SetFilter(ExpressionBase filter);
	}
}
