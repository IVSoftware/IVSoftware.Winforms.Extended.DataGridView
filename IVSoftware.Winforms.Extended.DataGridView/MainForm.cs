using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IVSoftware.Winforms.Extended
{
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
    enum FruitName
    {
        [Color("Red")]
        Apple = 1,

        [Color("Orange")]
        Orange,

        [Color("Yellow")]
        Banana,
    }

    internal class ColorAttribute : Attribute
    {
        public ColorAttribute(string color)
        {
            if (Enum.TryParse(color, out KnownColor parsedColor))
            {
                Color = Color.FromKnownColor(parsedColor);
            }
            else
            {
                throw new ArgumentException($"The color '{color}' is not a known color.");
            }
        }
        public Color Color { get; }
    }

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
}
