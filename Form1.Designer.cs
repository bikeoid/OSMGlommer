namespace OSMGlommer
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbFilename = new System.Windows.Forms.TextBox();
            this.btBrowseFolder = new System.Windows.Forms.Button();
            this.richTextLogBox = new System.Windows.Forms.RichTextBox();
            this.btGlom = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "OSM File:";
            // 
            // tbFilename
            // 
            this.tbFilename.Location = new System.Drawing.Point(93, 31);
            this.tbFilename.Name = "tbFilename";
            this.tbFilename.Size = new System.Drawing.Size(450, 23);
            this.tbFilename.TabIndex = 1;
            // 
            // btBrowseFolder
            // 
            this.btBrowseFolder.Location = new System.Drawing.Point(564, 27);
            this.btBrowseFolder.Name = "btBrowseFolder";
            this.btBrowseFolder.Size = new System.Drawing.Size(134, 36);
            this.btBrowseFolder.TabIndex = 2;
            this.btBrowseFolder.Text = "Browse";
            this.btBrowseFolder.UseVisualStyleBackColor = true;
            this.btBrowseFolder.Click += new System.EventHandler(this.btBrowseFolder_Click);
            // 
            // richTextLogBox
            // 
            this.richTextLogBox.Location = new System.Drawing.Point(44, 95);
            this.richTextLogBox.Name = "richTextLogBox";
            this.richTextLogBox.Size = new System.Drawing.Size(677, 243);
            this.richTextLogBox.TabIndex = 3;
            this.richTextLogBox.Text = "";
            // 
            // btGlom
            // 
            this.btGlom.Location = new System.Drawing.Point(536, 366);
            this.btGlom.Name = "btGlom";
            this.btGlom.Size = new System.Drawing.Size(103, 39);
            this.btGlom.TabIndex = 4;
            this.btGlom.Text = "Glom";
            this.btGlom.UseVisualStyleBackColor = true;
            this.btGlom.Click += new System.EventHandler(this.btGlom_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "(Wild cards OK)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btGlom);
            this.Controls.Add(this.richTextLogBox);
            this.Controls.Add(this.btBrowseFolder);
            this.Controls.Add(this.tbFilename);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "OSM Glommer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbFilename;
        private System.Windows.Forms.Button btBrowseFolder;
        private System.Windows.Forms.RichTextBox richTextLogBox;
        private System.Windows.Forms.Button btGlom;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label2;
    }
}

