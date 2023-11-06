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
                        var fruit = ((Fruit)Rows[e.RowIndex].DataBoundItem);
                        var color = fruit.Color;
                        int diameter = Math.Min(e.CellBounds.Width, e.CellBounds.Height) - 50;
                        int x = e.CellBounds.X + (e.CellBounds.Width - diameter) / 2;
                        int y = e.CellBounds.Y + (e.CellBounds.Height - diameter) / 2;
                        e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
                        Rectangle circleBounds = new Rectangle(x, y, diameter, diameter);
                        using (var brush = new SolidBrush(color))
                        {
                            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            e.Graphics.FillEllipse(brush, circleBounds);
                        }
                        using (var pen = new Pen(Color.DarkSlateGray, 2)) 
                        {
                            e.Graphics.DrawEllipse(pen, circleBounds);
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
