using System;
using System.Windows.Forms;
using System.Globalization;

namespace yz.furniture.UI.Forms
{
    public class DoorInputForm : Form
    {
        private TextBox lengthTextBox;
        private TextBox widthTextBox;
        private TextBox marginTextBox;
        private TextBox grooveBottomTextBox;
        private TextBox grooveLengthTextBox;

        public double DoorLength { get; private set; }
        public double DoorWidth { get; private set; }
        public double SideMargin { get; private set; }
        public double GrooveBottom { get; private set; }
        public double GrooveLength { get; private set; }

        public DoorInputForm()
        {
            this.Text = "输入门板参数";
            this.Size = new System.Drawing.Size(300, 320);
            this.StartPosition = FormStartPosition.CenterParent;

            var layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 2;
            layout.RowCount = 6;
            this.Controls.Add(layout);

            // Labels and TextBoxes
            layout.Controls.Add(new Label() { Text = "门板长度:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
            lengthTextBox = new TextBox() { Text = "2000", Dock = DockStyle.Fill };
            layout.Controls.Add(lengthTextBox, 1, 0);

            layout.Controls.Add(new Label() { Text = "门板宽度:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 1);
            widthTextBox = new TextBox() { Text = "600", Dock = DockStyle.Fill };
            layout.Controls.Add(widthTextBox, 1, 1);

            layout.Controls.Add(new Label() { Text = "串带边距:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 2);
            marginTextBox = new TextBox() { Text = "60", Dock = DockStyle.Fill };
            layout.Controls.Add(marginTextBox, 1, 2);

            layout.Controls.Add(new Label() { Text = "串带下边距:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 3);
            grooveBottomTextBox = new TextBox() { Text = "60", Dock = DockStyle.Fill };
            layout.Controls.Add(grooveBottomTextBox, 1, 3);

            layout.Controls.Add(new Label() { Text = "串带长度:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 4);
            grooveLengthTextBox = new TextBox() { Text = "500", Dock = DockStyle.Fill };
            layout.Controls.Add(grooveLengthTextBox, 1, 4);

            // OK and Cancel buttons
            var okButton = new Button() { Text = "确定", DialogResult = DialogResult.OK };
            okButton.Click += OkButton_Click;
            var cancelButton = new Button() { Text = "取消", DialogResult = DialogResult.Cancel };

            var buttonPanel = new FlowLayoutPanel();
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.Controls.Add(cancelButton);
            buttonPanel.Controls.Add(okButton);
            layout.Controls.Add(buttonPanel, 0, 5);
            layout.SetColumnSpan(buttonPanel, 2);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;

            // Add KeyDown event handlers for focus switching
            lengthTextBox.KeyDown += TextBox_KeyDown;
            widthTextBox.KeyDown += TextBox_KeyDown;
            marginTextBox.KeyDown += TextBox_KeyDown;
            grooveBottomTextBox.KeyDown += TextBox_KeyDown;
            grooveLengthTextBox.KeyDown += TextBox_KeyDown;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var currentTextBox = sender as TextBox;
            if (currentTextBox == null) return;

            if (e.KeyCode == Keys.Down)
            {
                if (currentTextBox == lengthTextBox) widthTextBox.Focus();
                else if (currentTextBox == widthTextBox) marginTextBox.Focus();
                else if (currentTextBox == marginTextBox) grooveBottomTextBox.Focus();
                else if (currentTextBox == grooveBottomTextBox) grooveLengthTextBox.Focus();
                else if (currentTextBox == grooveLengthTextBox) lengthTextBox.Focus(); // Loop
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (currentTextBox == lengthTextBox) grooveLengthTextBox.Focus(); // Loop
                else if (currentTextBox == widthTextBox) lengthTextBox.Focus();
                else if (currentTextBox == marginTextBox) widthTextBox.Focus();
                else if (currentTextBox == grooveBottomTextBox) marginTextBox.Focus();
                else if (currentTextBox == grooveLengthTextBox) grooveBottomTextBox.Focus();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            try
            {
                DoorLength = double.Parse(lengthTextBox.Text, CultureInfo.InvariantCulture);
                DoorWidth = double.Parse(widthTextBox.Text, CultureInfo.InvariantCulture);
                SideMargin = double.Parse(marginTextBox.Text, CultureInfo.InvariantCulture);
                GrooveBottom = double.Parse(grooveBottomTextBox.Text, CultureInfo.InvariantCulture);
                GrooveLength = double.Parse(grooveLengthTextBox.Text, CultureInfo.InvariantCulture);
                this.DialogResult = DialogResult.OK;
            }
            catch (FormatException)
            {
                MessageBox.Show("请输入有效的数字。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None; // Prevent form from closing
            }
        }
    }
}
