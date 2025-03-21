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
    class MainPageViewModel : INotifyPropertyChanged
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
        private readonly XElement _xroot = new XElement("root");
        public ObservableCollection<FileItem> FileItems { get; } = new ObservableCollection<FileItem>();
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
    enum NodeSortOrder { text, node, plusminus, }

    [DebuggerDisplay("{Text}")]
    class FileItem : INotifyPropertyChanged
    {
        public FileItem(XElement xel)
        {
            XEL = xel;
            PlusMinusToggleCommand = new Command(()=> 
            {
                switch (PlusMinus)
                {
                    case "+":
                        XEL?.SetAttributeValue(nameof(NodeSortOrder.plusminus), "-");
                        break;
                    case "-":
                        XEL?.SetAttributeValue(nameof(NodeSortOrder.plusminus), "+");
                        break;
                    default:
                        return;
                }
                OnPropertyChanged(nameof(PlusMinus));
            });
        }
        public XElement XEL { get; }

        public int Depth
        {
            get
            {
                if (_depth is null)
                {
                    _depth = XEL.Ancestors().SkipLast(1).Count();
                }
                return (int)_depth;
            }
        }
        int? _depth = null;
        public int Space => 10 * Depth;

        public string? Text => XEL.Attribute(nameof(NodeSortOrder.text))?.Value;

        public string PlusMinus
        {
            get
            {
                var currentValue = XEL?.Attribute(nameof(NodeSortOrder.plusminus))?.Value ?? "?";
                switch (currentValue)
                {
                    case "+":
                    case "-":
                        if (XEL?.Elements().Any() == true)
                        {
                            return currentValue;
                        }
                        else
                        {
                            XEL?.SetAttributeValue(nameof(NodeSortOrder.plusminus), string.Empty);
                            return string.Empty;
                        }
                    default:
                        if (XEL?.Elements().Any() == true)
                        {
                            XEL?.SetAttributeValue(nameof(NodeSortOrder.plusminus), "+");
                            return "+";
                        }
                        else
                        {
                            XEL?.SetAttributeValue(nameof(NodeSortOrder.plusminus), string.Empty);
                            return string.Empty;
                        }
                }
            }
        }

        public ICommand PlusMinusToggleCommand { get; }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    class TestData
    {
        public static string FILES { get; } = @"
D:\files-and-folders\.gitattributes
D:\files-and-folders\.gitignore
D:\files-and-folders\files-and-folders.sln
D:\files-and-folders\LICENSE.txt
D:\files-and-folders\README.md
D:\files-and-folders\.git\config
D:\files-and-folders\.git\description
D:\files-and-folders\.git\HEAD
D:\files-and-folders\.git\ms-persist.xml
D:\files-and-folders\FilesAndFolders\FilesAndFolders.csproj
D:\files-and-folders\FilesAndFolders\FilesAndFolders.csproj.user
D:\files-and-folders\FilesAndFolders\MainForm.cs
D:\files-and-folders\FilesAndFolders\MainForm.Designer.cs
D:\files-and-folders\FilesAndFolders\MainForm.resx
D:\files-and-folders\FilesAndFolders\Placer.cs
D:\files-and-folders\FilesAndFolders\Program.cs
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.deps.json
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.dll
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.exe
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.pdb
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.runtimeconfig.json
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\IVSoftware.Portable.Threading.dll
D:\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\IVSoftware.Portable.Xml.Linq.XBoundObject.dll
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\.NETCoreApp,Version=v8.0.AssemblyAttributes.cs
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\apphost.exe
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAnd.77B0DA9F.Up2Date
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.AssemblyInfo.cs
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.AssemblyInfoInputs.cache
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.assets.cache
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.AssemblyReference.cache
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.BuildWithSkipAnalyzers
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.CoreCompileInputs.cache
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.FileListAbsolute.txt
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.GenerateResource.cache
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.designer.deps.json
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.designer.runtimeconfig.json
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.dll
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.GeneratedMSBuildEditorConfig.editorconfig
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.genruntimeconfig.cache
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.GlobalUsings.g.cs
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.MainForm.resources
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.pdb
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\ref\FilesAndFolders.dll
D:\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\refint\FilesAndFolders.dll".Replace('\\', Path.DirectorySeparatorChar).Trim();
    }
}
