using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace IVSoftware.Winforms.Extended
{
    public partial class EnumComboBox : Control
    {
        private Label _label;
        private ListBoxContainer _listBoxContainer;
        private ListBox _listBox => _listBoxContainer;
        private Label icon;
        private bool _isDropDownVisible = false;
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            InitializeCustomComboBox();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            icon.Location = new Point(Width - Height, 0);
            icon.Size = new Size(Height, Height);
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _listBox.Font = Font;
        }
        private void InitializeCustomComboBox()
        {
            _label = new Label
            {
                BackColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Select...",
                BorderStyle = BorderStyle.FixedSingle,
            };
            _label.Dock = DockStyle.Fill;

            _listBoxContainer = new ListBoxContainer
            {
                Width = Width,
            };
            _listBox.SelectedIndexChanged += (sender, e) => UpdateTextBoxText();

            //https://symbl.cc/en/collections/arrow-symbols/
            icon = new Label
            {
                BackColor = Color.WhiteSmoke,
                Height = Height,
                Width = Height,
                Left = Width - Height,
                Text = "\u25BC",
                TextAlign = ContentAlignment.MiddleCenter,
            };

            this.Controls.Add(_label);
            _label.Controls.Add(icon);
            _label.Click += (sender, e) => ToggleDropDown();
            icon.Click += (sender, e) => ToggleDropDown();
        }
        public IEnumerable<string> Items
        {
            get { return _listBox.Items.Cast<string>(); }
            set
            {
                _listBox.Items.Clear();
                _listBox.Items.AddRange(value.ToArray());
            }
        }
        private void ToggleDropDown()
        {
            if (_isDropDownVisible)
            {
                HideDropDown();
            }
            else
            {
                ShowDropDown();
            }
        }
        private void ShowDropDown()
        {
            var screen = RectangleToScreen(ClientRectangle);
            _listBoxContainer.Height = localMeasureItems();
            _isDropDownVisible = true;
            _listBoxContainer.Top = screen.Bottom;
            _listBoxContainer.Left = screen.Left;
            _listBoxContainer.Width = Width;
            _listBoxContainer.Show(this);
            _listBoxContainer.TopMost = true;
            _listBoxContainer.BringToFront();

            int localMeasureItems()
            {
                var totalHeight = 0;
                if (_listBox.Items.Count == 0)
                {
                    return 20;
                }
                else
                {
                    foreach (var item in _listBox.Items)
                    {
                        using (Graphics graphics = CreateGraphics())
                        {
                            SizeF size = graphics.MeasureString(item.ToString(), _listBox.Font);
                            totalHeight += (int)Math.Ceiling(size.Height);
                        }
                    }
                    return totalHeight;
                }
            }
        }
        private void HideDropDown()
        {
            _isDropDownVisible = false;
            _listBoxContainer.Visible = false;
        }
        private void UpdateTextBoxText()
        {
            if (_listBox.SelectedIndex >= 0)
            {
                _label.Text = _listBox.SelectedItem.ToString();
            }

            HideDropDown();
        }
        internal void SetEnumType<T>() where T : Enum
        {
            _listBox.Items.Clear();
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                _listBox.Items.Add(value);
            }
        }
    }
    class ListBoxContainer : Form
    {
        public static implicit operator ListBox(ListBoxContainer @this) => @this._listBox;
             
        public ListBoxContainer() 
        {
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            _listBox = new ListBox
            {
                Font = Font,
                Dock = DockStyle.Fill,                
            };
            Controls.Add(_listBox);
            BackColor = Color.LimeGreen;
            TransparencyKey = Color.LimeGreen;
        }
        readonly ListBox _listBox;
    }
}
