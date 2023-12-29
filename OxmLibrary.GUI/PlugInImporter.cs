using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxmLibrary;
using System.Windows.Forms;
using OxmLibrary.CodeGeneration;
using System.Threading;
using System.ComponentModel;

namespace OxmLibrary.GUI
{
    public class PlugInImporter
    {
        public virtual IOxmGeneratorPlugin PlugIn
        {
            get
            {
                return null;
            }
        }

        public event EventHandler ImportFinished;

        public void OnImportFinished()
        {
            if (ImportFinished != null)
                ImportFinished(this, EventArgs.Empty);
        }

        public virtual void ImportPlug(OpenFileDialog openGenFileDialog)
        {
        }

        public static PlugInImporter Create(Type getPlugin, CodeTemplateBase template, bool FromWeb)
        {
            var importer = typeof(PlugInImporter<>).MakeGenericType(getPlugin);
            var method = importer.GetMethod("Import", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var result = (PlugInImporter)method.Invoke(null, new object[] { template, FromWeb });
            return result;
        }
    }

    public class PlugInImporter<T> : PlugInImporter where T : IOxmGeneratorPlugin
    {
        private T plugin;
        private OxmGenerator3 generator;
        private bool importFromWeb;

        public override IOxmGeneratorPlugin PlugIn
        {
            get
            {
                return plugin;
            }
        }

        private PlugInImporter(T plugIn, bool fromWeb)
        {
            this.plugin = plugIn;
            this.importFromWeb = fromWeb;
        }

        public GenerationConfiguration Configuration
        {
            get
            {
                return generator.Config;
            }
        }

        public static PlugInImporter<T> Import(CodeTemplateBase template, bool fromWeb)
        {
            var plugin = Activator.CreateInstance<T>();
            var importer = new PlugInImporter<T>(plugin, fromWeb);
            importer.generator = OxmGenerator3.Fresh(template);
            return importer;
        }

        public override void ImportPlug(OpenFileDialog openGenFileDialog)
        {
            generator.AssignPlugin(plugin);
            try
            {
                if (plugin.RequiresExternalFile != PluginFileFormatSupport.NoFileRequired)
                {
                    if (importFromWeb)
                    {
                        if (!GetRemoteFile())
                            return;
                    }
                    else
                    {
                        if (!GetLocalFile(openGenFileDialog))
                            return;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error while accessing file");
                return;
            }

            var config = new ConfigurationPropertyForm(generator.Config);

            plugin.ModifyConfiguration(generator.Config);
            if (config.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            Action process = plugin.AfterConfigurationAction;
            var handle = process.BeginInvoke(Finished, null); //process.BeginInvoke(openGenFileDialog.FileName, Finished, null);
        }

        private bool GetRemoteFile()
        {
            var dialog = new TextBoxDialog();
            dialog.SetTitle("Enter url of web service...");
            if (dialog.ShowDialog() != DialogResult.OK)
                return false;
            plugin.ProcessFile(dialog.TextBoxText);
            return true;
        }

        private bool GetLocalFile(OpenFileDialog openGenFileDialog)
        {
            openGenFileDialog = new OpenFileDialog();
            openGenFileDialog.Filter = plugin.FileFilter;
            openGenFileDialog.AutoUpgradeEnabled = true;
            openGenFileDialog.CheckFileExists = false;
            openGenFileDialog.CheckPathExists = false;
            openGenFileDialog.Title = "Choose File To Import";

            if (openGenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            plugin.ProcessFile(openGenFileDialog.FileName);
            return true;
        }

        public void Finished(IAsyncResult result)
        {
            OnImportFinished();
        }
    }
}
