using FilesAndFolders.Portable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilesAndFolders
{
    public partial class FileItemDataTemplate : UserControl
    {
        public FileItemDataTemplate()
        {
            InitializeComponent();
            Height = 50;
            PlusMinus.Click += (sender, e) => DataContext?.PlusMinusToggleCommand?.Execute(null); 
        }
        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is not null)
            {
                Spacer.DataBindings.Clear();
                PlusMinus.DataBindings.Clear();
                TextLabel.DataBindings.Clear();

                Spacer.DataBindings.Add(nameof(Spacer.Width), DataContext, nameof(DataContext.Space), false, DataSourceUpdateMode.OnPropertyChanged);
                PlusMinus.DataBindings.Add(nameof(PlusMinus.Text), DataContext, nameof(DataContext.PlusMinus), false, DataSourceUpdateMode.OnPropertyChanged);
                TextLabel.DataBindings.Add(nameof(TextLabel.Text), DataContext, nameof(DataContext.Text), false, DataSourceUpdateMode.OnPropertyChanged);
            }
        }
        public new FileItem? DataContext
        {
            get => (FileItem?)base.DataContext;
            set => base.DataContext = value;
        }
    }
}
