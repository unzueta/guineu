using System;
using System.Reflection;
using Guineu.Expression;
using Guineu.Gui;
using Guineu.ObjectEngine;

namespace Guineu.Classes
{
	/// <summary>
	/// A new base class "Control" that is used to wrap any silverlight object
	/// </summary>
	public partial class ControlClass : ObjectBase, IControl
	{
		private readonly object control;

		public ControlClass(object ctrl) : this(null, ctrl){}

		public ControlClass(ObjectTemplate obj, object ctrl):base(obj)
		{
			control = ctrl;
			DoAddControl();
		}

		partial void DoAddControl();

		public object Native { get { return control; } }

		protected override void DoInitializeInstance()
		{
			base.DoInitializeInstance();
			AddUserDefinedMembers();
		}

		protected override Member DoGetMember(Nti name)
		{
			if (!Members.Exists(name))
			{
				var mType = GetMemberType(name);
				switch (mType)
				{
					case MemberType.Method:
						AddNewMethod(name);
						break;
					case MemberType.Property:
						AddNewProperty(name);
						break;
					default:
						throw new ErrorException(ErrorCodes.PropertyIsNotFound, name);
				}
			}
			return base.DoGetMember(name);
		}

		enum MemberType
		{
			Unknown,
			Method,
			Property
		}

		MemberType GetMemberType(Nti nti)
		{
			String name = (new Nti(nti)).ToString();
			var fld = control.GetType().GetMember(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase);
			if (fld.Length < 1)
				return MemberType.Unknown;
			if (fld[0] is FieldInfo || fld[0] is PropertyInfo)
				return MemberType.Property;
			if (fld[0] is MethodInfo)
				return MemberType.Method;
			return MemberType.Unknown;
		}

		void AddNewProperty(Nti name)
		{
			var p = new GenericProperty(name, GetVariant(name), this);
			AddMember(p);
		}

		void AddNewMethod(Nti name)
		{
			var m = new GenericMethod(this, name, this);
			Add(name, m);
		}

		public void SetVariant(KnownNti nti, Variant value)
		{
			PropertyInfo prop = GetProperty(nti);
			prop.SetValue(control, value.ToType(prop.PropertyType), new object[] { });
		}

		public Variant GetVariant(KnownNti nti)
		{
			object value;
			PropertyInfo prop = GetProperty(nti);
			if (prop == null)
			{
				String name = (new Nti(nti)).ToString();
				var fld = (FieldInfo[])control.GetType().GetMember(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase);
				var fi = fld[0];
				value = fi.GetValue(control);
			}
			else
				value = prop.GetValue(control, new object[] { });

			var ret = Variant.Create(value);
			// TODO: Move this into a generic class
			if (ret.Type == VariantType.Unknown)
				ret = new Variant(new ControlClass(value));
			return ret;
		}

		PropertyInfo GetProperty(KnownNti nti)
		{
			String name = (new Nti(nti)).ToString();
			var prop = control.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.IgnoreCase);
			if (prop == null)
			{
				//throw new ErrorException(ErrorCodes.PropertyIsNotFound, name);
			}
			return prop;
		}

		public Variant CallMethod(KnownNti nti, ParameterCollection parms)
		{
			String name = (new Nti(nti)).ToString();
			var fld =
				control.GetType().GetMember(name,
																		BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static |
																		BindingFlags.IgnoreCase | BindingFlags.InvokeMethod);

			var p = parms.ToObjectArray();

			MethodInfo mi = null;
			foreach (MethodInfo method in fld)
			{
				var controlParams = method.GetParameters();
				if (controlParams.Length == p.Length)
				{
					mi = method;
					for (int i = 0; i < controlParams.Length; i++)
					{
						if ((controlParams[i].ParameterType == typeof(Int32) && p[i] is double))
							p[i] = (Int32) (Double) p[i];

							if (!controlParams[i].ParameterType.IsInstanceOfType(p[i]))
								mi = null;
					}

					if (mi != null)
						break;
				}
			}

			if (mi == null)
				throw new ErrorException(ErrorCodes.PropertyIsNotFound, nti);

			var value = mi.Invoke(control, p);

			var ret = Variant.Create(value);
			if (ret.Type == VariantType.Unknown)
				ret = new Variant(new ControlClass(value));
			return ret;
		}

		public event Action<EventData> EventHandler;
	}

	class ControlClassTemplate : ObjectTemplate
	{
		readonly Type type;

		internal ControlClassTemplate(Type t) : base(t.FullName) {
			type = t; }

		protected override void DoAddMembers()
		{
		}

		protected override void DoAddMembers(IMemberList members)
		{
		}

		protected override ObjectBase DoCreateInstance()
		{
			object o = Activator.CreateInstance(type);
			return new ControlClass(this, o);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new ControlClassTemplate(type);
		}
	}
}
