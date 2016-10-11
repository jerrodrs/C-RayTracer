namespace rayTracer
{
    partial class rayTracerUI
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
            this.label1 = new System.Windows.Forms.Label();
            this.statusTxt = new System.Windows.Forms.TextBox();
            this.renderBox = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.renderTimeTxt = new System.Windows.Forms.TextBox();
            this.renderBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.fpsTxt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.aaSamplesTxt = new System.Windows.Forms.NumericUpDown();
            this.threadsTxt = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.renderBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aaSamplesTxt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.threadsTxt)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status:";
            // 
            // statusTxt
            // 
            this.statusTxt.Location = new System.Drawing.Point(90, 10);
            this.statusTxt.Name = "statusTxt";
            this.statusTxt.ReadOnly = true;
            this.statusTxt.Size = new System.Drawing.Size(152, 20);
            this.statusTxt.TabIndex = 1;
            // 
            // renderBox
            // 
            this.renderBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.renderBox.Location = new System.Drawing.Point(264, 10);
            this.renderBox.Name = "renderBox";
            this.renderBox.Size = new System.Drawing.Size(640, 480);
            this.renderBox.TabIndex = 2;
            this.renderBox.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Render Time:";
            // 
            // renderTimeTxt
            // 
            this.renderTimeTxt.Location = new System.Drawing.Point(90, 33);
            this.renderTimeTxt.Name = "renderTimeTxt";
            this.renderTimeTxt.ReadOnly = true;
            this.renderTimeTxt.Size = new System.Drawing.Size(152, 20);
            this.renderTimeTxt.TabIndex = 4;
            // 
            // renderBtn
            // 
            this.renderBtn.Location = new System.Drawing.Point(8, 467);
            this.renderBtn.Name = "renderBtn";
            this.renderBtn.Size = new System.Drawing.Size(75, 23);
            this.renderBtn.TabIndex = 5;
            this.renderBtn.Text = "Render";
            this.renderBtn.UseVisualStyleBackColor = true;
            this.renderBtn.Click += new System.EventHandler(this.renderBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "FPS:";
            // 
            // fpsTxt
            // 
            this.fpsTxt.Location = new System.Drawing.Point(90, 56);
            this.fpsTxt.Name = "fpsTxt";
            this.fpsTxt.ReadOnly = true;
            this.fpsTxt.Size = new System.Drawing.Size(152, 20);
            this.fpsTxt.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "AA Samples:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Threads:";
            // 
            // aaSamplesTxt
            // 
            this.aaSamplesTxt.Location = new System.Drawing.Point(90, 101);
            this.aaSamplesTxt.Name = "aaSamplesTxt";
            this.aaSamplesTxt.Size = new System.Drawing.Size(152, 20);
            this.aaSamplesTxt.TabIndex = 12;
            this.aaSamplesTxt.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // threadsTxt
            // 
            this.threadsTxt.Location = new System.Drawing.Point(90, 127);
            this.threadsTxt.Name = "threadsTxt";
            this.threadsTxt.Size = new System.Drawing.Size(152, 20);
            this.threadsTxt.TabIndex = 13;
            this.threadsTxt.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // rayTracerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 499);
            this.Controls.Add(this.threadsTxt);
            this.Controls.Add(this.aaSamplesTxt);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.fpsTxt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.renderBtn);
            this.Controls.Add(this.renderTimeTxt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.renderBox);
            this.Controls.Add(this.statusTxt);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "rayTracerUI";
            this.ShowIcon = false;
            this.Text = "rayTracer";
            ((System.ComponentModel.ISupportInitialize)(this.renderBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aaSamplesTxt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.threadsTxt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox statusTxt;
        private System.Windows.Forms.PictureBox renderBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox renderTimeTxt;
        private System.Windows.Forms.Button renderBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox fpsTxt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown aaSamplesTxt;
        private System.Windows.Forms.NumericUpDown threadsTxt;
    }
}

