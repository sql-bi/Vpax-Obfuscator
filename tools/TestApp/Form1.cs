using System.Reflection;
using Dax.Vpax.Obfuscator;
using Dax.Vpax.Obfuscator.Common;

namespace TestApp
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
            openVpaxFileDialog.Filter = "VPAX files (*.vpax)|*.vpax|All files (*.*)|*.*";

            if (openVpaxFileDialog.ShowDialog() != DialogResult.OK) return;
            if (checkBoxIncrementalFileObfuscation.Checked && openDictionaryFileDialog.ShowDialog() != DialogResult.OK) return;

            folderBrowserDialog1.Description = "Select the output folder for the obfuscated VPAX file.";
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;

            buttonObfuscate.Enabled = false;
            try
            {
                var dictionary = checkBoxIncrementalFileObfuscation.Checked ? new FileInfo(openDictionaryFileDialog.FileName) : null;
                var vpax = new FileInfo(openVpaxFileDialog.FileName);
                var outputPath = folderBrowserDialog1.SelectedPath;
                var overwrite = checkBoxOverwriteDictionary.Checked;
                var trackUnobfuscated = checkBoxTrackUnobfuscated.Checked;

                Obfuscate(vpax, dictionary, outputPath, overwrite, trackUnobfuscated);
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
                var overwrite = checkBoxOverwriteDictionary.Checked;
                var trackUnobfuscated = checkBoxTrackUnobfuscated.Checked;
                foreach (var vpax in new DirectoryInfo(inputPath).GetFiles("*.vpax"))
                    Obfuscate(vpax, dictionaryFile: null, outputPath, overwrite, trackUnobfuscated);

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

        private void buttonDeobfuscate_Click(object sender, EventArgs e)
        {
            openVpaxFileDialog.Filter = "OVPAX files (*.ovpax)|*.ovpax|All files (*.*)|*.*";

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

        private static void Obfuscate(FileInfo vpaxFile, FileInfo? dictionaryFile, string outputPath, bool overwrite, bool trackUnobfuscated)
        {
            var data = File.ReadAllBytes(vpaxFile.FullName);
            using var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);

            var options = new ObfuscationOptions { TrackUnobfuscated = trackUnobfuscated };
            var obfuscator = new VpaxObfuscator();
            var dictionary = dictionaryFile != null
                ? obfuscator.Obfuscate(stream, ObfuscationDictionary.ReadFrom(dictionaryFile.FullName))
                : obfuscator.Obfuscate(stream);

            var dictionaryPath = Path.Combine(outputPath, Path.ChangeExtension(vpaxFile.Name, ".vpax.dict"));
            var ovpaxPath = Path.Combine(outputPath, Path.ChangeExtension(vpaxFile.Name, ".ovpax"));

            dictionary.WriteTo(dictionaryPath, overwrite, indented: true);
            File.WriteAllBytes(ovpaxPath, stream.ToArray());
        }

        private static void Deobfuscate(FileInfo ovpaxFile, FileInfo dictionaryFile)
        {
            var data = File.ReadAllBytes(ovpaxFile.FullName);
            using var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);

            var obfuscator = new VpaxObfuscator();
            obfuscator.Deobfuscate(stream, ObfuscationDictionary.ReadFrom(dictionaryFile.FullName));

            var vpaxPath = Path.Combine(ovpaxFile.DirectoryName!, Path.ChangeExtension(ovpaxFile.Name, ".deobfuscated.vpax"));
            File.WriteAllBytes(vpaxPath, stream.ToArray());
        }

        private void buttonExtractVpax_Click(object sender, EventArgs e)
        {
            var connectionString = textBoxConnectionString.Text;

            var model = Dax.Model.Extractor.TomExtractor.GetDaxModel(connectionString, applicationName: "ObfuscatorTest", applicationVersion: "0.0.0");

            //var database = Dax.Model.Extractor.TomExtractor.GetDatabase(serverName, databaseName);
            //var viewVpa = new Dax.ViewVpaExport.Model(model);

            saveFileDialog1.Title = "Select the output VPAX file.";
            saveFileDialog1.Filter = "VPAX files (*.vpax)|*.vpax|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            var path = saveFileDialog1.FileName;
            Dax.Vpax.Tools.VpaxTools.ExportVpax(path, model, viewVpa: null, database: null);
        }
    }
}
