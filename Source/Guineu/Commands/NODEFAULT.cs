namespace Guineu
{
	class NODEFAULT : ICommand
	{
		public void Compile(CodeBlock code)	{}

		public void Do(CallingContext ctx, ref int nextLine)
		{
			var ev = ctx.Context.GetEvent();
			if (ev != null)
				ev.ExecuteNativeBehavior = false;
		}
	}
}
