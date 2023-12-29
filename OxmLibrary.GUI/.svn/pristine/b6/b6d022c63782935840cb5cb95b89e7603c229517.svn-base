using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OxmLibrary.CodeGeneration;

namespace OxmLibrary.GUI
{
    public partial class AdditionalsEditor : Form
    {
        private int y = 10;
        private Dictionary<Control, GenerationAdditionalParameters> values;
        public AdditionalsEditor()
        {
            InitializeComponent();
            values = new Dictionary<Control, GenerationAdditionalParameters>();
        }

        public void AddControl(GenerationAdditionalParameters collect)
        {
            
            Label k = new Label { Text = collect.DisplayName, Location = new Point(10, y + 10), Size = new Size(180, 30) };
            this.Controls.Add(k);
            if (collect.PropertyType == typeof(string))
            {
                TextBox text = new TextBox { Location = new Point(250, y), Size = new Size(200, 30) };
                Binding textBind = new Binding("Text", collect, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
                text.DataBindings.Add(textBind);
                this.Controls.Add(text);
            }
            else if (collect.PropertyType == typeof(bool))
            {
                CheckBox check = new CheckBox { Location = new Point(250, y), Size = new Size(30, 30)};
                check.Checked = collect.GetValue<bool>();
                this.Controls.Add(check);
                values.Add(check, collect);
            }
            y += 30;
        }

        public void AddRange(IList<GenerationAdditionalParameters> iList)
        {
            foreach (var param in iList)
            {
                AddControl(param);
            }
        }

        private void OkBTN_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            UnBind();
            this.Close();
        }

        private void UnBind()
        {
            foreach (var item in values)
            {
                if (item.Value.PropertyType == typeof(bool))
                {
                    item.Value.Value = ((CheckBox)item.Key).Checked.ToString();
                }
            }
        }
    }
}
