using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	partial class ListInteractiveChangeMethod
	{
		void CallClickMethod()
		{
			var mth = (ClickMethod)Object.GetMember(KnownNti.Click);
			mth.Execute(new ParameterCollection());
		}
	}
}
