namespace FLER
{
    partial class FLERForm
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
            System.Windows.Forms.Label lbl_tags;
            System.Windows.Forms.Label lbl_img_left;
            System.Windows.Forms.Label lbl_img_top;
            System.Windows.Forms.Label lbl_img_height;
            System.Windows.Forms.Label lbl_img_width;
            System.Windows.Forms.Label lbl_txt_left;
            System.Windows.Forms.Label lbl_txt_top;
            System.Windows.Forms.Label lbl_txt_height;
            System.Windows.Forms.Label lbl_txt_width;
            System.Windows.Forms.Label lbl_filename;
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.txt_tags = new System.Windows.Forms.TextBox();
            this.radio_ml = new System.Windows.Forms.RadioButton();
            this.radio_tl = new System.Windows.Forms.RadioButton();
            this.radio_mr = new System.Windows.Forms.RadioButton();
            this.radio_tr = new System.Windows.Forms.RadioButton();
            this.check_drawover = new System.Windows.Forms.CheckBox();
            this.txt_text = new System.Windows.Forms.TextBox();
            this.pnl_builder = new System.Windows.Forms.Panel();
            this.btn_save = new System.Windows.Forms.Button();
            this.txt_filename = new System.Windows.Forms.TextBox();
            this.lbl_font = new System.Windows.Forms.Label();
            this.num_img_left = new System.Windows.Forms.NumericUpDown();
            this.num_img_top = new System.Windows.Forms.NumericUpDown();
            this.num_img_width = new System.Windows.Forms.NumericUpDown();
            this.num_img_height = new System.Windows.Forms.NumericUpDown();
            this.pnl_txt_color = new System.Windows.Forms.Panel();
            this.img_img = new System.Windows.Forms.PictureBox();
            this.radio_bc = new System.Windows.Forms.RadioButton();
            this.radio_mc = new System.Windows.Forms.RadioButton();
            this.radio_tc = new System.Windows.Forms.RadioButton();
            this.radio_br = new System.Windows.Forms.RadioButton();
            this.pnl_linecolor = new System.Windows.Forms.Panel();
            this.pnl_backcolor = new System.Windows.Forms.Panel();
            this.radio_bl = new System.Windows.Forms.RadioButton();
            this.num_txt_left = new System.Windows.Forms.NumericUpDown();
            this.num_txt_top = new System.Windows.Forms.NumericUpDown();
            this.num_txt_width = new System.Windows.Forms.NumericUpDown();
            this.num_txt_height = new System.Windows.Forms.NumericUpDown();
            this.tim_builder = new System.Windows.Forms.Timer(this.components);
            this.btn_success = new System.Windows.Forms.Button();
            this.btn_fail = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            lbl_tags = new System.Windows.Forms.Label();
            lbl_img_left = new System.Windows.Forms.Label();
            lbl_img_top = new System.Windows.Forms.Label();
            lbl_img_height = new System.Windows.Forms.Label();
            lbl_img_width = new System.Windows.Forms.Label();
            lbl_txt_left = new System.Windows.Forms.Label();
            lbl_txt_top = new System.Windows.Forms.Label();
            lbl_txt_height = new System.Windows.Forms.Label();
            lbl_txt_width = new System.Windows.Forms.Label();
            lbl_filename = new System.Windows.Forms.Label();
            this.pnl_builder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_img_left)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_img_top)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_img_width)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_img_height)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.img_img)).BeginInit();
            this.pnl_linecolor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_txt_left)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_txt_top)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_txt_width)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_txt_height)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_tags
            // 
            lbl_tags.AutoSize = true;
            lbl_tags.Location = new System.Drawing.Point(5, 42);
            lbl_tags.Name = "lbl_tags";
            lbl_tags.Size = new System.Drawing.Size(140, 13);
            lbl_tags.TabIndex = 45;
            lbl_tags.Text = "Tags (separate with spaces)";
            // 
            // lbl_img_left
            // 
            lbl_img_left.AutoSize = true;
            lbl_img_left.Location = new System.Drawing.Point(5, 137);
            lbl_img_left.Name = "lbl_img_left";
            lbl_img_left.Size = new System.Drawing.Size(25, 13);
            lbl_img_left.TabIndex = 37;
            lbl_img_left.Text = "Left";
            // 
            // lbl_img_top
            // 
            lbl_img_top.AutoSize = true;
            lbl_img_top.Location = new System.Drawing.Point(60, 137);
            lbl_img_top.Name = "lbl_img_top";
            lbl_img_top.Size = new System.Drawing.Size(26, 13);
            lbl_img_top.TabIndex = 39;
            lbl_img_top.Text = "Top";
            // 
            // lbl_img_height
            // 
            lbl_img_height.AutoSize = true;
            lbl_img_height.Location = new System.Drawing.Point(60, 176);
            lbl_img_height.Name = "lbl_img_height";
            lbl_img_height.Size = new System.Drawing.Size(38, 13);
            lbl_img_height.TabIndex = 43;
            lbl_img_height.Text = "Height";
            // 
            // lbl_img_width
            // 
            lbl_img_width.AutoSize = true;
            lbl_img_width.Location = new System.Drawing.Point(5, 176);
            lbl_img_width.Name = "lbl_img_width";
            lbl_img_width.Size = new System.Drawing.Size(35, 13);
            lbl_img_width.TabIndex = 41;
            lbl_img_width.Text = "Width";
            // 
            // lbl_txt_left
            // 
            lbl_txt_left.AutoSize = true;
            lbl_txt_left.Location = new System.Drawing.Point(130, 137);
            lbl_txt_left.Name = "lbl_txt_left";
            lbl_txt_left.Size = new System.Drawing.Size(25, 13);
            lbl_txt_left.TabIndex = 25;
            lbl_txt_left.Text = "Left";
            // 
            // lbl_txt_top
            // 
            lbl_txt_top.AutoSize = true;
            lbl_txt_top.Location = new System.Drawing.Point(189, 137);
            lbl_txt_top.Name = "lbl_txt_top";
            lbl_txt_top.Size = new System.Drawing.Size(26, 13);
            lbl_txt_top.TabIndex = 27;
            lbl_txt_top.Text = "Top";
            // 
            // lbl_txt_height
            // 
            lbl_txt_height.AutoSize = true;
            lbl_txt_height.Location = new System.Drawing.Point(189, 176);
            lbl_txt_height.Name = "lbl_txt_height";
            lbl_txt_height.Size = new System.Drawing.Size(38, 13);
            lbl_txt_height.TabIndex = 31;
            lbl_txt_height.Text = "Height";
            // 
            // lbl_txt_width
            // 
            lbl_txt_width.AutoSize = true;
            lbl_txt_width.Location = new System.Drawing.Point(130, 176);
            lbl_txt_width.Name = "lbl_txt_width";
            lbl_txt_width.Size = new System.Drawing.Size(35, 13);
            lbl_txt_width.TabIndex = 29;
            lbl_txt_width.Text = "Width";
            // 
            // lbl_filename
            // 
            lbl_filename.AutoSize = true;
            lbl_filename.Location = new System.Drawing.Point(5, 4);
            lbl_filename.Name = "lbl_filename";
            lbl_filename.Size = new System.Drawing.Size(49, 13);
            lbl_filename.TabIndex = 48;
            lbl_filename.Text = "Filename";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 42);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(93, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2 (edit)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.BuildNewCard);
            // 
            // txt_tags
            // 
            this.txt_tags.Location = new System.Drawing.Point(7, 57);
            this.txt_tags.Multiline = true;
            this.txt_tags.Name = "txt_tags";
            this.txt_tags.Size = new System.Drawing.Size(232, 20);
            this.txt_tags.TabIndex = 8;
            this.txt_tags.TextChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // radio_ml
            // 
            this.radio_ml.AutoSize = true;
            this.radio_ml.Location = new System.Drawing.Point(159, 238);
            this.radio_ml.Name = "radio_ml";
            this.radio_ml.Size = new System.Drawing.Size(14, 13);
            this.radio_ml.TabIndex = 19;
            this.radio_ml.TabStop = true;
            this.radio_ml.UseVisualStyleBackColor = true;
            this.radio_ml.CheckedChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // radio_tl
            // 
            this.radio_tl.AutoSize = true;
            this.radio_tl.Location = new System.Drawing.Point(159, 219);
            this.radio_tl.Name = "radio_tl";
            this.radio_tl.Size = new System.Drawing.Size(14, 13);
            this.radio_tl.TabIndex = 18;
            this.radio_tl.TabStop = true;
            this.radio_tl.Tag = "";
            this.radio_tl.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radio_tl.UseVisualStyleBackColor = true;
            this.radio_tl.CheckedChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // radio_mr
            // 
            this.radio_mr.AutoSize = true;
            this.radio_mr.Location = new System.Drawing.Point(199, 238);
            this.radio_mr.Name = "radio_mr";
            this.radio_mr.Size = new System.Drawing.Size(14, 13);
            this.radio_mr.TabIndex = 19;
            this.radio_mr.TabStop = true;
            this.radio_mr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.radio_mr.UseVisualStyleBackColor = true;
            this.radio_mr.CheckedChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // radio_tr
            // 
            this.radio_tr.AutoSize = true;
            this.radio_tr.Location = new System.Drawing.Point(199, 219);
            this.radio_tr.Name = "radio_tr";
            this.radio_tr.Size = new System.Drawing.Size(14, 13);
            this.radio_tr.TabIndex = 18;
            this.radio_tr.TabStop = true;
            this.radio_tr.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.radio_tr.UseVisualStyleBackColor = true;
            this.radio_tr.CheckedChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // check_drawover
            // 
            this.check_drawover.AutoSize = true;
            this.check_drawover.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.check_drawover.Location = new System.Drawing.Point(15, 212);
            this.check_drawover.Name = "check_drawover";
            this.check_drawover.Size = new System.Drawing.Size(99, 18);
            this.check_drawover.TabIndex = 22;
            this.check_drawover.Text = "Draw over text";
            this.check_drawover.UseVisualStyleBackColor = true;
            this.check_drawover.CheckedChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // txt_text
            // 
            this.txt_text.Location = new System.Drawing.Point(133, 83);
            this.txt_text.Multiline = true;
            this.txt_text.Name = "txt_text";
            this.txt_text.Size = new System.Drawing.Size(106, 34);
            this.txt_text.TabIndex = 23;
            this.txt_text.Text = "Text";
            this.txt_text.TextChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // pnl_builder
            // 
            this.pnl_builder.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnl_builder.Controls.Add(this.btn_save);
            this.pnl_builder.Controls.Add(lbl_filename);
            this.pnl_builder.Controls.Add(this.txt_filename);
            this.pnl_builder.Controls.Add(this.lbl_font);
            this.pnl_builder.Controls.Add(lbl_tags);
            this.pnl_builder.Controls.Add(this.num_img_left);
            this.pnl_builder.Controls.Add(lbl_img_left);
            this.pnl_builder.Controls.Add(this.num_img_top);
            this.pnl_builder.Controls.Add(lbl_img_top);
            this.pnl_builder.Controls.Add(this.num_img_width);
            this.pnl_builder.Controls.Add(lbl_img_height);
            this.pnl_builder.Controls.Add(lbl_img_width);
            this.pnl_builder.Controls.Add(this.num_img_height);
            this.pnl_builder.Controls.Add(this.pnl_txt_color);
            this.pnl_builder.Controls.Add(this.img_img);
            this.pnl_builder.Controls.Add(this.radio_bc);
            this.pnl_builder.Controls.Add(this.radio_mc);
            this.pnl_builder.Controls.Add(this.radio_tc);
            this.pnl_builder.Controls.Add(this.radio_br);
            this.pnl_builder.Controls.Add(this.pnl_linecolor);
            this.pnl_builder.Controls.Add(this.radio_bl);
            this.pnl_builder.Controls.Add(this.radio_mr);
            this.pnl_builder.Controls.Add(this.radio_ml);
            this.pnl_builder.Controls.Add(this.radio_tr);
            this.pnl_builder.Controls.Add(this.num_txt_left);
            this.pnl_builder.Controls.Add(this.radio_tl);
            this.pnl_builder.Controls.Add(lbl_txt_left);
            this.pnl_builder.Controls.Add(this.num_txt_top);
            this.pnl_builder.Controls.Add(lbl_txt_top);
            this.pnl_builder.Controls.Add(this.num_txt_width);
            this.pnl_builder.Controls.Add(lbl_txt_height);
            this.pnl_builder.Controls.Add(lbl_txt_width);
            this.pnl_builder.Controls.Add(this.num_txt_height);
            this.pnl_builder.Controls.Add(this.txt_text);
            this.pnl_builder.Controls.Add(this.check_drawover);
            this.pnl_builder.Controls.Add(this.txt_tags);
            this.pnl_builder.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnl_builder.Location = new System.Drawing.Point(734, 0);
            this.pnl_builder.Name = "pnl_builder";
            this.pnl_builder.Size = new System.Drawing.Size(242, 378);
            this.pnl_builder.TabIndex = 5;
            this.pnl_builder.Visible = false;
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(7, 284);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(232, 23);
            this.btn_save.TabIndex = 7;
            this.btn_save.Text = "SAVE";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // txt_filename
            // 
            this.txt_filename.Location = new System.Drawing.Point(7, 19);
            this.txt_filename.MaxLength = 64;
            this.txt_filename.Multiline = true;
            this.txt_filename.Name = "txt_filename";
            this.txt_filename.Size = new System.Drawing.Size(232, 20);
            this.txt_filename.TabIndex = 47;
            this.txt_filename.TextChanged += new System.EventHandler(this.ValidateFilename);
            this.txt_filename.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ValidateFilenameKeyPress);
            // 
            // lbl_font
            // 
            this.lbl_font.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_font.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_font.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbl_font.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.lbl_font.Location = new System.Drawing.Point(8, 229);
            this.lbl_font.Name = "lbl_font";
            this.lbl_font.Size = new System.Drawing.Size(109, 41);
            this.lbl_font.TabIndex = 46;
            this.lbl_font.Tag = "";
            this.lbl_font.Text = "Arial, 24pt, Bold";
            this.lbl_font.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_font.UseCompatibleTextRendering = true;
            this.lbl_font.Click += new System.EventHandler(this.PickFont);
            // 
            // num_img_left
            // 
            this.num_img_left.Location = new System.Drawing.Point(7, 153);
            this.num_img_left.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_img_left.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.num_img_left.Name = "num_img_left";
            this.num_img_left.Size = new System.Drawing.Size(50, 20);
            this.num_img_left.TabIndex = 36;
            this.num_img_left.ValueChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // num_img_top
            // 
            this.num_img_top.Location = new System.Drawing.Point(63, 153);
            this.num_img_top.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_img_top.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.num_img_top.Name = "num_img_top";
            this.num_img_top.Size = new System.Drawing.Size(50, 20);
            this.num_img_top.TabIndex = 38;
            this.num_img_top.ValueChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // num_img_width
            // 
            this.num_img_width.Location = new System.Drawing.Point(8, 192);
            this.num_img_width.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_img_width.Name = "num_img_width";
            this.num_img_width.Size = new System.Drawing.Size(49, 20);
            this.num_img_width.TabIndex = 40;
            this.num_img_width.ValueChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // num_img_height
            // 
            this.num_img_height.Location = new System.Drawing.Point(63, 192);
            this.num_img_height.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_img_height.Name = "num_img_height";
            this.num_img_height.Size = new System.Drawing.Size(50, 20);
            this.num_img_height.TabIndex = 42;
            this.num_img_height.ValueChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // pnl_txt_color
            // 
            this.pnl_txt_color.BackColor = System.Drawing.Color.Black;
            this.pnl_txt_color.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnl_txt_color.Location = new System.Drawing.Point(133, 123);
            this.pnl_txt_color.Name = "pnl_txt_color";
            this.pnl_txt_color.Size = new System.Drawing.Size(106, 10);
            this.pnl_txt_color.TabIndex = 34;
            this.pnl_txt_color.Click += new System.EventHandler(this.PickColor);
            // 
            // img_img
            // 
            this.img_img.Cursor = System.Windows.Forms.Cursors.Hand;
            this.img_img.Image = global::FLER.Properties.Resources.Missing;
            this.img_img.Location = new System.Drawing.Point(63, 83);
            this.img_img.Name = "img_img";
            this.img_img.Size = new System.Drawing.Size(50, 50);
            this.img_img.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.img_img.TabIndex = 7;
            this.img_img.TabStop = false;
            this.img_img.Click += new System.EventHandler(this.PickImage);
            // 
            // radio_bc
            // 
            this.radio_bc.AutoSize = true;
            this.radio_bc.Location = new System.Drawing.Point(179, 257);
            this.radio_bc.Name = "radio_bc";
            this.radio_bc.Size = new System.Drawing.Size(14, 13);
            this.radio_bc.TabIndex = 35;
            this.radio_bc.TabStop = true;
            this.radio_bc.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.radio_bc.UseVisualStyleBackColor = true;
            this.radio_bc.CheckedChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // radio_mc
            // 
            this.radio_mc.AutoSize = true;
            this.radio_mc.Checked = true;
            this.radio_mc.Location = new System.Drawing.Point(179, 238);
            this.radio_mc.Name = "radio_mc";
            this.radio_mc.Size = new System.Drawing.Size(14, 13);
            this.radio_mc.TabIndex = 34;
            this.radio_mc.TabStop = true;
            this.radio_mc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radio_mc.UseVisualStyleBackColor = true;
            this.radio_mc.CheckedChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // radio_tc
            // 
            this.radio_tc.AutoSize = true;
            this.radio_tc.Location = new System.Drawing.Point(179, 219);
            this.radio_tc.Name = "radio_tc";
            this.radio_tc.Size = new System.Drawing.Size(14, 13);
            this.radio_tc.TabIndex = 33;
            this.radio_tc.TabStop = true;
            this.radio_tc.Tag = "";
            this.radio_tc.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.radio_tc.UseVisualStyleBackColor = true;
            this.radio_tc.CheckedChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // radio_br
            // 
            this.radio_br.AutoSize = true;
            this.radio_br.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radio_br.Location = new System.Drawing.Point(199, 257);
            this.radio_br.Name = "radio_br";
            this.radio_br.Size = new System.Drawing.Size(14, 13);
            this.radio_br.TabIndex = 20;
            this.radio_br.TabStop = true;
            this.radio_br.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radio_br.UseVisualStyleBackColor = true;
            this.radio_br.CheckedChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // pnl_linecolor
            // 
            this.pnl_linecolor.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.pnl_linecolor.Controls.Add(this.pnl_backcolor);
            this.pnl_linecolor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnl_linecolor.Location = new System.Drawing.Point(7, 83);
            this.pnl_linecolor.Name = "pnl_linecolor";
            this.pnl_linecolor.Size = new System.Drawing.Size(50, 50);
            this.pnl_linecolor.TabIndex = 32;
            this.pnl_linecolor.Click += new System.EventHandler(this.PickColor);
            // 
            // pnl_backcolor
            // 
            this.pnl_backcolor.BackColor = System.Drawing.Color.LightSkyBlue;
            this.pnl_backcolor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnl_backcolor.Location = new System.Drawing.Point(10, 10);
            this.pnl_backcolor.Name = "pnl_backcolor";
            this.pnl_backcolor.Size = new System.Drawing.Size(30, 30);
            this.pnl_backcolor.TabIndex = 33;
            this.pnl_backcolor.Click += new System.EventHandler(this.PickColor);
            // 
            // radio_bl
            // 
            this.radio_bl.AutoSize = true;
            this.radio_bl.Location = new System.Drawing.Point(159, 257);
            this.radio_bl.Name = "radio_bl";
            this.radio_bl.Size = new System.Drawing.Size(14, 13);
            this.radio_bl.TabIndex = 20;
            this.radio_bl.TabStop = true;
            this.radio_bl.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.radio_bl.UseVisualStyleBackColor = true;
            this.radio_bl.CheckedChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // num_txt_left
            // 
            this.num_txt_left.Location = new System.Drawing.Point(133, 153);
            this.num_txt_left.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_txt_left.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.num_txt_left.Name = "num_txt_left";
            this.num_txt_left.Size = new System.Drawing.Size(50, 20);
            this.num_txt_left.TabIndex = 24;
            this.num_txt_left.ValueChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // num_txt_top
            // 
            this.num_txt_top.Location = new System.Drawing.Point(189, 153);
            this.num_txt_top.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_txt_top.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.num_txt_top.Name = "num_txt_top";
            this.num_txt_top.Size = new System.Drawing.Size(50, 20);
            this.num_txt_top.TabIndex = 26;
            this.num_txt_top.ValueChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // num_txt_width
            // 
            this.num_txt_width.Location = new System.Drawing.Point(133, 192);
            this.num_txt_width.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_txt_width.Name = "num_txt_width";
            this.num_txt_width.Size = new System.Drawing.Size(50, 20);
            this.num_txt_width.TabIndex = 28;
            this.num_txt_width.ValueChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // num_txt_height
            // 
            this.num_txt_height.Location = new System.Drawing.Point(189, 192);
            this.num_txt_height.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.num_txt_height.Name = "num_txt_height";
            this.num_txt_height.Size = new System.Drawing.Size(50, 20);
            this.num_txt_height.TabIndex = 30;
            this.num_txt_height.ValueChanged += new System.EventHandler(this.BeginUpdateBuilder);
            // 
            // tim_builder
            // 
            this.tim_builder.Interval = 16;
            this.tim_builder.Tick += new System.EventHandler(this.tim_builder_Tick);
            // 
            // btn_success
            // 
            this.btn_success.Location = new System.Drawing.Point(444, 343);
            this.btn_success.Name = "btn_success";
            this.btn_success.Size = new System.Drawing.Size(75, 23);
            this.btn_success.TabIndex = 6;
            this.btn_success.Text = "yay!";
            this.btn_success.UseVisualStyleBackColor = true;
            this.btn_success.Visible = false;
            this.btn_success.Click += new System.EventHandler(this.DrawCard);
            // 
            // btn_fail
            // 
            this.btn_fail.Location = new System.Drawing.Point(363, 343);
            this.btn_fail.Name = "btn_fail";
            this.btn_fail.Size = new System.Drawing.Size(75, 23);
            this.btn_fail.TabIndex = 7;
            this.btn_fail.Text = "boo!";
            this.btn_fail.UseVisualStyleBackColor = true;
            this.btn_fail.Visible = false;
            this.btn_fail.Click += new System.EventHandler(this.DrawCard);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(93, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "button3 (draw)";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.DrawCard);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 343);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "button1 (back)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FLERForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 378);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btn_fail);
            this.Controls.Add(this.btn_success);
            this.Controls.Add(this.pnl_builder);
            this.Controls.Add(this.button2);
            this.Name = "FLERForm";
            this.Text = "Form1";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FLERForm_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FLERForm_MouseDown);
            this.MouseLeave += new System.EventHandler(this.FLERForm_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FLERForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FLERForm_MouseUp);
            this.pnl_builder.ResumeLayout(false);
            this.pnl_builder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_img_left)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_img_top)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_img_width)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_img_height)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.img_img)).EndInit();
            this.pnl_linecolor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.num_txt_left)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_txt_top)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_txt_width)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_txt_height)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txt_tags;
        private System.Windows.Forms.RadioButton radio_ml;
        private System.Windows.Forms.RadioButton radio_tl;
        private System.Windows.Forms.RadioButton radio_mr;
        private System.Windows.Forms.RadioButton radio_tr;
        private System.Windows.Forms.CheckBox check_drawover;
        private System.Windows.Forms.TextBox txt_text;
        private System.Windows.Forms.Panel pnl_builder;
        private System.Windows.Forms.RadioButton radio_bc;
        private System.Windows.Forms.RadioButton radio_mc;
        private System.Windows.Forms.RadioButton radio_tc;
        private System.Windows.Forms.RadioButton radio_br;
        private System.Windows.Forms.Panel pnl_linecolor;
        private System.Windows.Forms.RadioButton radio_bl;
        private System.Windows.Forms.NumericUpDown num_txt_left;
        private System.Windows.Forms.NumericUpDown num_txt_top;
        private System.Windows.Forms.NumericUpDown num_txt_width;
        private System.Windows.Forms.NumericUpDown num_txt_height;
        private System.Windows.Forms.PictureBox img_img;
        private System.Windows.Forms.NumericUpDown num_img_left;
        private System.Windows.Forms.NumericUpDown num_img_top;
        private System.Windows.Forms.NumericUpDown num_img_width;
        private System.Windows.Forms.NumericUpDown num_img_height;
        private System.Windows.Forms.Panel pnl_txt_color;
        private System.Windows.Forms.Panel pnl_backcolor;
        private System.Windows.Forms.Label lbl_font;
        private System.Windows.Forms.TextBox txt_filename;
        private System.Windows.Forms.Timer tim_builder;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_success;
        private System.Windows.Forms.Button btn_fail;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
    }
}

