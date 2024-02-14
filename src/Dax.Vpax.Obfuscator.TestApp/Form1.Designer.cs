namespace Dax.Vpax.Obfuscator.TestApp
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
            if (disposing && (components != null)) {
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
            buttonObfuscate = new Button();
            openVpaxFileDialog = new OpenFileDialog();
            checkBoxOverwriteDictionary = new CheckBox();
            buttonObfuscateDirectory = new Button();
            folderBrowserDialog1 = new FolderBrowserDialog();
            checkBoxIncrementalFileObfuscation = new CheckBox();
            openFileDialog1 = new OpenFileDialog();
            openDictionaryFileDialog = new OpenFileDialog();
            buttonDeobfuscate = new Button();
            SuspendLayout();
            // 
            // buttonObfuscate
            // 
            buttonObfuscate.Location = new Point(37, 155);
            buttonObfuscate.Name = "buttonObfuscate";
            buttonObfuscate.Size = new Size(133, 44);
            buttonObfuscate.TabIndex = 0;
            buttonObfuscate.Text = "Obfuscate File";
            buttonObfuscate.UseVisualStyleBackColor = true;
            buttonObfuscate.Click += buttonObfuscate_Click;
            // 
            // openVpaxFileDialog
            // 
            openVpaxFileDialog.DefaultExt = "*.vpax";
            openVpaxFileDialog.Filter = "VertiPaq-Analyzer file|*.vpax";
            // 
            // checkBoxOverwriteDictionary
            // 
            checkBoxOverwriteDictionary.AutoSize = true;
            checkBoxOverwriteDictionary.Location = new Point(37, 26);
            checkBoxOverwriteDictionary.Name = "checkBoxOverwriteDictionary";
            checkBoxOverwriteDictionary.Size = new Size(203, 19);
            checkBoxOverwriteDictionary.TabIndex = 1;
            checkBoxOverwriteDictionary.Text = "Allow overwrite output dictionary";
            checkBoxOverwriteDictionary.UseVisualStyleBackColor = true;
            // 
            // buttonObfuscateDirectory
            // 
            buttonObfuscateDirectory.Location = new Point(37, 82);
            buttonObfuscateDirectory.Name = "buttonObfuscateDirectory";
            buttonObfuscateDirectory.Size = new Size(133, 44);
            buttonObfuscateDirectory.TabIndex = 2;
            buttonObfuscateDirectory.Text = "Obfuscate Directory";
            buttonObfuscateDirectory.UseVisualStyleBackColor = true;
            buttonObfuscateDirectory.Click += buttonObfuscateDirectory_Click;
            // 
            // checkBoxIncrementalFileObfuscation
            // 
            checkBoxIncrementalFileObfuscation.AutoSize = true;
            checkBoxIncrementalFileObfuscation.Location = new Point(182, 169);
            checkBoxIncrementalFileObfuscation.Name = "checkBoxIncrementalFileObfuscation";
            checkBoxIncrementalFileObfuscation.Size = new Size(321, 19);
            checkBoxIncrementalFileObfuscation.TabIndex = 3;
            checkBoxIncrementalFileObfuscation.Text = "Incremental obfuscation (requires a previous dictionary)";
            checkBoxIncrementalFileObfuscation.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "*.vpax";
            openFileDialog1.Filter = "VertiPaq-Analyzer file|*.vpax";
            // 
            // openDictionaryFileDialog
            // 
            openDictionaryFileDialog.DefaultExt = "*.dictionary.json";
            openDictionaryFileDialog.Filter = "Obfucator Dictionary file|*.dictionary.json";
            // 
            // buttonDeobfuscate
            // 
            buttonDeobfuscate.Location = new Point(37, 231);
            buttonDeobfuscate.Name = "buttonDeobfuscate";
            buttonDeobfuscate.Size = new Size(133, 44);
            buttonDeobfuscate.TabIndex = 4;
            buttonDeobfuscate.Text = "Deobfuscate File";
            buttonDeobfuscate.UseVisualStyleBackColor = true;
            buttonDeobfuscate.Click += buttonDeobfuscate_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(515, 304);
            Controls.Add(buttonDeobfuscate);
            Controls.Add(checkBoxIncrementalFileObfuscation);
            Controls.Add(buttonObfuscateDirectory);
            Controls.Add(checkBoxOverwriteDictionary);
            Controls.Add(buttonObfuscate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonObfuscate;
        private OpenFileDialog openVpaxFileDialog;
        private CheckBox checkBoxOverwriteDictionary;
        private Button buttonObfuscateDirectory;
        private FolderBrowserDialog folderBrowserDialog1;
        private CheckBox checkBoxIncrementalFileObfuscation;
        private OpenFileDialog openFileDialog1;
        private OpenFileDialog openDictionaryFileDialog;
        private Button buttonDeobfuscate;
    }
}
