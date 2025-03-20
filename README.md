First I'd like to show you my tree view and then I'll tell you it's dirty little secret: It's not any kind of "tree view" and it uses no recursion. It's all a smoke-and-mirrors illusion where a spaces in the DataTemplate is bound to the "depth" of the file node.

**XAML**

```
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:FilesAndFolders.Maui"
    x:Class="FilesAndFolders.Maui.MainPage"
    BackgroundColor="Fuchsia">

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

This isn't to say that there's not a tree structure involved, but consider the very portable approach of managing the hierarchy in a runtime XML document, but then projecting it onto a flat list to display it in a tree-like view.

