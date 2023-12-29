using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OxmLibrary;
using OxmLibrary.CodeGeneration;

namespace OxmLibrary.GUI
{
    public partial class ConfigurationPropertyForm : Form
    {
        private GenerationConfiguration currentConfig;


        public ConfigurationPropertyForm(GenerationConfiguration config)
        {
            InitializeComponent();

            currentConfig = config;            
        }

        private void glassButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ConfigurationPropertyForm_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = currentConfig;
        }
    }
}
