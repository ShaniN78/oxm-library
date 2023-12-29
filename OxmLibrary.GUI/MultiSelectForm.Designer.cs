using Glass;
namespace OxmLibrary.GUI
{
    partial class MultiSelectForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.itemsDiaplay = new System.Windows.Forms.ListView();
            this.selectFrom = new System.Windows.Forms.ColumnHeader();
            this.cancelButton = new Glass.GlassButton();
            this.helpLabel = new System.Windows.Forms.Label();
            this.AddnewBTN = new Glass.GlassButton();
            this.listContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // itemsDiaplay
            // 
            this.itemsDiaplay.BackColor = System.Drawing.SystemColors.MenuText;
            this.itemsDiaplay.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.selectFrom});
            this.itemsDiaplay.ContextMenuStrip = this.listContextMenu;
            this.itemsDiaplay.ForeColor = System.Drawing.Color.Lime;
            this.itemsDiaplay.FullRowSelect = true;
            this.itemsDiaplay.Location = new System.Drawing.Point(13, 26);
            this.itemsDiaplay.MultiSelect = false;
            this.itemsDiaplay.Name = "itemsDiaplay";
            this.itemsDiaplay.Size = new System.Drawing.Size(479, 201);
            this.itemsDiaplay.TabIndex = 0;
            this.itemsDiaplay.UseCompatibleStateImageBehavior = false;
            this.itemsDiaplay.View = System.Windows.Forms.View.Details;
            this.itemsDiaplay.ItemActivate += new System.EventHandler(this.listView1_ItemActivate);
            // 
            // selectFrom
            // 
            this.selectFrom.Text = "Select From";
            this.selectFrom.Width = 253;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(410, 232);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(82, 26);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // helpLabel
            // 
            this.helpLabel.AutoSize = true;
            this.helpLabel.Location = new System.Drawing.Point(13, 7);
            this.helpLabel.Name = "helpLabel";
            this.helpLabel.Size = new System.Drawing.Size(172, 13);
            this.helpLabel.TabIndex = 2;
            this.helpLabel.Text = "Double click on one of the choices";
            // 
            // AddnewBTN
            // 
            this.AddnewBTN.Location = new System.Drawing.Point(16, 234);
            this.AddnewBTN.Name = "AddnewBTN";
            this.AddnewBTN.Size = new System.Drawing.Size(75, 23);
            this.AddnewBTN.TabIndex = 3;
            this.AddnewBTN.Text = "Add New";
            this.AddnewBTN.Click += new System.EventHandler(this.AddnewBTN_Click);
            // 
            // listContextMenu
            // 
            this.listContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteMenuItem});
            this.listContextMenu.Name = "listContextMenu";
            this.listContextMenu.Size = new System.Drawing.Size(153, 48);
            // 
            // deleteMenuItem
            // 
            this.deleteMenuItem.Name = "deleteMenuItem";
            this.deleteMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteMenuItem.Text = "Delete";
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            // 
            // MultiSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 264);
            this.ControlBox = false;
            this.Controls.Add(this.AddnewBTN);
            this.Controls.Add(this.helpLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.itemsDiaplay);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MultiSelectForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Select From List";
            this.listContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView itemsDiaplay;
        private GlassButton cancelButton;
        private System.Windows.Forms.ColumnHeader selectFrom;
        private System.Windows.Forms.Label helpLabel;
        private GlassButton AddnewBTN;
        private System.Windows.Forms.ContextMenuStrip listContextMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
    }
}