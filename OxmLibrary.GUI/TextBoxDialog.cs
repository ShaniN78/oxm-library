using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OxmLibrary.GUI
{
    public partial class TextBoxDialog : Form
    {
        public event EventHandler SelectedValueChanged;
        public string TextBoxText
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public void SetTitle(string title)
        {
            this.Text = title;
        }

        public TextBoxDialog()
        {
            InitializeComponent();            
        }

        private void cancelBTN_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();            
        }

        private void okBTN_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Hide();
            if (SelectedValueChanged != null)
                SelectedValueChanged(this, e);
        }

        private void TextBoxDialog_Shown(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                okBTN_Click(sender, e);
        }
    }
}
