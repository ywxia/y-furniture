using System;
using System.Windows.Forms;

namespace furniture
{
    public class DrawingsPaletteControl : UserControl
    {
        public DrawingsPaletteControl()
        {
            this.Dock = DockStyle.Fill;
            var layout = new FlowLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.FlowDirection = FlowDirection.TopDown;
            layout.WrapContents = false;
            layout.AutoScroll = true;

            // 示例：添加几种型号的按钮，后续可扩展
            AddModelButton(layout, "型号A", OnModelAClick);
            AddModelButton(layout, "型号B", OnModelBClick);
            AddModelButton(layout, "型号C", OnModelCClick);
            AddModelButton(layout, "306型餐桌绘制", OnTable306Click);
            AddModelButton(layout, "串带门板绘制", OnDrawDoorWithGrooveClick);
            AddModelButton(layout, "309型餐桌绘制", OnTable309Click);

            this.Controls.Add(layout);
        }

        private void AddModelButton(FlowLayoutPanel panel, string text, EventHandler handler)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Width = 260;
            btn.Height = 40;
            btn.Margin = new Padding(10, 10, 10, 0);
            btn.Click += handler;
            panel.Controls.Add(btn);
        }

        // 各型号按钮点击事件
        private void OnModelAClick(object sender, EventArgs e)
        {
            MessageBox.Show("执行型号A图纸绘制逻辑");
            // TODO: 调用具体绘图命令
        }
        private void OnModelBClick(object sender, EventArgs e)
        {
            MessageBox.Show("执行型号B图纸绘制逻辑");
        }
        private void OnModelCClick(object sender, EventArgs e)
        {
            MessageBox.Show("执行型号C图纸绘制逻辑");
        }
        private void OnTable306Click(object sender, EventArgs e)
        {
            DrawingUtils.DrawTable306();
        }

        private void OnDrawDoorWithGrooveClick(object sender, EventArgs e)
        {
            using (var form = new DoorInputForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    DrawingUtils.DrawDoorWithGroove(
                        form.DoorLength,
                        form.DoorWidth,
                        form.SideMargin,
                        form.GrooveBottom,
                        form.GrooveLength
                    );
                }
            }
        }

        private void OnTable309Click(object sender, EventArgs e)
        {
            using (var form = new Table309InputForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    DrawingUtils.DrawTable309(form.TableLength, form.TableWidth);
                }
            }
        }
    }
}
