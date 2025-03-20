using IVSoftware.Portable.Xml.Linq;
using IVSoftware.Portable.Xml.Linq.XBoundObject;
using System.Xml.Linq;

namespace FilesAndFolders
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
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
            files = TEST_DATA.Replace("\r\n", "\n").Split('\n');
#endif
            foreach (var file in files)
            {
                var pathArray = file.Split(Path.DirectorySeparatorChar);
                new Placer(xroot, file, onBeforeAdd: (sender, e) =>
                {
                    e.Xel.SetBoundAttributeValue(
                        new FlatTreeNode(e.Xel, depth: pathArray.Length - 1),
                        name: nameof(NodeSortOrder.node));
                });
            }
            xroot.SortAttributes<NodeSortOrder>();
            { }
        }

        FolderBrowserDialog _folderBrowser = new();
        XElement xroot = new XElement("root");

        public string TEST_DATA { get; } = @"
D:\Github\IVSoftware\Maui\files-and-folders\.gitattributes
D:\Github\IVSoftware\Maui\files-and-folders\.gitignore
D:\Github\IVSoftware\Maui\files-and-folders\files-and-folders.sln
D:\Github\IVSoftware\Maui\files-and-folders\LICENSE.txt
D:\Github\IVSoftware\Maui\files-and-folders\README.md
D:\Github\IVSoftware\Maui\files-and-folders\.git\COMMIT_EDITMSG
D:\Github\IVSoftware\Maui\files-and-folders\.git\config
D:\Github\IVSoftware\Maui\files-and-folders\.git\description
D:\Github\IVSoftware\Maui\files-and-folders\.git\HEAD
D:\Github\IVSoftware\Maui\files-and-folders\.git\index
D:\Github\IVSoftware\Maui\files-and-folders\.git\ms-persist.xml
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\FilesAndFolders.csproj
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\FilesAndFolders.csproj.user
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\MainForm.cs
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\MainForm.Designer.cs
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\MainForm.resx
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\Placer.cs
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\Program.cs
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\applypatch-msg.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\commit-msg.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\fsmonitor-watchman.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\post-update.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\pre-applypatch.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\pre-commit.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\pre-merge-commit.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\pre-push.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\pre-rebase.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\pre-receive.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\prepare-commit-msg.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\push-to-checkout.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\sendemail-validate.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\hooks\update.sample
D:\Github\IVSoftware\Maui\files-and-folders\.git\info\exclude
D:\Github\IVSoftware\Maui\files-and-folders\.git\logs\HEAD
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\FilesAndFolders.csproj.nuget.dgspec.json
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\FilesAndFolders.csproj.nuget.g.props
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\FilesAndFolders.csproj.nuget.g.targets
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\project.assets.json
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\project.nuget.cache
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\1f\f0c423042b46cb1d617b81efb715defbe8054d
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\26\d5b31ff6f59df48775d23033d20eec50a0ac9b
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\31\084d5f7d0094b3d1a306ed23ec2991e15e668f
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\44\e3d4c87289fe53c79bd606ac8d478fd1216f8f
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\48\6ab842ce61ee2d5f0e92b816cfbd207fbe0949
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\74\c2597058de9f7b912b5d5d84113fa19df35acf
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\78\e537e1e67125524cdf955eaeb1e3a768b08529
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\8a\a26455d23acf904be3ed9dfb3a3efe3e49245a
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\92\0a0339acb76b6667b79fb16d21bed52728bfd6
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\94\91a2fda28342ab358eaf234e1afe0c07a53d62
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\a3\022ef4a21371886e00a7972212d65602e2ae21
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\b4\05a4a5cd5f47780d3b1ffdaec476dabe0c2298
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\c3\97bbf191d9342a4e596240871e58f85d2536ee
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\ce\2629b9c33a8e01ea5023b956cf2ed00e38c7ba
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\d4\611fbb6c9b59c0e034c0effca782b30afb49f2
D:\Github\IVSoftware\Maui\files-and-folders\.git\objects\ea\379430627c8fe6a66da9aff5b30c16030e719a
D:\Github\IVSoftware\Maui\files-and-folders\.git\refs\heads\master
D:\Github\IVSoftware\Maui\files-and-folders\.vs\files-and-folders\DesignTimeBuild\.dtbcache.v2
D:\Github\IVSoftware\Maui\files-and-folders\.vs\files-and-folders\FileContentIndex\4cceda04-c4f2-4d49-aea8-7ccbaa0d4f66.vsidx
D:\Github\IVSoftware\Maui\files-and-folders\.vs\files-and-folders\FileContentIndex\9bea6a1f-56b4-4239-a291-42aafd5aa941.vsidx
D:\Github\IVSoftware\Maui\files-and-folders\.vs\files-and-folders\FileContentIndex\a8256b2d-aed0-4e65-8c8c-31cd3ff247e5.vsidx
D:\Github\IVSoftware\Maui\files-and-folders\.vs\files-and-folders\FileContentIndex\b5b6ba28-1be9-45b2-8a70-110e1a9d4944.vsidx
D:\Github\IVSoftware\Maui\files-and-folders\.vs\files-and-folders\FileContentIndex\f70ca458-880b-4b64-b3d7-1c48540867cd.vsidx
D:\Github\IVSoftware\Maui\files-and-folders\.vs\files-and-folders\v17\.suo
D:\Github\IVSoftware\Maui\files-and-folders\.vs\files-and-folders\v17\DocumentLayout.backup.json
D:\Github\IVSoftware\Maui\files-and-folders\.vs\files-and-folders\v17\DocumentLayout.json
D:\Github\IVSoftware\Maui\files-and-folders\.git\logs\refs\heads\master
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.deps.json
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.dll
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.exe
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.pdb
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\FilesAndFolders.runtimeconfig.json
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\IVSoftware.Portable.Threading.dll
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\bin\Debug\net8.0-windows\IVSoftware.Portable.Xml.Linq.XBoundObject.dll
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\.NETCoreApp,Version=v8.0.AssemblyAttributes.cs
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\apphost.exe
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAnd.77B0DA9F.Up2Date
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.AssemblyInfo.cs
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.AssemblyInfoInputs.cache
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.assets.cache
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.AssemblyReference.cache
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.BuildWithSkipAnalyzers
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.CoreCompileInputs.cache
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.FileListAbsolute.txt
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.csproj.GenerateResource.cache
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.designer.deps.json
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.designer.runtimeconfig.json
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.dll
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.GeneratedMSBuildEditorConfig.editorconfig
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.genruntimeconfig.cache
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.GlobalUsings.g.cs
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.MainForm.resources
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\FilesAndFolders.pdb
D:\Github\IVSoftware\Maui\files-and-folders\.vs\files-and-folders\v17\TestStore\0\000.testlog
D:\Github\IVSoftware\Maui\files-and-folders\.vs\files-and-folders\v17\TestStore\0\testlog.manifest
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\ref\FilesAndFolders.dll
D:\Github\IVSoftware\Maui\files-and-folders\FilesAndFolders\obj\Debug\net8.0-windows\refint\FilesAndFolders.dll".Trim();
    }
    enum NodeSortOrder { text, node }
    class FlatTreeNode
    {
        public FlatTreeNode(XElement xel, int depth) 
        {
            XEL = xel;
        }
        XElement XEL { get; }
        public int Depth { get; set; }
        public string? Text => XEL.Attribute(nameof(NodeSortOrder.text))?.Value;
    }
}
