namespace WinForm
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnExec = new System.Windows.Forms.Button();
			this.list = new System.Windows.Forms.ListBox();
			this.btnDo = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(53, 12);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(260, 20);
			this.txtName.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Name";
			// 
			// btnExec
			// 
			this.btnExec.Location = new System.Drawing.Point(214, 136);
			this.btnExec.Name = "btnExec";
			this.btnExec.Size = new System.Drawing.Size(99, 23);
			this.btnExec.TabIndex = 2;
			this.btnExec.Text = "btnClick(tcName)";
			this.btnExec.UseVisualStyleBackColor = true;
			this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
			// 
			// list
			// 
			this.list.FormattingEnabled = true;
			this.list.Location = new System.Drawing.Point(53, 48);
			this.list.Name = "list";
			this.list.Size = new System.Drawing.Size(260, 82);
			this.list.TabIndex = 3;
			// 
			// btnDo
			// 
			this.btnDo.Location = new System.Drawing.Point(109, 136);
			this.btnDo.Name = "btnDo";
			this.btnDo.Size = new System.Drawing.Size(99, 23);
			this.btnDo.TabIndex = 4;
			this.btnDo.Text = "Execute FXP";
			this.btnDo.UseVisualStyleBackColor = true;
			this.btnDo.Click += new System.EventHandler(this.btnDo_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(325, 167);
			this.Controls.Add(this.btnDo);
			this.Controls.Add(this.list);
			this.Controls.Add(this.btnExec);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Name = "Form1";
			this.Text = "Call FXP files";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnExec;
		private System.Windows.Forms.ListBox list;
		private System.Windows.Forms.Button btnDo;

	}
}

