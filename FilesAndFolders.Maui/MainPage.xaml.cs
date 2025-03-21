using FilesAndFolders.Portable;
using IVSoftware.Portable.Xml.Linq;
using IVSoftware.Portable.Xml.Linq.XBoundObject;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml.Linq;

namespace FilesAndFolders.Maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()=> InitializeComponent();
    }
    // <PackageReference Include = "IVSoftware.Portable.Xml.Linq.XBoundObject" Version="1.4.0-prerelease" />
    class MainPageViewModel : PortableMainPageViewModel
    {
        public MainPageViewModel()
        {
            var files = TestData.FILES.Split(Environment.NewLine);
            foreach (var file in files)
            {
                new Placer(_xroot, file, onBeforeAdd: (sender, e) =>
                {
                    // Attach an instance of FileItem to the XElement.                    
                    e.Xel.SetBoundAttributeValue(
                        new FileItem(e.Xel),
                        name: nameof(NodeSortOrder.node));
                });
            }
            _xroot.SortAttributes<NodeSortOrder>();
            foreach (var root in _xroot.Elements().ToArray())
            {
                FileItems.Add(root.To<FileItem>());
            }
            _xroot.Changed += async(sender, e) =>
            {
                switch (e.ObjectChange)
                {
                    case XObjectChange.Add:
                    case XObjectChange.Remove:
                    case XObjectChange.Name:
                        return;
                    case XObjectChange.Value:
                        break;
                }
                if (
                    sender is XAttribute attr &&
                    string.Equals(attr.Name.LocalName, nameof(NodeSortOrder.plusminus)) &&
                    attr.Parent is XElement xel)
                {
                    if (xel.To<FileItem>() is { } fileItem)
                    {
                        switch (attr.Value)
                        {
                            case "":
                                // N O O P
                                break;
                            case "-":
                                IsBusy = true;
                                await Task.Delay(1);
                                FileItems.Clear();
                                foreach (var visibleFileItem in VisibleFileItems())
                                {
                                    FileItems.Add(visibleFileItem);
                                }
                                IsBusy = false;
                                break;
                            case "+":
                                IsBusy = true;
                                await Task.Delay(1);
                                foreach (var desc in xel.Descendants().Select(_ => _.To<FileItem>()))
                                {
                                    if (desc != null)
                                    {
                                        FileItems.Remove(desc);
                                    }
                                }
                                IsBusy = false;
                                await Task.Delay(25);
                                break;
                        }
                    }
                }
            };
        }
        IEnumerable<FileItem> VisibleFileItems()
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
                    if(element.Attribute(nameof(NodeSortOrder.plusminus))?.Value == "-")
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
