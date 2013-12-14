using System;
using Guineu.Expression;
using Guineu.Classes;

namespace Guineu.Functions
{
	/// <summary>
	/// Enables additional base classes
	/// </summary>
	 partial class SYS8008
	{
		public static Boolean RegisterBaseClass(Nti n)
		{
			if (GuineuInstance.ObjectFactory.IsRegistered(n))
				return false;
			
			switch (n.ToKnownNti())
			{
				case KnownNti.Signature:
					GuineuInstance.ObjectFactory.RegisterClass(new SignatureClassTemplate(n));
					break;
			}
			return true;
		}
	}
}