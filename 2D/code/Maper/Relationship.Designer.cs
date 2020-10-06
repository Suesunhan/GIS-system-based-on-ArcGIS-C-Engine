namespace Maper
{
    partial class Relationship
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
            this.checkListTargetLayer = new System.Windows.Forms.CheckedListBox();
            this.comBoxSourceLayer = new System.Windows.Forms.ComboBox();
            this.comBoxMethod = new System.Windows.Forms.ComboBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkListTargetLayer
            // 
            this.checkListTargetLayer.FormattingEnabled = true;
            this.checkListTargetLayer.Location = new System.Drawing.Point(12, 31);
            this.checkListTargetLayer.Name = "checkListTargetLayer";
            this.checkListTargetLayer.Size = new System.Drawing.Size(345, 224);
            this.checkListTargetLayer.TabIndex = 0;
            // 
            // comBoxSourceLayer
            // 
            this.comBoxSourceLayer.FormattingEnabled = true;
            this.comBoxSourceLayer.Location = new System.Drawing.Point(12, 293);
            this.comBoxSourceLayer.Name = "comBoxSourceLayer";
            this.comBoxSourceLayer.Size = new System.Drawing.Size(345, 23);
            this.comBoxSourceLayer.TabIndex = 1;
            // 
            // comBoxMethod
            // 
            this.comBoxMethod.FormattingEnabled = true;
            this.comBoxMethod.Location = new System.Drawing.Point(12, 362);
            this.comBoxMethod.Name = "comBoxMethod";
            this.comBoxMethod.Size = new System.Drawing.Size(345, 23);
            this.comBoxMethod.TabIndex = 2;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(186, 403);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 3;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(283, 403);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(75, 23);
            this.buttonExit.TabIndex = 5;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(89, 403);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "TargetLayer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 275);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "SourceLayer";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 344);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Method";
            // 
            // Relationship
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 472);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.comBoxMethod);
            this.Controls.Add(this.comBoxSourceLayer);
            this.Controls.Add(this.checkListTargetLayer);
            this.Name = "Relationship";
            this.Text = "Relationship";
            this.Load += new System.EventHandler(this.Relationship_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkListTargetLayer;
        private System.Windows.Forms.ComboBox comBoxSourceLayer;
        private System.Windows.Forms.ComboBox comBoxMethod;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;

    }
}