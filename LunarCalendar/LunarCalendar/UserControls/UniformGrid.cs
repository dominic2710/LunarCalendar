using System.Collections;
using System.Collections.ObjectModel;
using System.Data.Common;

namespace LunarCalendar.UserControls;

public class UniformGrid : Grid
{
    public IEnumerable ItemsSource
    {
        get { return (IEnumerable)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(UniformGrid),
                                propertyChanged: (bindable, oldValue, newValue) => 
                                                ((UniformGrid)bindable).PopulateItems((IEnumerable)oldValue, (IEnumerable)newValue));
    public DataTemplate ItemDataTemplate
    {
        get { return (DataTemplate)GetValue(ItemDataTemplateProperty); }
        set { SetValue(ItemDataTemplateProperty, value); }
    }
    public static readonly BindableProperty ItemDataTemplateProperty =
        BindableProperty.Create(nameof(ItemDataTemplate), typeof(DataTemplate), typeof(UniformGrid),
                                propertyChanged: (uniformGrid, oldValue, newValue) =>
                                            ((UniformGrid)uniformGrid).SetTemplate((DataTemplate)oldValue, (DataTemplate)newValue));

    public int UniformRow
    {
        get { return (int)GetValue(UniformRowProperty); }
        set { SetValue(UniformRowProperty, value); }
    }
    public static readonly BindableProperty UniformRowProperty =
        BindableProperty.Create(nameof(UniformRow), typeof(int), typeof(UniformGrid),
                                propertyChanged: (uniformGrid, oldValue, newValue) =>
                                            ((UniformGrid)uniformGrid).SetRow((int)oldValue, (int)newValue));

    public int UniformColumn
    {
        get { return (int)GetValue(UniformColumnProperty); }
        set { SetValue(UniformColumnProperty, value); }
    }
    public static readonly BindableProperty UniformColumnProperty =
        BindableProperty.Create(nameof(UniformColumn), typeof(int), typeof(UniformGrid),
                                propertyChanged: (uniformGrid, oldValue, newValue) =>
                                            ((UniformGrid)uniformGrid).SetColumn((int)oldValue, (int)newValue));

    private int maxItem = 0;

    public UniformGrid()
    {

    }

    void SetTemplate(DataTemplate oldValue, DataTemplate newValue)
    {
        for (int row = 0; row < ColumnDefinitions.Count; row++)
            for (int col = 0; col < ColumnDefinitions.Count; col++)
            {
                var itemTemplate = ItemDataTemplate.CreateContent() as View;
                Children.Add(itemTemplate);
                Grid.SetRow(itemTemplate, row);
                Grid.SetColumn(itemTemplate, col);
            }
    }

    void SetRow(int oldValue, int newValue)
    {
        for (int row = oldValue;  row < newValue; row++)
        {
            this.RowDefinitions.Add(new RowDefinition());
        }

        maxItem = (RowDefinitions.Count - 1) * (ColumnDefinitions.Count - 1);
    }

    void SetColumn(int oldValue, int newValue)
    {
        for (int col = oldValue; col < newValue; col++)
        {
            this.ColumnDefinitions.Add(new ColumnDefinition());
        }
        maxItem = (RowDefinitions.Count - 1) * (ColumnDefinitions.Count - 1);
    }

    void PopulateItems(IEnumerable oldValue, IEnumerable newValue)
    {
        int i = 0;

        if (newValue == null) return;

        foreach (var item in newValue)
        {
            (this.Children[i] as View).BindingContext = item;
            i++;

            if (i >= maxItem) break;
        }

        for (int j = i; j < maxItem; j++)
        {
            (this.Children[j] as View).BindingContext = null;
        }
    }

    private void UniformGrid_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        //throw new NotImplementedException();
    }
}