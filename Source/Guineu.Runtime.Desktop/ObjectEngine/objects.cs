using System;
using System.Collections.Generic;
using Guineu.Commands;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	internal class ObjectCollection : List<ObjectBase>
	{
		readonly ObjectBase parent;

		internal ObjectCollection(ObjectBase owner)
		{
			parent = owner;
		}

		/// <summary>
		/// Adds a child object to the collection of a parent.
		/// </summary>
		/// <param name="value"></param>
		public new void Add(ObjectBase value)
		{
			if (value.Nti.Valid())
				parent.Add(value.Nti, new ControlMember(value));

			// Add the control to our collection
			value.AssignParent(parent);
			base.Add(value);
		}
	}

	public interface IObjectFactory
	{
		ObjectBase CreateObject(Nti name, List<ExpressionBase> param);
		ObjectBase CreateObject(CallingContext context, Nti name, List<ExpressionBase> param);
		ObjectBase CreateObject(CallingContext context, ClassLocator clsLoc, List<ExpressionBase> param);
		ObjectBase AddObject(ObjectBase parent, CallingContext context, Nti name, Nti objName);
		void RegisterClass(ObjectTemplate o);
		Boolean IsRegistered(Nti name);
	}

	public interface IClassObjectManager
	{
		ObjectTemplate GetClassObject(CallingContext context, ClassLocator clsLoc);
		void RegisterBaseClass(ObjectTemplate cls);
		Boolean IsRegistered(Nti n);
	}

	public struct ClassLocator
	{
		internal Nti Name;
		internal IClassLibrary Src;
		internal Boolean IsForm;

		internal ClassLocator(Nti name, IClassLibrary src, Boolean isForm)
		{
			Name = name;
			Src = src;
			IsForm = isForm;
		}

		public ClassLocator(Nti name, IClassLibrary src)
			: this(name, src, false) { }

		public ClassLocator(Nti name)
			: this(name, null, false) { }
	}

	//======================================================================================
	public class ClassObjectManager
		: Dictionary<Nti, ObjectTemplate>,
			IClassObjectManager
	{
		/// <summary>
		/// Returns the class object for a given class from the class cache. If a class
		/// hasn't yet been loaded, it's added to the cache.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="clsLoc"></param>
		/// <returns></returns>
		public ObjectTemplate GetClassObject(CallingContext context, ClassLocator clsLoc)
		{
			if (!ContainsKey(clsLoc.Name))
			{
				CreateClass(context, clsLoc);
				if (!ContainsKey(clsLoc.Name))
					throw new ErrorException(ErrorCodes.ClassDefinitionNotFound, clsLoc.Name.ToString());
			}
			ObjectTemplate result = this[clsLoc.Name];
			return result;
		}

		private void CreateClass(CallingContext context, ClassLocator clsLoc)
		{
			var x = ClassResolver.FindClass(context, clsLoc);
			if (x == null)
				throw new ErrorException(ErrorCodes.ClassDefinitionNotFound);

			var ctx = x.GetObjectCreationContext(context, clsLoc, this);
			CreateClass(ctx);
		}

		private void CreateClass(ObjectCreationContext ctx)
		{
			ObjectTemplate newObj = ctx.ClassObject.CreateSubclass(ctx.Name);

			// AddBaseClass
			using (var cc = new CallingContext(ctx.Context.Context, newObj))
			{
				if (ctx.Constructor != null)
					cc.Execute(ctx.Constructor, newObj);
				Add(ctx.Name, newObj);
			}
		}

		public void RegisterBaseClass(ObjectTemplate cls)
		{
			if (cls == null)
				throw new ArgumentNullException("cls");
			Add(cls.Nti, cls);
		}

		public Boolean IsRegistered(Nti name)
		{
			Boolean registered = ContainsKey(name);
			return registered;
		}

	}

	//======================================================================================
	public struct ObjectCreationContext
	{
		internal CallingContext Context;
		internal Nti Name;
		internal CodeBlock Constructor;
		internal ObjectTemplate ClassObject;
	}


	//======================================================================================
	public sealed partial class ObjectFactory : IObjectFactory
	{
		readonly IClassObjectManager classes;

		public ObjectFactory() : this(new ClassObjectManager()) { }

		public ObjectFactory(IClassObjectManager clsMgr)
		{
			classes = clsMgr;
			RegisterBaseClasses();
		}
		partial void RegisterBaseClasses();

		public void RegisterClass(ObjectTemplate o)
		{
			classes.RegisterBaseClass(o);
		}

		public Boolean IsRegistered(Nti nti)
		{
			return classes.IsRegistered(nti);
		}

		public ObjectBase CreateObject(Nti name, List<ExpressionBase> p)
		{
			using (var context = new CallingContext(GuineuInstance.Context))
				return CreateAndInitializeObject(new ClassLocator(name), context, p);
		}

		public ObjectBase CreateObject(CallingContext context, Nti name, List<ExpressionBase> p)
		{
			return CreateAndInitializeObject(new ClassLocator(name), context, p);
		}

		public ObjectBase CreateObject(CallingContext context, ClassLocator clsLoc, List<ExpressionBase> p)
		{
			return CreateAndInitializeObject(clsLoc, context, p);
		}

		public ObjectBase AddObject(ObjectBase parent, CallingContext context, Nti name, Nti objName)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			ObjectTemplate classObj = classes.GetClassObject(context, new ClassLocator(name));
			// TODO: Distinuish betwen adding to a class object and adding to an instance
			ObjectTemplate newObj = classObj.CreateSubclass(objName);
			parent.Controls.Add(newObj);
			return newObj;
		}

		private ObjectBase CreateAndInitializeObject(ClassLocator clsLoc, CallingContext context, IEnumerable<ExpressionBase> p)
		{
			var parms = new ParameterCollection(context, p);
			ObjectTemplate classObj = classes.GetClassObject(context, clsLoc);
			ObjectBase retVal = classObj.CreateInstance();
			if (clsLoc.IsForm)
				retVal.EnableThisform();
			// TODO: Evaluate return value
			retVal.InitInstance(parms);
			return retVal;
		}
	}
}
