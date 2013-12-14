namespace Guineu.ObjectEngine
{
	class MethodImplementation : MethodMember
	{
		internal CodeBlock Code;

		internal MethodImplementation(CodeBlock code, ObjectBase obj)
			: base(obj)
		{
			Code = code;
		}

		internal MethodImplementation(CodeBlock code)
			: base(null)
		{
			Code = code;
		}

		internal override Member Clone()
		{
			var newMember = new MethodImplementation(Code, Object);
			return newMember;
		}

		internal override Variant ExecuteNative(CallingContext cc, ParameterCollection param)
		{
			var retVal = new Variant(true);
			if (Code != null)
				retVal = cc.Context.ExecuteInNewContext(Code, param, Object);
			
			return retVal;
		}



	}
}
