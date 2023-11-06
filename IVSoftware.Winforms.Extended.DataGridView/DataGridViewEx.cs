using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;

namespace IVSoftware.Winforms.Extended
{
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
                                // Probably want to switch(Columns[e.ColumnIndex
                                cbCell.EnumComboBox.SetEnumType<FruitName>();
                                cbCell.EnumComboBox.Font = DefaultCellStyle.Font;
                                cbCell.EnumComboBox.Cell = cbCell;
                                cbCell.EnumComboBox.SelectedEnumValueChanged += (sender, e) =>
                                {
                                    if(sender is EnumComboBox enumComboBox)
                                    {
                                        ((Fruit)Rows[enumComboBox.Cell.RowIndex].DataBoundItem).Name = (FruitName)e.Value;
                                    }
                                };
                            }
                            cbCell.EnumComboBox.Bounds = GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                        }
                        break;
                    case nameof(Fruit.Color):
                        var fruit = ((Fruit)Rows[e.RowIndex].DataBoundItem);
                        var color = fruit.Color;
                        using(var brush = new SolidBrush(color))
                        {
                            e.Graphics.FillRectangle(brush, e.CellBounds);
                        }
                        e.Handled = true;
                        break;
                }
            }
        }
    }
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
}
