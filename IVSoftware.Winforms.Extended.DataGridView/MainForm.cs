using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        Apple,
        Orange,
        Banana,
    }
    class Fruit
    {
        public FruitType Name { get; set; }
    }
}
