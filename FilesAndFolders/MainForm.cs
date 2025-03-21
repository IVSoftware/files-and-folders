using FilesAndFolders.Portable;
using IVSoftware.Portable;
using IVSoftware.Portable.Xml.Linq;
using IVSoftware.Portable.Xml.Linq.XBoundObject;
using System.Drawing.Printing;
using System.Xml.Linq;

namespace FilesAndFolders
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            base.DataContext = new MainPageViewModel();
            DataContext.FileItems.CollectionChanged += (sender, e) 
                => wdtCollectionChangeSettled.StartOrRestart();
            // Go ahead and populate the FLP with test data!
            wdtCollectionChangeSettled.StartOrRestart();
#if false

            string[] files;
#if INTERACTIVE
            var resolvedPath = Path.GetFullPath(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", ".."));
            _folderBrowser.InitialDirectory = resolvedPath;
            Disposed += (sender, e) => _folderBrowser.Dispose();
            tsmiBrowse.Click += (sender, e) =>
            {
                if(DialogResult.Cancel != _folderBrowser.ShowDialog(this))
                {
                    files = Directory.GetFiles(_folderBrowser.SelectedPath, "*", SearchOption.AllDirectories);
                    var test_data = string.Join("\n", files);
                }
            };
#else
            files = TestData.FILES.Replace("\r\n", "\n").Split('\n');
#endif
            foreach (var file in files)
            {
                var pathArray = file.Split(Path.DirectorySeparatorChar);
                new Placer(xroot, file, onBeforeAdd: (sender, e) =>
                {
                    e.Xel.SetBoundAttributeValue(
                        new FileItem(e.Xel),
                        name: nameof(NodeSortOrder.node));
                });
            }
            xroot.SortAttributes<NodeSortOrder>();
            { }
#endif
        }
        new MainPageViewModel DataContext => (MainPageViewModel)base.DataContext!;

        FolderBrowserDialog _folderBrowser = new();


        public WatchdogTimer wdtCollectionChangeSettled
        {
            get
            {
                if (_wdtCollectionChangeSettled is null)
                {
                    _wdtCollectionChangeSettled = new WatchdogTimer { Interval = TimeSpan.FromMilliseconds(100) };
                    _wdtCollectionChangeSettled.RanToCompletion += (sender, e) =>
                    {
                        BeginInvoke(() =>
                        {
                            FileCollectionView.Controls.Clear();
                            foreach (var fileItem in DataContext.FileItems)
                            {
                                FileCollectionView.Add(fileItem);
                            }
                        });
                    };
                }
                return _wdtCollectionChangeSettled;
            }
        }
        WatchdogTimer? _wdtCollectionChangeSettled = null;
    }
    static partial class Extensions
    {
        public static void Add(this FlowLayoutPanel @this, FileItem fileItem)
        {
            var MARGIN = new Padding(2);
            @this.Controls.Add(new FileItemDataTemplate
            {
                AutoSize = false,
                Margin = MARGIN,
                Width =
                        @this.Width - @this.Padding.Horizontal
                        - SystemInformation.VerticalScrollBarWidth - MARGIN.Horizontal,
                DataContext = fileItem,
            });
        }
    }
}
