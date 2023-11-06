using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            dataGridView.Columns.RemoveAt(index);
            dataGridView.Columns.Insert(index, new DataGridViewEnumComboBoxColumn(col));
            dataGridView.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            BeginInvoke((MethodInvoker)delegate { dataGridView.CurrentCell = null; });

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
    enum FruitType
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

    class Fruit
    {
        public Color Color
        {
            get
            {
                var fieldInfo = typeof(FruitType).GetField(Name.ToString());
                var color = fieldInfo?.GetCustomAttribute<ColorAttribute>().Color ?? default;
                { }
                return color;
            }
        }
        public FruitType Name
        {
            get => _name;
            set
            {
                if (!Equals(_name, value))
                {
                    _name = value;
                }
            }
        }
        FruitType _name = 0;

    }
}
