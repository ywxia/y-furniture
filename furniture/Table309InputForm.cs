using System;
using System.Windows.Forms;
using System.Drawing;

namespace furniture
{
    public class Table309InputForm : Form
    {
        private TextBox txtLength;
        private TextBox txtWidth;
        private Button btnOk;
        private Button btnCancel;

        public double TableLength { get; private set; }
        public double TableWidth { get; private set; }

        public Table309InputForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "输入309型餐桌参数";
            this.Size = new Size(300, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblLength = new Label() { Text = "长度:", Location = new Point(20, 20), Size = new Size(80, 20) };
            txtLength = new TextBox() { Location = new Point(100, 20), Size = new Size(150, 20), Text = "1200" };
            txtLength.KeyDown += new KeyEventHandler(TextBox_KeyDown);

            var lblWidth = new Label() { Text = "宽度:", Location = new Point(20, 50), Size = new Size(80, 20) };
            txtWidth = new TextBox() { Location = new Point(100, 50), Size = new Size(150, 20), Text = "600" };
            txtWidth.KeyDown += new KeyEventHandler(TextBox_KeyDown);

            btnOk = new Button() { Text = "确定", Location = new Point(100, 100), Size = new Size(75, 25) };
            btnOk.Click += new EventHandler(BtnOk_Click);
            this.AcceptButton = btnOk;

            btnCancel = new Button() { Text = "取消", Location = new Point(180, 100), Size = new Size(75, 25) };
            btnCancel.Click += (sender, e) => { this.DialogResult = DialogResult.Cancel; };
            this.CancelButton = btnCancel;

            this.Controls.Add(lblLength);
            this.Controls.Add(txtLength);
            this.Controls.Add(lblWidth);
            this.Controls.Add(txtWidth);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                this.SelectNextControl((Control)sender, true, true, true, true);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                this.SelectNextControl((Control)sender, false, true, true, true);
                e.SuppressKeyPress = true;
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtLength.Text, out double length) && double.TryParse(txtWidth.Text, out double width))
            {
                TableLength = length;
                TableWidth = width;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("请输入有效的数字。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
