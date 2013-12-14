using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using Microsoft.WindowsCE.Forms;
using OpenNETCF.Windows.Forms;


namespace Guineu.compact
{
	public partial class Form1 : Form
	{
		//PersonCollection pc;
		Signature signature1;

		public Form1()
		{
			InitializeComponent();
			InputPanel ip = new InputPanel();
			signature1 = new Signature();
			this.signature1.BackColor = System.Drawing.Color.LightYellow;
			// this.signature1.BackgroundBitmap = ((System.Drawing.Bitmap)(resources.GetObject("signature1.BackgroundBitmap")));
			this.signature1.BorderColor = System.Drawing.Color.DarkGray;
			this.signature1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.signature1.ForeColor = System.Drawing.Color.Black;
			this.signature1.PenWidth = 2;
			this.signature1.Location = new System.Drawing.Point(10, 150);
			this.signature1.Size = new System.Drawing.Size(200, 100);
			this.signature1.Text = "SignatureControl";
			this.Controls.Add(this.signature1);
		//	ip.Enabled = true;

			//pc = new PersonCollection(new PersonFullNameAgeView());
			//pc.Add(new Person("John", "Opincar, Jr", "Thomas", new DateTime(1968, 8, 12)));
			//pc.Add(new Person("Abraham", "Lincoln", "X", new DateTime(1825, 1, 1)));
			//this.dataGrid1.DataSource = pc;

			//DataGridTableStyle ts = new DataGridTableStyle();
			//ts.MappingName = "PersonList";

			//DataGridTextBoxColumn cs1 = new DataGridTextBoxColumn();
			//cs1.MappingName = "FullName";
			//cs1.HeaderText = "Nombre";
			//ts.GridColumnStyles.Add(cs1);

			//DataGridTextBoxColumn cs2 = new DataGridTextBoxColumn();
			//cs2.MappingName = "Age";
			//cs2.HeaderText = "Edad";
			//ts.GridColumnStyles.Add(cs2);

			//dataGrid1.TableStyles.Clear();
			//dataGrid1.TableStyles.Add(ts);

		}

			private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			comboBox1.SelectedIndex = 1;
		}

	
		//private void button1_Click(object sender, EventArgs e)
		//{
		//  dataGrid1.TableStyles[0].GridColumnStyles.RemoveAt(1);
		//}
	}

	
	public class PersonFullNameAgeView : IPersonViewBuilder
	{
		public PropertyDescriptorCollection GetView()
		{
			List<PropertyDescriptor> props = new List<PropertyDescriptor>();

			PersonMethodDelegate del = delegate(Person p)
			{ return p.FirstName + " " + p.MiddleName + " " + p.LastName; };
			props.Add(new PersonMethodDescriptor("FullName", del, typeof(string)));
			
			del = delegate(Person p) { return DateTime.Today.Year - p.DateOfBirth.Year; };
			props.Add(new PersonMethodDescriptor("Age", del, typeof(int)));
			
			PropertyDescriptor[] propArray = new PropertyDescriptor[props.Count];
			props.CopyTo(propArray);
			
			return new PropertyDescriptorCollection(propArray);
		}
	}
	public delegate object PersonMethodDelegate(Person person);
	public class PersonMethodDescriptor : PropertyDescriptor
	{
		protected PersonMethodDelegate _method;
		protected Type _methodReturnType;

		public PersonMethodDescriptor(string name, PersonMethodDelegate method,
		 Type methodReturnType)
			: base(name, null)
		{
			_method = method;
			_methodReturnType = methodReturnType;
		}

		public override object GetValue(object component)
		{
			Person p = (Person)component;
			return _method(p);
		}

		public override Type ComponentType
		{
			get { return typeof(Person); }
		}

		public override Type PropertyType
		{
			get { return _methodReturnType; }
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override void ResetValue(object component) { }

		public override bool IsReadOnly
		{
			get { return true; }
		}

		public override void SetValue(object component, object value) { }

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
	public interface IPersonViewBuilder
	{
		PropertyDescriptorCollection GetView();
	}
	public class PersonCollection : BindingList<Person>, ITypedList
	{
		protected IPersonViewBuilder _viewBuilder;

		public PersonCollection(IPersonViewBuilder viewBuilder)
		{
			_viewBuilder = viewBuilder;
		}

		#region ITypedList Members

		protected PropertyDescriptorCollection _props;

		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (_props == null)
			{
				_props = _viewBuilder.GetView();
			}
			return _props;
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "PersonList"; // was used by 1.1 datagrid
		}

		#endregion

		}
	public class Person {
 protected string _firstName;
 protected string _lastName;
 protected string _midName;
 protected DateTime _dob;

 public Person(string firstName, string lastName, string midName, DateTime dob) {
  _firstName = firstName;
  _lastName = lastName;
  _midName = midName;
  _dob = dob;
 }

 public string FirstName {
  get { return _firstName; }
	 set { _firstName = value; }
 }

 public string LastName {
  get { return _lastName; }
 }

 public string MiddleName {
  get { return _midName; }
 }

 public DateTime DateOfBirth {
  get { return _dob; }
 }
}

}