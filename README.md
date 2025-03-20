First I'd like to show you my tree view and then I'll tell you it's dirty little secret: It's not any kind of "tree view" and it uses no recursion. It's all a smoke-and-mirrors illusion where a spaces in the DataTemplate is bound to the "depth" of the file node. It's just one approach to consider.

**XAML**

```
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:FilesAndFolders.Maui"
    x:Class="FilesAndFolders.Maui.MainPage"
    BackgroundColor="RoyalBlue">

    <ContentPage.BindingContext>
        <local:MainPageViewModel/>
    </ContentPage.BindingContext>
    <CollectionView 
        x:Name="FileCollectionView" 
            ItemsSource="{Binding FileItems}" 
            SelectionMode="None"
            SelectedItem="{Binding SelectedItem, Mode=TwoWay}"
            BackgroundColor="LightBlue"
            Margin="1">
        <CollectionView.ItemsLayout>
            <LinearItemsLayout Orientation="Vertical" ItemSpacing="2" />
        </CollectionView.ItemsLayout>
        <CollectionView.ItemTemplate>
            <DataTemplate>
                <Grid ColumnDefinitions="Auto,40,*" RowDefinitions="40" Background="White">
                    <BoxView 
                        Grid.Column="0" 
                        WidthRequest="{Binding Space}"
                        BackgroundColor="{Binding BackgroundColor, Source={x:Reference FileCollectionView}}"/>
                    <Button 
                        Grid.Column="1" 
                        Text="{Binding PlusMinus}" 
                        TextColor="Black"
                        Command="{Binding PlusMinusToggleCommand}"
                        FontSize="16"
                        BackgroundColor="Transparent"
                        Padding="0"
                        BorderWidth="0"
                        VerticalOptions="Fill"
                        HorizontalOptions="Fill"
                        MinimumHeightRequest="0"
                        MinimumWidthRequest="0"
                        CornerRadius="0"/>
                    <Label 
                        Grid.Column="2"
                        Text="{Binding Text}" 
                        VerticalTextAlignment="Center" Padding="2,0,0,0"/>                    
                </Grid>
            </DataTemplate>
        </CollectionView.ItemTemplate>
    </CollectionView>
</ContentPage>
```

**Data Model**

```
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

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (!Equals(_isVisible, value))
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }
    }
    bool _isVisible = false;

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
```
___

**Managing the (actual) Hierarchy**

This isn't to say that there's not a tree structure involved, but consider the very portable approach of managing the hierarchy in a runtime XML document, but then projecting it onto a flat list to display it in a tree-like view. Here's what you're seeing here.

- The `Placer` code block projects the file names onto the XML document.
- The `System.Xml.Linq.Changed` event is used to detect when the data model sets "+" or "-" as a attribute on the bound node.
- The `ObservableCollection<FileItem>` in this non-optimized version is cleared then repopulated using the `VisibleFileItems` iterator on the XML root model.

```
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
}
```

