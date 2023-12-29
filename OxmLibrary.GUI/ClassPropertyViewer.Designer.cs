using System.Windows.Forms;
namespace OxmLibrary.GUI
{
    partial class ClassPropertyViewer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.propertiesView = new System.Windows.Forms.DataGridView();
            this.pNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pMaxCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.defaultValueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isAttributeDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.overRideNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.propertyDescriptorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.PropertyNameCell = new System.Windows.Forms.DataGridViewColumn();
            this.DataTypeCell = new System.Windows.Forms.DataGridViewColumn();
            this.countCLM = new System.Windows.Forms.DataGridViewColumn();
            this.overRideCLM = new System.Windows.Forms.DataGridViewColumn();
            this.editPropMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeArrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jumpToClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseDataTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inspectEnumerationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeAllDataTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.propertiesView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.propertyDescriptorBindingSource)).BeginInit();
            this.editPropMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertiesView
            // 
            this.propertiesView.AllowUserToAddRows = false;
            this.propertiesView.AllowUserToDeleteRows = false;
            this.propertiesView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DeepSkyBlue;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.LightSkyBlue;
            this.propertiesView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.propertiesView.AutoGenerateColumns = false;
            this.propertiesView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.propertiesView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.propertiesView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.propertiesView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.AppWorkspace;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe Condensed", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.propertiesView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.propertiesView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.pNameDataGridViewTextBoxColumn,
            this.pMaxCountDataGridViewTextBoxColumn,
            this.defaultValueDataGridViewTextBoxColumn,
            this.isAttributeDataGridViewCheckBoxColumn,
            this.pTypeDataGridViewTextBoxColumn,
            this.overRideNameDataGridViewTextBoxColumn});
            this.propertiesView.DataSource = this.propertyDescriptorBindingSource;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Lime;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.propertiesView.DefaultCellStyle = dataGridViewCellStyle3;
            this.propertiesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesView.Location = new System.Drawing.Point(0, 0);
            this.propertiesView.MultiSelect = false;
            this.propertiesView.Name = "propertiesView";
            this.propertiesView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.propertiesView.RowHeadersVisible = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.DeepSkyBlue;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.LightSkyBlue;
            this.propertiesView.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.propertiesView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.propertiesView.Size = new System.Drawing.Size(595, 376);
            this.propertiesView.TabIndex = 1;
            this.propertiesView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.propertiesView_MouseClick);
            this.propertiesView.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.propertiesView_PreviewKeyDown);
            this.propertiesView.SelectionChanged += new System.EventHandler(this.propertiesView_SelectionChanged);
            // 
            // pNameDataGridViewTextBoxColumn
            // 
            this.pNameDataGridViewTextBoxColumn.DataPropertyName = "PName";
            this.pNameDataGridViewTextBoxColumn.HeaderText = "Property Name";
            this.pNameDataGridViewTextBoxColumn.Name = "pNameDataGridViewTextBoxColumn";
            // 
            // pMaxCountDataGridViewTextBoxColumn
            // 
            this.pMaxCountDataGridViewTextBoxColumn.DataPropertyName = "PMaxCount";
            this.pMaxCountDataGridViewTextBoxColumn.HeaderText = "Count";
            this.pMaxCountDataGridViewTextBoxColumn.Name = "pMaxCountDataGridViewTextBoxColumn";
            // 
            // defaultValueDataGridViewTextBoxColumn
            // 
            this.defaultValueDataGridViewTextBoxColumn.DataPropertyName = "DefaultValue";
            this.defaultValueDataGridViewTextBoxColumn.HeaderText = "Default Value";
            this.defaultValueDataGridViewTextBoxColumn.Name = "defaultValueDataGridViewTextBoxColumn";
            // 
            // isAttributeDataGridViewCheckBoxColumn
            // 
            this.isAttributeDataGridViewCheckBoxColumn.DataPropertyName = "IsAttribute";
            this.isAttributeDataGridViewCheckBoxColumn.HeaderText = "Is Attribute";
            this.isAttributeDataGridViewCheckBoxColumn.Name = "isAttributeDataGridViewCheckBoxColumn";
            // 
            // pTypeDataGridViewTextBoxColumn
            // 
            this.pTypeDataGridViewTextBoxColumn.DataPropertyName = "PType";
            this.pTypeDataGridViewTextBoxColumn.HeaderText = "Property Type";
            this.pTypeDataGridViewTextBoxColumn.Name = "pTypeDataGridViewTextBoxColumn";
            this.pTypeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // overRideNameDataGridViewTextBoxColumn
            // 
            this.overRideNameDataGridViewTextBoxColumn.DataPropertyName = "OverRideName";
            this.overRideNameDataGridViewTextBoxColumn.HeaderText = "OverRideName";
            this.overRideNameDataGridViewTextBoxColumn.Name = "overRideNameDataGridViewTextBoxColumn";
            // 
            // propertyDescriptorBindingSource
            // 
            this.propertyDescriptorBindingSource.DataSource = typeof(OxmLibrary.CodeGeneration.PropertyDescriptor);
            // 
            // PropertyNameCell
            // 
            this.PropertyNameCell.HeaderText = "Propery Name";
            this.PropertyNameCell.Name = "PropertyNameCell";
            this.PropertyNameCell.Width = 113;
            // 
            // DataTypeCell
            // 
            this.DataTypeCell.HeaderText = "DataType";
            this.DataTypeCell.Name = "DataTypeCell";
            this.DataTypeCell.ReadOnly = true;
            this.DataTypeCell.Width = 64;
            // 
            // countCLM
            // 
            this.countCLM.HeaderText = "Count";
            this.countCLM.Name = "countCLM";
            this.countCLM.ReadOnly = true;
            // 
            // overRideCLM
            // 
            this.overRideCLM.HeaderText = "Over Ride Name";
            this.overRideCLM.Name = "overRideCLM";
            this.overRideCLM.ReadOnly = true;
            this.overRideCLM.Width = 97;
            // 
            // editPropMenu
            // 
            this.editPropMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.makeArrayToolStripMenuItem,
            this.jumpToClassToolStripMenuItem,
            this.chooseDataTypeToolStripMenuItem,
            this.inspectEnumerationToolStripMenuItem,
            this.changeAllDataTypesToolStripMenuItem});
            this.editPropMenu.Name = "editPropMenu";
            this.editPropMenu.Size = new System.Drawing.Size(194, 136);
            this.editPropMenu.Text = "Edit Properties";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // makeArrayToolStripMenuItem
            // 
            this.makeArrayToolStripMenuItem.Name = "makeArrayToolStripMenuItem";
            this.makeArrayToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.makeArrayToolStripMenuItem.Text = "Make Array";
            this.makeArrayToolStripMenuItem.Click += new System.EventHandler(this.makeArrayToolStripMenuItem_Click);
            // 
            // jumpToClassToolStripMenuItem
            // 
            this.jumpToClassToolStripMenuItem.Name = "jumpToClassToolStripMenuItem";
            this.jumpToClassToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.jumpToClassToolStripMenuItem.Text = "Jump To Class";
            this.jumpToClassToolStripMenuItem.Click += new System.EventHandler(this.jumpToClassToolStripMenuItem_Click);
            // 
            // chooseDataTypeToolStripMenuItem
            // 
            this.chooseDataTypeToolStripMenuItem.Name = "chooseDataTypeToolStripMenuItem";
            this.chooseDataTypeToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.chooseDataTypeToolStripMenuItem.Text = "Choose Data Type";
            this.chooseDataTypeToolStripMenuItem.Click += new System.EventHandler(this.chooseDataTypeToolStripMenuItem_Click);
            // 
            // inspectEnumerationToolStripMenuItem
            // 
            this.inspectEnumerationToolStripMenuItem.Name = "inspectEnumerationToolStripMenuItem";
            this.inspectEnumerationToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.inspectEnumerationToolStripMenuItem.Text = "Inspect Enumeration";
            this.inspectEnumerationToolStripMenuItem.Click += new System.EventHandler(this.inspectEnumerationToolStripMenuItem_Click);
            // 
            // changeAllDataTypesToolStripMenuItem
            // 
            this.changeAllDataTypesToolStripMenuItem.Name = "changeAllDataTypesToolStripMenuItem";
            this.changeAllDataTypesToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.changeAllDataTypesToolStripMenuItem.Text = "Change All Data Types";
            this.changeAllDataTypesToolStripMenuItem.Click += new System.EventHandler(this.changeAllDataTypesToolStripMenuItem_Click);
            // 
            // ClassPropertyViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propertiesView);
            this.ForeColor = System.Drawing.Color.Lime;
            this.Name = "ClassPropertyViewer";
            this.Size = new System.Drawing.Size(595, 376);
            ((System.ComponentModel.ISupportInitialize)(this.propertiesView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.propertyDescriptorBindingSource)).EndInit();
            this.editPropMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView propertiesView;
        private System.Windows.Forms.DataGridViewColumn newPropName;
        private System.Windows.Forms.DataGridViewColumn dataType;
        private System.Windows.Forms.DataGridViewColumn countCLM;
        private System.Windows.Forms.DataGridViewColumn overRideCLM;
        private System.Windows.Forms.ContextMenuStrip editPropMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeArrayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jumpToClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseDataTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inspectEnumerationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeAllDataTypesToolStripMenuItem;
        private BindingSource propertyDescriptorBindingSource;
        private DataGridViewColumn PropertyNameCell;
        private DataGridViewColumn DataTypeCell;
        private DataGridViewTextBoxColumn pNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn pMaxCountDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn defaultValueDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn isAttributeDataGridViewCheckBoxColumn;
        private DataGridViewTextBoxColumn pTypeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn overRideNameDataGridViewTextBoxColumn;

    }
}
