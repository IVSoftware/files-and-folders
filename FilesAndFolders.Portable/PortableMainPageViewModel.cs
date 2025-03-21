using IVSoftware.Portable.Xml.Linq.XBoundObject;
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


        protected IEnumerable<FileItem> VisibleFileItems()
        {
            foreach (var element in localAddChildItems(_xroot.Elements()))
            {
                yield return element;
            }

            IEnumerable<FileItem> localAddChildItems(IEnumerable<XElement> elements)
            {
                foreach (var element in elements)
                {
                    if (element.To<FileItem>() is { } fileItem)
                    {
                        yield return fileItem;
                    }
                    if (element.Attribute(nameof(NodeSortOrder.plusminus))?.Value == "-")
                    {
                        foreach (var childElement in localAddChildItems(element.Elements()))
                        {
                            yield return childElement;
                        }
                    }
                    else
                    {   /* G T K */
                    }
                }
            }
        }
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (!Equals(_isBusy, value))
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }
        bool _isBusy = false;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler? PropertyChanged;

    }
}
