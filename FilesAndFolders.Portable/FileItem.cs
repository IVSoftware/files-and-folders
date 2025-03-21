using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml.Linq;

namespace FilesAndFolders.Portable
{

    [DebuggerDisplay("{Text}")]
    public class FileItem : INotifyPropertyChanged
    {
        public FileItem(XElement xel)
        {
            XEL = xel;
            PlusMinusToggleCommand = new Command(() =>
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
}
