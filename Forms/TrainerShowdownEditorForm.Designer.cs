﻿using System;

namespace ImpostersOrdeal
{
    partial class TrainerShowdownEditorForm
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.Save = new System.Windows.Forms.Button();
            this.Preview = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(384, 461);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "Paste Showdown Here";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(402, 12);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ReadOnly = true;
            this.richTextBox2.Size = new System.Drawing.Size(384, 461);
            this.richTextBox2.TabIndex = 1;
            this.richTextBox2.Text = "Preview";
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(580, 479);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(100, 29);
            this.Save.TabIndex = 2;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Preview
            // 
            this.Preview.Location = new System.Drawing.Point(686, 479);
            this.Preview.Name = "Preview";
            this.Preview.Size = new System.Drawing.Size(100, 29);
            this.Preview.TabIndex = 3;
            this.Preview.Text = "Preview";
            this.Preview.UseVisualStyleBackColor = true;
            this.Preview.Click += new System.EventHandler(this.Preview_Click);
            // 
            // TrainerShowdownEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 520);
            this.Controls.Add(this.Preview);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.richTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "TrainerShowdownEditorForm";
            this.Text = "TrainerShowdownEditorForm";
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Preview;
    }
}