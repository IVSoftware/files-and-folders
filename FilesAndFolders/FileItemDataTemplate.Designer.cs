namespace FilesAndFolders
{
    partial class FileItemDataTemplate
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
            Grid = new TableLayoutPanel();
            spacer = new Control();
            Grid.SuspendLayout();
            SuspendLayout();
            // 
            // Grid
            // 
            Grid.ColumnCount = 3;
            Grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            Grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            Grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Grid.Controls.Add(spacer, 0, 0);
            Grid.Dock = DockStyle.Fill;
            Grid.Location = new Point(0, 0);
            Grid.Margin = new Padding(0);
            Grid.Name = "Grid";
            Grid.RowCount = 1;
            Grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Grid.Size = new Size(443, 54);
            Grid.TabIndex = 0;
            // 
            // spacer
            // 
            spacer.AutoSize = true;
            spacer.Location = new Point();
            spacer.Padding = new Padding();
            spacer.Margin = new Padding();
            spacer.Name = "spacer";
            spacer.Size = new Size(49, 49);
            spacer.TabIndex = 0;
            // 
            // FileItem
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(Grid);
            Name = "FileItem";
            Size = new Size(443, 54);
            Grid.ResumeLayout(false);
            Grid.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel Grid;
        private Control spacer;
    }
}
