using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FilesAndFolders.Portable
{
    class PortableMainPageViewModel : INotifyPropertyChanged
    {
        protected readonly XElement _xroot = new XElement("root");
        public ObservableCollection<FileItem> FileItems { get; } = new ObservableCollection<FileItem>();

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler? PropertyChanged;

    }
}
