using System;
using System.Drawing;
using System.Windows.Forms;

namespace IVSoftware.Winforms.Extended
{
    public class DataGridViewEx : DataGridView
    {
        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);
            if (!((e.ColumnIndex == -1) || (e.RowIndex == -1)))
            {
                if (this[e.ColumnIndex, e.RowIndex] is EnumComboBoxExCell cbCell)
                {
                    if (cbCell.EnumComboBox.Parent is null)
                    {
                        Controls.Add(cbCell.EnumComboBox);
                        cbCell.EnumComboBox.SetEnumType<FruitType>();
                        cbCell.EnumComboBox.Font = DefaultCellStyle.Font;
                    }
                    cbCell.EnumComboBox.Bounds = GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
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
