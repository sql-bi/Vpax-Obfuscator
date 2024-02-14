using System.Reflection;
using Dax.Vpax.Obfuscator.Common;

namespace Dax.Vpax.Obfuscator.TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Text = $"VPAX Obfuscator v{typeof(VpaxObfuscator).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}";
        }

        private void buttonObfuscate_Click(object sender, EventArgs e)
        {
            if (openVpaxFileDialog.ShowDialog() != DialogResult.OK) return;
            if (checkBoxIncrementalFileObfuscation.Checked && openDictionaryFileDialog.ShowDialog() != DialogResult.OK) return;

            folderBrowserDialog1.Description = "Select the output folder for the obfuscated VPAX file.";
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;

            buttonObfuscate.Enabled = false;
            try
            {
                var dictionary = checkBoxIncrementalFileObfuscation.Checked ? new FileInfo(openDictionaryFileDialog.FileName) : null;
                var vpax = new FileInfo(openVpaxFileDialog.FileName);

                Obfuscate(vpax, dictionary, outputPath: folderBrowserDialog1.SelectedPath);
                MessageBox.Show("Obfuscation completed.", "Obfuscation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Obfuscation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonObfuscate.Enabled = true;
            }
        }

        private void buttonObfuscateDirectory_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Select the input folder containing VPAX files to obfuscate.";
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;
            var inputPath = folderBrowserDialog1.SelectedPath;

            folderBrowserDialog1.Description = "Select the output folder for the obfuscated VPAX files.";
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;
            var outputPath = folderBrowserDialog1.SelectedPath;

            buttonObfuscateDirectory.Enabled = false;
            try
            {
                foreach (var vpax in new DirectoryInfo(inputPath).GetFiles("*.vpax"))
                    Obfuscate(vpax, dictionaryFile: null, outputPath);

                MessageBox.Show("Obfuscation completed.", "Obfuscation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Obfuscation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonObfuscateDirectory.Enabled = true;
            }
        }

        private void Obfuscate(FileInfo vpaxFile, FileInfo? dictionaryFile, string outputPath)
        {
            var data = File.ReadAllBytes(vpaxFile.FullName);
            using var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);

            var obfuscator = new VpaxObfuscator();
            var dictionary = dictionaryFile != null
                ? obfuscator.Obfuscate(stream, ObfuscationDictionary.ReadFrom(dictionaryFile.FullName))
                : obfuscator.Obfuscate(stream);

            var dictionaryPath = Path.Combine(outputPath, Path.ChangeExtension(vpaxFile.Name, ".dictionary.json"));
            var vpaxPath = Path.Combine(outputPath, Path.ChangeExtension(vpaxFile.Name, ".obfuscated.vpax"));

            dictionary.WriteTo(dictionaryPath, overwrite: checkBoxOverwriteDictionary.Checked, indented: true);
            File.WriteAllBytes(vpaxPath, stream.ToArray());
        }

        private void buttonDeobfuscate_Click(object sender, EventArgs e)
        {
            if (openVpaxFileDialog.ShowDialog() != DialogResult.OK) return;
            if (openDictionaryFileDialog.ShowDialog() != DialogResult.OK) return;

            buttonDeobfuscate.Enabled = false;
            try
            {
                var dictionary = new FileInfo(openDictionaryFileDialog.FileName);
                var vpax = new FileInfo(openVpaxFileDialog.FileName);

                Deobfuscate(vpax, dictionary);
                MessageBox.Show("Deobfuscation completed.", "Deobfuscation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Deobfuscation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonDeobfuscate.Enabled = true;
            }
        }

        private void Deobfuscate(FileInfo vpaxFile, FileInfo dictionaryFile)
        {
            var data = File.ReadAllBytes(vpaxFile.FullName);
            using var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);

            var obfuscator = new VpaxObfuscator();
            obfuscator.Deobfuscate(stream, ObfuscationDictionary.ReadFrom(dictionaryFile.FullName));

            var vpaxPath = Path.Combine(vpaxFile.DirectoryName!, Path.ChangeExtension(vpaxFile.Name, ".deobfuscated.vpax"));
            File.WriteAllBytes(vpaxPath, stream.ToArray());
        }
    }
}
