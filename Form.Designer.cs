namespace Kawai2003_trainer
{
    partial class Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnMatchOne = new Button();
            cbAuto = new CheckBox();
            SuspendLayout();
            // 
            // btnMatchOne
            // 
            btnMatchOne.Location = new Point(314, 168);
            btnMatchOne.Name = "btnMatchOne";
            btnMatchOne.Size = new Size(230, 79);
            btnMatchOne.TabIndex = 1;
            btnMatchOne.Text = "Match one pair";
            btnMatchOne.UseVisualStyleBackColor = true;
            btnMatchOne.Click += btnMatchOne_Click;
            // 
            // cbAuto
            // 
            cbAuto.AutoSize = true;
            cbAuto.Location = new Point(182, 190);
            cbAuto.Name = "cbAuto";
            cbAuto.Size = new Size(97, 36);
            cbAuto.TabIndex = 2;
            cbAuto.Text = "Auto";
            cbAuto.UseVisualStyleBackColor = true;
            cbAuto.CheckedChanged += cbAuto_CheckedChanged;
            // 
            // Form
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(cbAuto);
            Controls.Add(btnMatchOne);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Kawai 2003 trainer";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnMatchOne;
        private CheckBox cbAuto;
    }
}