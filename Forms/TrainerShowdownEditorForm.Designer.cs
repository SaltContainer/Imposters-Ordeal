using System;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainerShowdownEditorForm));
            rtxtInput = new System.Windows.Forms.RichTextBox();
            rtxtPreview = new System.Windows.Forms.RichTextBox();
            btnSave = new System.Windows.Forms.Button();
            btnPreview = new System.Windows.Forms.Button();
            panelText = new System.Windows.Forms.TableLayoutPanel();
            panelButtons = new System.Windows.Forms.TableLayoutPanel();
            panelText.SuspendLayout();
            panelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // rtxtInput
            // 
            rtxtInput.DetectUrls = false;
            rtxtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            rtxtInput.Location = new System.Drawing.Point(3, 2);
            rtxtInput.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            rtxtInput.Name = "rtxtInput";
            rtxtInput.Size = new System.Drawing.Size(351, 281);
            rtxtInput.TabIndex = 0;
            rtxtInput.Text = "Paste Showdown Here";
            rtxtInput.WordWrap = false;
            rtxtInput.TextChanged += richTextBox1_TextChanged;
            // 
            // rtxtPreview
            // 
            rtxtPreview.DetectUrls = false;
            rtxtPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            rtxtPreview.Location = new System.Drawing.Point(360, 2);
            rtxtPreview.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            rtxtPreview.Name = "rtxtPreview";
            rtxtPreview.ReadOnly = true;
            rtxtPreview.Size = new System.Drawing.Size(351, 281);
            rtxtPreview.TabIndex = 1;
            rtxtPreview.Text = "Preview";
            rtxtPreview.WordWrap = false;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(3, 2);
            btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(88, 22);
            btnSave.TabIndex = 2;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += Save_Click;
            // 
            // btnPreview
            // 
            btnPreview.Location = new System.Drawing.Point(97, 2);
            btnPreview.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btnPreview.Name = "btnPreview";
            btnPreview.Size = new System.Drawing.Size(88, 22);
            btnPreview.TabIndex = 3;
            btnPreview.Text = "Preview";
            btnPreview.UseVisualStyleBackColor = true;
            btnPreview.Click += Preview_Click;
            // 
            // panelText
            // 
            panelText.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelText.ColumnCount = 2;
            panelText.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            panelText.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            panelText.Controls.Add(rtxtInput, 0, 0);
            panelText.Controls.Add(rtxtPreview, 1, 0);
            panelText.Location = new System.Drawing.Point(8, 7);
            panelText.Margin = new System.Windows.Forms.Padding(0);
            panelText.Name = "panelText";
            panelText.RowCount = 1;
            panelText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            panelText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 285F));
            panelText.Size = new System.Drawing.Size(714, 285);
            panelText.TabIndex = 4;
            // 
            // panelButtons
            // 
            panelButtons.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            panelButtons.AutoSize = true;
            panelButtons.ColumnCount = 2;
            panelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            panelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            panelButtons.Controls.Add(btnSave, 0, 0);
            panelButtons.Controls.Add(btnPreview, 1, 0);
            panelButtons.Location = new System.Drawing.Point(534, 292);
            panelButtons.Margin = new System.Windows.Forms.Padding(0);
            panelButtons.Name = "panelButtons";
            panelButtons.RowCount = 1;
            panelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            panelButtons.Size = new System.Drawing.Size(188, 26);
            panelButtons.TabIndex = 5;
            // 
            // TrainerShowdownEditorForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(730, 325);
            Controls.Add(panelButtons);
            Controls.Add(panelText);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TrainerShowdownEditorForm";
            Text = "TrainerShowdownEditorForm";
            Load += OnLoad;
            panelText.ResumeLayout(false);
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxtInput;
        private System.Windows.Forms.RichTextBox rtxtPreview;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.TableLayoutPanel panelText;
        private System.Windows.Forms.TableLayoutPanel panelButtons;
    }
}