using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Windows.Forms;
using yz.furniture.Features;

namespace yz.furniture.UI.Palettes
{
    public partial class ComponentsPaletteControl : UserControl
    {
        public ComponentsPaletteControl()
        {
            InitializeComponent();
        }

        private void btnDrawerBox_Click(object sender, EventArgs e)
        {
            // 获取当前文档和编辑器
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            // 执行抽屉框制作命令
            DrawerBox.CreateDrawerBox();
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.btnDrawerBox = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDrawerBox
            // 
            this.btnDrawerBox.Location = new System.Drawing.Point(20, 20);
            this.btnDrawerBox.Name = "btnDrawerBox";
            this.btnDrawerBox.Size = new System.Drawing.Size(150, 23);
            this.btnDrawerBox.TabIndex = 0;
            this.btnDrawerBox.Text = "抽屉框制作";
            this.btnDrawerBox.UseVisualStyleBackColor = true;
            this.btnDrawerBox.Click += new System.EventHandler(this.btnDrawerBox_Click);
            // 
            // ComponentsPaletteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDrawerBox);
            this.Name = "ComponentsPaletteControl";
            this.Size = new System.Drawing.Size(200, 100);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnDrawerBox;

        #endregion
    }
}
