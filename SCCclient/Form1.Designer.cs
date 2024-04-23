namespace conversaoClient
{
   partial class frmMain
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
         this.components = new System.ComponentModel.Container();
         this.cmdStop = new System.Windows.Forms.Button();
         this.cmdManual = new System.Windows.Forms.Button();
         this.timer1 = new System.Windows.Forms.Timer(this.components);
         this.txtCodigoAdm = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.gbConversao = new System.Windows.Forms.GroupBox();
         this.label3 = new System.Windows.Forms.Label();
         this.txtCodigoCliente = new System.Windows.Forms.TextBox();
         this.rbOmeupredioLocal = new System.Windows.Forms.RadioButton();
         this.rbWorkOffice = new System.Windows.Forms.RadioButton();
         this.txtPath = new System.Windows.Forms.TextBox();
         this.dtInicio = new System.Windows.Forms.DateTimePicker();
         this.rbSCCanexos = new System.Windows.Forms.RadioButton();
         this.rbOmeupredioTaco = new System.Windows.Forms.RadioButton();
         this.label2 = new System.Windows.Forms.Label();
         this.listBox1 = new System.Windows.Forms.ListBox();
         this.rbLocacaoTaco = new System.Windows.Forms.RadioButton();
         this.gbConversao.SuspendLayout();
         this.SuspendLayout();
         // 
         // cmdStop
         // 
         this.cmdStop.Location = new System.Drawing.Point(40, 538);
         this.cmdStop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.cmdStop.Name = "cmdStop";
         this.cmdStop.Size = new System.Drawing.Size(84, 31);
         this.cmdStop.TabIndex = 86;
         this.cmdStop.Text = "Stop";
         this.cmdStop.UseVisualStyleBackColor = true;
         this.cmdStop.Visible = false;
         this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
         // 
         // cmdManual
         // 
         this.cmdManual.Location = new System.Drawing.Point(526, 538);
         this.cmdManual.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.cmdManual.Name = "cmdManual";
         this.cmdManual.Size = new System.Drawing.Size(84, 31);
         this.cmdManual.TabIndex = 87;
         this.cmdManual.Text = "Manual";
         this.cmdManual.UseVisualStyleBackColor = true;
         this.cmdManual.Click += new System.EventHandler(this.cmdManual_Click);
         // 
         // timer1
         // 
         this.timer1.Interval = 2000;
         // 
         // txtCodigoAdm
         // 
         this.txtCodigoAdm.Location = new System.Drawing.Point(195, 6);
         this.txtCodigoAdm.Name = "txtCodigoAdm";
         this.txtCodigoAdm.Size = new System.Drawing.Size(76, 22);
         this.txtCodigoAdm.TabIndex = 98;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(37, 6);
         this.label1.Name = "label1";
         this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
         this.label1.Size = new System.Drawing.Size(148, 16);
         this.label1.TabIndex = 99;
         this.label1.Text = "Código Administradora:";
         // 
         // gbConversao
         // 
         this.gbConversao.Controls.Add(this.rbLocacaoTaco);
         this.gbConversao.Controls.Add(this.label3);
         this.gbConversao.Controls.Add(this.txtCodigoCliente);
         this.gbConversao.Controls.Add(this.rbOmeupredioLocal);
         this.gbConversao.Controls.Add(this.rbWorkOffice);
         this.gbConversao.Controls.Add(this.txtPath);
         this.gbConversao.Controls.Add(this.dtInicio);
         this.gbConversao.Controls.Add(this.rbSCCanexos);
         this.gbConversao.Controls.Add(this.rbOmeupredioTaco);
         this.gbConversao.Location = new System.Drawing.Point(35, 36);
         this.gbConversao.Name = "gbConversao";
         this.gbConversao.Size = new System.Drawing.Size(593, 177);
         this.gbConversao.TabIndex = 100;
         this.gbConversao.TabStop = false;
         this.gbConversao.Text = "Conversão ";
         this.gbConversao.Enter += new System.EventHandler(this.gbConversao_Enter);
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(363, 113);
         this.label3.Name = "label3";
         this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
         this.label3.Size = new System.Drawing.Size(129, 16);
         this.label3.TabIndex = 103;
         this.label3.Text = "Código Condomínio:";
         this.label3.Click += new System.EventHandler(this.label3_Click);
         // 
         // txtCodigoCliente
         // 
         this.txtCodigoCliente.Location = new System.Drawing.Point(498, 109);
         this.txtCodigoCliente.Name = "txtCodigoCliente";
         this.txtCodigoCliente.Size = new System.Drawing.Size(76, 22);
         this.txtCodigoCliente.TabIndex = 106;
         // 
         // rbOmeupredioLocal
         // 
         this.rbOmeupredioLocal.AutoSize = true;
         this.rbOmeupredioLocal.Location = new System.Drawing.Point(28, 114);
         this.rbOmeupredioLocal.Name = "rbOmeupredioLocal";
         this.rbOmeupredioLocal.Size = new System.Drawing.Size(245, 20);
         this.rbOmeupredioLocal.TabIndex = 105;
         this.rbOmeupredioLocal.Text = "Arquivos Omeupredio (Taco / Local)";
         this.rbOmeupredioLocal.UseVisualStyleBackColor = true;
         this.rbOmeupredioLocal.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged_1);
         // 
         // rbWorkOffice
         // 
         this.rbWorkOffice.AutoSize = true;
         this.rbWorkOffice.Location = new System.Drawing.Point(28, 84);
         this.rbWorkOffice.Name = "rbWorkOffice";
         this.rbWorkOffice.Size = new System.Drawing.Size(218, 20);
         this.rbWorkOffice.TabIndex = 104;
         this.rbWorkOffice.Text = "Foto Usuário (Workoffice/Azure)";
         this.rbWorkOffice.UseVisualStyleBackColor = true;
         // 
         // txtPath
         // 
         this.txtPath.Location = new System.Drawing.Point(381, 52);
         this.txtPath.Name = "txtPath";
         this.txtPath.ReadOnly = true;
         this.txtPath.Size = new System.Drawing.Size(194, 22);
         this.txtPath.TabIndex = 103;
         this.txtPath.Text = "C:\\\\PRG\\\\SCC\\\\ANEXOS\\\\";
         // 
         // dtInicio
         // 
         this.dtInicio.CustomFormat = "";
         this.dtInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
         this.dtInicio.Location = new System.Drawing.Point(458, 24);
         this.dtInicio.Name = "dtInicio";
         this.dtInicio.Size = new System.Drawing.Size(117, 22);
         this.dtInicio.TabIndex = 2;
         // 
         // rbSCCanexos
         // 
         this.rbSCCanexos.AutoSize = true;
         this.rbSCCanexos.Location = new System.Drawing.Point(30, 55);
         this.rbSCCanexos.Name = "rbSCCanexos";
         this.rbSCCanexos.Size = new System.Drawing.Size(277, 20);
         this.rbSCCanexos.TabIndex = 1;
         this.rbSCCanexos.Text = "Arquivos SCC GED local (SCC4W / Azure)";
         this.rbSCCanexos.UseVisualStyleBackColor = true;
         this.rbSCCanexos.CheckedChanged += new System.EventHandler(this.rbSCCanexos_CheckedChanged);
         // 
         // rbOmeupredioTaco
         // 
         this.rbOmeupredioTaco.AutoSize = true;
         this.rbOmeupredioTaco.Checked = true;
         this.rbOmeupredioTaco.Location = new System.Drawing.Point(29, 24);
         this.rbOmeupredioTaco.Name = "rbOmeupredioTaco";
         this.rbOmeupredioTaco.Size = new System.Drawing.Size(246, 20);
         this.rbOmeupredioTaco.TabIndex = 0;
         this.rbOmeupredioTaco.TabStop = true;
         this.rbOmeupredioTaco.Text = "Arquivos Omeupredio (Taco / Azure)";
         this.rbOmeupredioTaco.UseVisualStyleBackColor = true;
         this.rbOmeupredioTaco.CheckedChanged += new System.EventHandler(this.rbOmeupredioTaco_CheckedChanged);
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(430, 62);
         this.label2.Name = "label2";
         this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
         this.label2.Size = new System.Drawing.Size(54, 16);
         this.label2.TabIndex = 101;
         this.label2.Text = "Desde :";
         // 
         // listBox1
         // 
         this.listBox1.FormattingEnabled = true;
         this.listBox1.ItemHeight = 16;
         this.listBox1.Location = new System.Drawing.Point(35, 221);
         this.listBox1.Name = "listBox1";
         this.listBox1.ScrollAlwaysVisible = true;
         this.listBox1.Size = new System.Drawing.Size(593, 308);
         this.listBox1.TabIndex = 102;
         // 
         // rbLocacaoTaco
         // 
         this.rbLocacaoTaco.AutoSize = true;
         this.rbLocacaoTaco.Location = new System.Drawing.Point(28, 143);
         this.rbLocacaoTaco.Name = "rbLocacaoTaco";
         this.rbLocacaoTaco.Size = new System.Drawing.Size(276, 25);
         this.rbLocacaoTaco.TabIndex = 107;
         this.rbLocacaoTaco.Text = "Arquivos Locação (Taco /Azure)";
         this.rbLocacaoTaco.UseVisualStyleBackColor = true;
         // 
         // frmMain
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(642, 578);
         this.Controls.Add(this.listBox1);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.gbConversao);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.txtCodigoAdm);
         this.Controls.Add(this.cmdManual);
         this.Controls.Add(this.cmdStop);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmMain";
         this.Text = "Conversão";
         this.Load += new System.EventHandler(this.frmMain_Load);
         this.gbConversao.ResumeLayout(false);
         this.gbConversao.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.Button cmdStop;
      private System.Windows.Forms.Button cmdManual;
      private System.Windows.Forms.Timer timer1;
      private System.Windows.Forms.TextBox txtCodigoAdm;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.GroupBox gbConversao;
      private System.Windows.Forms.RadioButton rbSCCanexos;
      private System.Windows.Forms.RadioButton rbOmeupredioTaco;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.DateTimePicker dtInicio;
      private System.Windows.Forms.ListBox listBox1;
      private System.Windows.Forms.TextBox txtPath;
      private System.Windows.Forms.RadioButton rbWorkOffice;
      private System.Windows.Forms.RadioButton rbOmeupredioLocal;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox txtCodigoCliente;
      private System.Windows.Forms.RadioButton rbLocacaoTaco;
   }
}

