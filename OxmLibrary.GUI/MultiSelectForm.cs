using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;

namespace OxmLibrary.GUI
{
    public partial class MultiSelectForm : Form
    {
        private object[] items;

        private bool showAddNewButton;

        public event EventHandler AddNew;

        public event EventHandler<MultiSelectItemEventArgs> DeleteItem;

        public void OnAddNew()
        {
            if (AddNew != null)
                AddNew(this, EventArgs.Empty);
        }

        public void OnDeleteItem(List<object> itemToDelete)
        {
            if (DeleteItem != null)
                DeleteItem(this, new MultiSelectItemEventArgs(itemToDelete));
        }

        public bool ShowAddNewButton
        {
            get
            {
                return showAddNewButton;
            }
            set
            {
                showAddNewButton = value;
                AddnewBTN.Visible = value;
                listContextMenu.Enabled = value;
            }
        }

        public void DataBind(IEnumerable List, params string[] DisplayFields)
        {
            List<object> NewItems = new List<object>();
            List<PropertyInfo> Properties = null;
            itemsDiaplay.BeginUpdate();
            itemsDiaplay.Columns.Clear();
            itemsDiaplay.Items.Clear();
            var itemWidth = itemsDiaplay.Width / DisplayFields.Length;
            foreach (var item in DisplayFields)
            {
                itemsDiaplay.Columns.Add(item, itemWidth);
            }
            foreach (var item in List)
            {
                NewItems.Add(item);
                Properties = (from df in DisplayFields
                              join pi in item.GetType().GetProperties()
                              on df equals pi.Name
                              select pi).ToList();
                var values = (from prop in Properties
                              select prop.GetValue(item, null).ToString()).ToArray();
                ListViewItem lItem = new ListViewItem(values);
                itemsDiaplay.Items.Add(lItem);
            }
            itemsDiaplay.View = View.Details;
            itemsDiaplay.EndUpdate();
            items = NewItems.ToArray();

        }

        public string[] Items
        {
            get
            {
                return items.Select(a => (a is string) ? (string)a : a.ToString()).ToArray();
            }
            set
            {
                items = value;
                itemsDiaplay.Clear();
                itemsDiaplay.Items.AddRange(value.Select(a => new ListViewItem(new string[] { a })).ToArray());
                itemsDiaplay.View = View.SmallIcon;
            }
        }

        public string SelectedText
        {
            get
            {
                return SelectedItem.SubItems[0].Text;
            }
        }

        public ListViewItem SelectedItem
        {
            get;
            private set;
        }

        public MultiSelectForm(Form Parent)
        {
            InitializeComponent();
            ShowAddNewButton = false;
            this.Owner = Parent;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
            if (Owner != null)
                Owner.BringToFront();
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            SelectedItem = itemsDiaplay.SelectedItems[0];
            Hide();
        }

        private void AddnewBTN_Click(object sender, EventArgs e)
        {
            OnAddNew();   
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            if (itemsDiaplay.SelectedItems.Count > 0)
            {
                var toDelete = (from item in itemsDiaplay.SelectedItems.OfType<ListViewItem>()
                           select items[item.Index]).ToList();
                OnDeleteItem(toDelete);
            }
        }
    }
}
