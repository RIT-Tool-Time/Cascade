//-----------------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

namespace MultiControls
{
    partial class MainForm
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
            this.touchManipulationEdit2 = new MultiControls.TouchManipulationEdit();
            this.touchManipulationEdit1 = new MultiControls.TouchManipulationEdit();
            this.touchEdit2 = new MultiControls.TouchGestureEdit();
            this.touchEdit1 = new MultiControls.TouchGestureEdit();
            this.SuspendLayout();
            // 
            // touchManipulationEdit2
            // 
            this.touchManipulationEdit2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.touchManipulationEdit2.Location = new System.Drawing.Point(23, 269);
            this.touchManipulationEdit2.Name = "touchManipulationEdit2";
            this.touchManipulationEdit2.Size = new System.Drawing.Size(452, 62);
            this.touchManipulationEdit2.TabIndex = 3;
            this.touchManipulationEdit2.Text = "0";
            // 
            // touchManipulationEdit1
            // 
            this.touchManipulationEdit1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.touchManipulationEdit1.Location = new System.Drawing.Point(23, 189);
            this.touchManipulationEdit1.Name = "touchManipulationEdit1";
            this.touchManipulationEdit1.Size = new System.Drawing.Size(452, 62);
            this.touchManipulationEdit1.TabIndex = 2;
            this.touchManipulationEdit1.Text = "0";
            // 
            // touchEdit2
            // 
            this.touchEdit2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.touchEdit2.Location = new System.Drawing.Point(23, 110);
            this.touchEdit2.Margin = new System.Windows.Forms.Padding(14, 13, 14, 13);
            this.touchEdit2.Name = "touchEdit2";
            this.touchEdit2.Size = new System.Drawing.Size(452, 62);
            this.touchEdit2.TabIndex = 1;
            this.touchEdit2.Text = "0";
            // 
            // touchEdit1
            // 
            this.touchEdit1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.touchEdit1.Location = new System.Drawing.Point(23, 22);
            this.touchEdit1.Margin = new System.Windows.Forms.Padding(14, 13, 14, 13);
            this.touchEdit1.Name = "touchEdit1";
            this.touchEdit1.Size = new System.Drawing.Size(452, 62);
            this.touchEdit1.TabIndex = 0;
            this.touchEdit1.Text = "0";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(28F, 55F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 343);
            this.Controls.Add(this.touchManipulationEdit2);
            this.Controls.Add(this.touchManipulationEdit1);
            this.Controls.Add(this.touchEdit2);
            this.Controls.Add(this.touchEdit1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.Margin = new System.Windows.Forms.Padding(14, 13, 14, 13);
            this.Name = "MainForm";
            this.Text = "MultiControls";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TouchGestureEdit touchEdit1;
        private TouchGestureEdit touchEdit2;
        private TouchManipulationEdit touchManipulationEdit1;
        private TouchManipulationEdit touchManipulationEdit2;
    }
}