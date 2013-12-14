using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	public class ObjectBase : IMemberList
	{
		ObjectBase instanceParent;
		protected readonly MemberList Members;

		// TODO: get rid of this property
		Boolean isForm;

		private Nti ntiField;
		public Nti Nti
		{
			get { return ntiField; }
			internal set
			{
				ntiField = value;

				// Register the child control as a member.
				if (instanceParent != null)
				{
					// TODO: Remove a previous registration
					instanceParent.Add(ntiField, new ControlMember(this));
				}
			}
		}

		public ObjectBase()
		{
			Members = new MemberList();
			Controls = new ObjectCollection(this);
		}

		/// <summary>
		/// Creates an instance object based on a given object. 
		/// </summary>
		/// <param name="classObject"></param>
		public ObjectBase(ObjectTemplate classObject)
			: this()
		{
			ClassObject = classObject;
		}

		public void Add(Nti name, Member value)
		{
			Members.Add(name, value);
		}

		public void Set(Nti name, Member value)
		{
			Members.Set(name, value);
		}

		public void AddMember(ValueMember value)
		{
			Members.Add(value.Nti, value);
		}

		public Member GetMember(Nti name)
		{
			return DoGetMember(name);
		}

		protected virtual Member DoGetMember(Nti name)
		{
			return Members.Get(name);
		}
		public Nti GetMemberNameByPosition(int pos)
		{
			return Members.GetMemberNameByPosition(pos);
		}

		public Int32 Count
		{
			get { return Members.Count; }
		}

		public void AddProperty(Nti name, String value)
		{
			Members.AddProperty(name, value);
		}

		public void AddProperty(Nti name, Boolean value)
		{
			Members.AddProperty(name, value);
		}

		public void AddProperty(Nti name, Int32 value)
		{
			Members.AddProperty(name, value);
		}

		public void AddProperty(Nti name, Variant value)
		{
			Members.AddProperty(name, value);
		}

		internal MethodImplementation GetMethod(Nti name)
		{
			return Members.Get(name) as MethodImplementation;
		}

		#region Simple properties

		internal ObjectCollection Controls { get; set; }
		public ObjectTemplate ClassObject { get; set; }

		public ObjectBase Parent
		{
			get { return instanceParent; }
		}
		#endregion

		internal void AssignParent(ObjectBase parent)
		{
			instanceParent = parent;
			DoAssignParent();
		}

		protected virtual void DoAssignParent()
		{
		}

		/// <summary>
		/// Calls the INIT of an object. InitInstance is called form the inside out.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		internal bool InitInstance(ParameterCollection p)
		{
			DoInitializeInstance();
			foreach (ObjectBase obj in Controls)
			{
				obj.InitInstance(new ParameterCollection());
			}
			DoInitializeInstanceAfter(p);
			return true;
		}

		protected virtual void DoInitializeInstanceAfter(ParameterCollection parms)
		{
			ObjectBase obj = GetCodeBase(KnownNti.Init);
			if (obj != null)
			{
				MethodImplementation m = obj.GetMethod(KnownNti.Init);

				using (var ctx = new CallingContext(GuineuInstance.Context, this))
					// TODO: Need to handle a return value of false here and in InitInstance
					ctx.Context.ExecuteInNewContext(m.Code, parms, this);
			}
		}

		protected void AddUserDefinedMembers()
		{
			AddUserDefinedMembers(ClassObject);
		}

		protected void AddUserDefinedMembers(IMemberList members)
		{
			for (int i = 0; i < members.Count; i++)
			{
				var name = members.GetMemberNameByPosition(i);
				if (Members.Get(name) == null)
				{
					Member mbr = members.GetMember(name);
					if (!(mbr is ControlMember))
					{
						Member m = mbr.Clone();
						Add(name, m);

						// TODO: Implement differently. The object has got an empty method.
						//       The Template contains the object code, so that we can implement
						//       DODEFAULT()
						if (m is MethodMember)
						{
							var mm = m as MethodMember;
							mm.Object = this;
						}
					}
				}
			}
		}

		//===================================================================================
		/// <summary>
		/// Insert the code that prepares the object according to the class object
		/// </summary>
		virtual protected void DoInitializeInstance()
		{
		}

		virtual internal ObjectBase GetThisForm()
		{
			if (isForm)
				return this;
			if (instanceParent == null)
				throw new ErrorException(ErrorCodes.ObjectNotContained, "FORM");
			return instanceParent.GetThisForm();
		}

		internal void EnableThisform()
		{
			isForm = true;
		}

		internal ObjectBase GetThisFormSet()
		{
			if (isForm)
				return this;
			if (instanceParent == null)
				throw new ErrorException(ErrorCodes.ObjectNotContained, "FORMSET");
			return instanceParent.GetThisForm();
		}


		public Variant CallMethod(ExecutionPath context, string name, ParameterCollection param)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (name == null)
				throw new ArgumentNullException("name");

			var resolver = new MemberListResolver(this);
			Member member = resolver.Resolve(new Nti(name));
			var method = member as MethodMember;

			var retVal = new Variant(true);
			if (method != null)
				retVal = method.Execute(context.CurrentContext, param);

			return retVal;
		}

		/// <summary>
		/// Returns the inheritance member that contains code
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		internal ObjectBase GetCodeBase(Nti method)
		{
			ObjectBase obj = this;
			ObjectBase found = null;
			while (obj != null)
			{
				MethodImplementation m = obj.GetMethod(method);
				if (m != null)
				{
					if (m.Code != null)
					{
						found = obj;
						break;
					}
				}
				obj = obj.ClassObject;
			}
			return found;
		}

		//===================================================================================

		// TODO: Remove next three methods
		protected String GetPropString(String prop)
		{
			return GetPropString(new Nti(prop));
		}
		protected Int32 GetPropInt32(String prop)
		{
			return GetPropInt32(new Nti(prop));
		}
		protected Boolean GetPropBoolean(String prop)
		{
			return GetPropBoolean(new Nti(prop));
		}

		protected String GetPropString(Nti prop)
		{
			return GetPropVariant(prop);
		}
		protected Int32 GetPropInt32(Nti prop)
		{
			return GetPropVariant(prop);
		}
		protected Boolean GetPropBoolean(Nti prop)
		{
			return GetPropVariant(prop);
		}
		protected Variant GetPropVariant(Nti prop)
		{
			return ((ValueMember)ClassObject.GetMember(prop)).Get();
		}

		internal static ObjectBase GetActualParent(ObjectBase parent, String objName)
		{
			ObjectBase retObj = parent;
			if (objName.IndexOf('.') > 0)
			{
				String[] names = objName.Split(new[] { '.' });
				for (Int32 pos = 0; pos < names.Length - 1; pos++)
				{
					var mbr = retObj.GetMember(new Nti(names[pos])) as ControlMember;
					if (mbr == null)
						throw new ErrorException(ErrorCodes.ObjectNotFound); // unknown member
					retObj = mbr.Get();
				}
			}
			return retObj;
		}

		internal ObjectBase GetActualParent(String objName)
		{
			return GetActualParent(this, objName);
		}



		internal static Nti GetActualName(string objName)
		{
			if (objName.IndexOf('.') > 0)
			{
				String[] names = objName.Split(new[] { '.' });
				return new Nti(names[names.Length - 1]);
			}
			return new Nti(objName);
		}

		protected void CloneMember(Nti name, IMemberList template)
		{
			var value = template.GetMember(name);
			if (value != null)
				Add(name, value.Clone());
		}
	}

	/// <summary>
	/// Provides access to a list of named values.
	/// </summary>
	public interface IMemberList
	{
		void Add(Nti name, Member value);
		void Set(Nti name, Member value);
		Member GetMember(Nti name);
		Nti GetMemberNameByPosition(int pos);
		Int32 Count { get; }
	}
}
