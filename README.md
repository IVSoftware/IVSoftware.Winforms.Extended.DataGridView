If this is the effect you want, what has worked for me is inheriting DataGridView so that it knows how to draw a `Control` in the cell display rectangle. It can be *any* `Control` or `UserControl`. 

In this case I made my own `EnumComboBox` ([Clone]() full sample) but that's not the point so much as that you can draw whatever you want in the cell bounds so why not let a custom control draw _itself_ there?

[![combo box fills height][1]][1]

```
public class DataGridViewEx : DataGridView
{
    protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
    {
        base.OnCellPainting(e);
        if (!((e.ColumnIndex == -1) || (e.RowIndex == -1)))
        {
            switch (Columns[e.ColumnIndex].Name)
            {
                case nameof(Fruit.Name):
                    if (this[e.ColumnIndex, e.RowIndex] is EnumComboBoxExCell cbCell)
                    {
                        if (cbCell.EnumComboBox.Parent is null)
                        {
                            Controls.Add(cbCell.EnumComboBox); 
                            cbCell.EnumComboBox.SetEnumType<FruitName>();
                            cbCell.EnumComboBox.Font = DefaultCellStyle.Font;
                            cbCell.EnumComboBox.Cell = cbCell;
                            cbCell.EnumComboBox.SelectedEnumValueChanged += (sender, e) =>
                            {
                                // Bind the combo box back to the data item.
                                if(sender is EnumComboBox enumComboBox)
                                {
                                    ((Fruit)Rows[enumComboBox.Cell.RowIndex].DataBoundItem).Name = (FruitName)e.Value;
                                }
                            };
                        }
                        cbCell.EnumComboBox.Bounds = GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                        e.Handled = true;
                    }
                    break;
                case nameof(Fruit.Color):
                    .
                    .
                    .
                    break;
            }
        }
    }
}
```
Where:

```
class DataGridViewEnumComboBoxColumn : DataGridViewColumn
{
    public DataGridViewEnumComboBoxColumn(DataGridViewColumn col)
    {
        base.Name = col.Name;
        base.HeaderText = col.HeaderText;
        base.CellTemplate = new EnumComboBoxExCell();
    }
}
class EnumComboBoxExCell : DataGridViewTextBoxCell
{
    public EnumComboBox EnumComboBox { get; } = new EnumComboBox();
}
```

___

***EXAMPLE: *Main form consumer of DataGridViewEx**

```
public partial class MainForm : Form
{
    public MainForm() => InitializeComponent();
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        dataGridView.RowTemplate.Height = 80;
        dataGridView.AllowUserToAddRows = false;
        dataGridView.RowHeadersVisible = false;
        dataGridView.DataSource = Fruits;
        var col = dataGridView.Columns["Name"];
        var index = col.Index;

        // Swap out the 'normal' column for DataGridViewEnumComboBoxColumn
        dataGridView.Columns.RemoveAt(index);
        dataGridView.Columns.Insert(index, new DataGridViewEnumComboBoxColumn(col));

        dataGridView.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dataGridView.Columns["Color"].DefaultCellStyle.SelectionBackColor = dataGridView.BackgroundColor;
        dataGridView.Columns["Color"].Width = 80;

        Fruits.Add(new Fruit());
        Fruits.Add(new Fruit());
        Fruits.Add(new Fruit());
    }
    BindingList<Fruit> Fruits { get; } = new BindingList<Fruit>();
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
            return cp;
        }
    }
}
```
**Where**

```
enum FruitType
{
    Apple,
    Orange,
    Banana,
}
```
And

```
class Fruit : INotifyPropertyChanged
{
    public Color Color
    {
        get
        {
            var fieldInfo = typeof(FruitName).GetField(Name.ToString());
            if (fieldInfo == null)
            {
                return Color.White;
            }
            else
            {
                return fieldInfo.GetCustomAttribute<ColorAttribute>().Color;
            }
        }
    }
    public FruitName Name
    {
        get => _name;
        set
        {
            if (!Equals(_name, value))
            {
                Debug.WriteLine(value);
                _name = value;
                // Notify 'Color' not 'Name'
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
            }
        }
    }
    FruitName _name = 0;
    public event PropertyChangedEventHandler PropertyChanged;
}
```


  [1]: https://i.stack.imgur.com/WqFnc.png