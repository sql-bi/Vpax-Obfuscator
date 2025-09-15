namespace TestApp
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
            openVpaxFileDialog = new OpenFileDialog();
            folderBrowserDialog1 = new FolderBrowserDialog();
            openFileDialog1 = new OpenFileDialog();
            openDictionaryFileDialog = new OpenFileDialog();
            groupBox1 = new GroupBox();
            checkBoxTrackUnobfuscated = new CheckBox();
            buttonDeobfuscate = new Button();
            checkBoxIncrementalFileObfuscation = new CheckBox();
            buttonObfuscateDirectory = new Button();
            checkBoxOverwriteDictionary = new CheckBox();
            buttonObfuscate = new Button();
            groupBox2 = new GroupBox();
            label1 = new Label();
            textBoxConnectionString = new TextBox();
            buttonExtractVpax = new Button();
            saveFileDialog1 = new SaveFileDialog();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // openVpaxFileDialog
            // 
            openVpaxFileDialog.DefaultExt = "*.vpax";
            openVpaxFileDialog.Filter = "VertiPaq-Analyzer file|*.vpax";
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "*.vpax";
            openFileDialog1.Filter = "VertiPaq-Analyzer file|*.vpax";
            // 
            // openDictionaryFileDialog
            // 
            openDictionaryFileDialog.DefaultExt = "*.vpax.dict";
            openDictionaryFileDialog.Filter = "Obfucator Dictionary file|*.vpax.dict";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBoxTrackUnobfuscated);
            groupBox1.Controls.Add(buttonDeobfuscate);
            groupBox1.Controls.Add(checkBoxIncrementalFileObfuscation);
            groupBox1.Controls.Add(buttonObfuscateDirectory);
            groupBox1.Controls.Add(checkBoxOverwriteDictionary);
            groupBox1.Controls.Add(buttonObfuscate);
            groupBox1.Dock = DockStyle.Bottom;
            groupBox1.Location = new Point(0, 127);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(611, 242);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Obfuscator";
            // 
            // checkBoxTrackUnobfuscated
            // 
            checkBoxTrackUnobfuscated.AutoSize = true;
            checkBoxTrackUnobfuscated.Checked = true;
            checkBoxTrackUnobfuscated.CheckState = CheckState.Checked;
            checkBoxTrackUnobfuscated.Location = new Point(26, 25);
            checkBoxTrackUnobfuscated.Name = "checkBoxTrackUnobfuscated";
            checkBoxTrackUnobfuscated.Size = new Size(246, 19);
            checkBoxTrackUnobfuscated.TabIndex = 11;
            checkBoxTrackUnobfuscated.Text = "Include unobfuscated values in dictionary";
            checkBoxTrackUnobfuscated.UseVisualStyleBackColor = true;
            // 
            // buttonDeobfuscate
            // 
            buttonDeobfuscate.Location = new Point(26, 187);
            buttonDeobfuscate.Name = "buttonDeobfuscate";
            buttonDeobfuscate.Size = new Size(133, 44);
            buttonDeobfuscate.TabIndex = 10;
            buttonDeobfuscate.Text = "Deobfuscate File";
            buttonDeobfuscate.UseVisualStyleBackColor = true;
            buttonDeobfuscate.Click += buttonDeobfuscate_Click;
            // 
            // checkBoxIncrementalFileObfuscation
            // 
            checkBoxIncrementalFileObfuscation.AutoSize = true;
            checkBoxIncrementalFileObfuscation.Location = new Point(165, 151);
            checkBoxIncrementalFileObfuscation.Name = "checkBoxIncrementalFileObfuscation";
            checkBoxIncrementalFileObfuscation.Size = new Size(321, 19);
            checkBoxIncrementalFileObfuscation.TabIndex = 9;
            checkBoxIncrementalFileObfuscation.Text = "Incremental obfuscation (requires a previous dictionary)";
            checkBoxIncrementalFileObfuscation.UseVisualStyleBackColor = true;
            // 
            // buttonObfuscateDirectory
            // 
            buttonObfuscateDirectory.Location = new Point(26, 87);
            buttonObfuscateDirectory.Name = "buttonObfuscateDirectory";
            buttonObfuscateDirectory.Size = new Size(133, 44);
            buttonObfuscateDirectory.TabIndex = 8;
            buttonObfuscateDirectory.Text = "Obfuscate Directory";
            buttonObfuscateDirectory.UseVisualStyleBackColor = true;
            buttonObfuscateDirectory.Click += buttonObfuscateDirectory_Click;
            // 
            // checkBoxOverwriteDictionary
            // 
            checkBoxOverwriteDictionary.AutoSize = true;
            checkBoxOverwriteDictionary.Location = new Point(26, 50);
            checkBoxOverwriteDictionary.Name = "checkBoxOverwriteDictionary";
            checkBoxOverwriteDictionary.Size = new Size(203, 19);
            checkBoxOverwriteDictionary.TabIndex = 7;
            checkBoxOverwriteDictionary.Text = "Allow overwrite output dictionary";
            checkBoxOverwriteDictionary.UseVisualStyleBackColor = true;
            // 
            // buttonObfuscate
            // 
            buttonObfuscate.Location = new Point(26, 137);
            buttonObfuscate.Name = "buttonObfuscate";
            buttonObfuscate.Size = new Size(133, 44);
            buttonObfuscate.TabIndex = 6;
            buttonObfuscate.Text = "Obfuscate File";
            buttonObfuscate.UseVisualStyleBackColor = true;
            buttonObfuscate.Click += buttonObfuscate_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(textBoxConnectionString);
            groupBox2.Controls.Add(buttonExtractVpax);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(0, 0);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(611, 127);
            groupBox2.TabIndex = 8;
            groupBox2.TabStop = false;
            groupBox2.Text = "Vpax Tools";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(26, 73);
            label1.Name = "label1";
            label1.Size = new Size(103, 15);
            label1.TabIndex = 9;
            label1.Text = "Connection String";
            // 
            // textBoxConnectionString
            // 
            textBoxConnectionString.Location = new Point(26, 91);
            textBoxConnectionString.Name = "textBoxConnectionString";
            textBoxConnectionString.Size = new Size(563, 23);
            textBoxConnectionString.TabIndex = 8;
            textBoxConnectionString.Text = "Provider=MSOLAP;Data Source=localhost:55607;Initial Catalog=07025879-19dc-4f4f-9cf2-4d66fbb25344;";
            // 
            // buttonExtractVpax
            // 
            buttonExtractVpax.Location = new Point(26, 22);
            buttonExtractVpax.Name = "buttonExtractVpax";
            buttonExtractVpax.Size = new Size(133, 44);
            buttonExtractVpax.TabIndex = 7;
            buttonExtractVpax.Text = "Extract Vpax";
            buttonExtractVpax.UseVisualStyleBackColor = true;
            buttonExtractVpax.Click += buttonExtractVpax_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(611, 369);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private OpenFileDialog openVpaxFileDialog;
        private FolderBrowserDialog folderBrowserDialog1;
        private OpenFileDialog openFileDialog1;
        private OpenFileDialog openDictionaryFileDialog;
        private GroupBox groupBox1;
        private CheckBox checkBoxTrackUnobfuscated;
        private Button buttonDeobfuscate;
        private CheckBox checkBoxIncrementalFileObfuscation;
        private Button buttonObfuscateDirectory;
        private CheckBox checkBoxOverwriteDictionary;
        private Button buttonObfuscate;
        private GroupBox groupBox2;
        private Label label1;
        private TextBox textBoxConnectionString;
        private Button buttonExtractVpax;
        private SaveFileDialog saveFileDialog1;
    }
}
