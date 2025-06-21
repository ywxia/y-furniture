using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Text;
using System.Windows.Forms;
using yz.furniture.Features;
using yz.furniture.UI.Forms;

namespace yz.furniture.UI.Palettes
{
    public partial class PropertyPaletteControl : UserControl
    {
        public PropertyPaletteControl()
        {
            InitializeComponent();
        }

        private void btnEditData_Click(object sender, EventArgs e)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("\n请选择一个要编辑属性的部件: ");
            PromptEntityResult per = ed.GetEntity(peo);

            if (per.Status != PromptStatus.OK) return;

            using (doc.LockDocument())
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    // 尝试读取现有数据
                    DrawerBox box = DrawerBox.ReadDataFromEntity(tr, per.ObjectId);

                    // 如果没有数据，则根据实体大小创建一个新的
                    if (box == null)
                    {
                        Entity ent = tr.GetObject(per.ObjectId, OpenMode.ForRead) as Entity;
                        if (ent != null && ent.Bounds.HasValue)
                        {
                            Extents3d bounds = ent.Bounds.Value;
                            double length = bounds.MaxPoint.X - bounds.MinPoint.X;
                            double width = bounds.MaxPoint.Y - bounds.MinPoint.Y;
                            double height = bounds.MaxPoint.Z - bounds.MinPoint.Z;
                            // 注意：这里的长宽高对应关系可能需要根据实际情况调整
                            box = new DrawerBox(width, height, length, "新零件", "新材质", "新部件");
                            // 关键修复：将新创建的数据对象与用户选择的实体ID关联起来
                            box.AssociateWithEntity(per.ObjectId);
                        }
                        else
                        {
                             MessageBox.Show("无法获取实体尺寸，无法创建新属性。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                             return;
                        }
                    }

                    // 打开编辑表单
                    using (var form = new PropertyEditForm(box))
                    {
                        if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) == DialogResult.OK)
                        {
                            // 使用新的方法将box对象的数据写入实体
                            // 这个方法会自动处理JSON序列化和XData的更新
                            box.WriteDataToEntity(tr, per.ObjectId);
                            
                            ed.WriteMessage("\n属性已成功更新。");
                        }
                    }
                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    ed.WriteMessage($"\n编辑属性时出错: {ex.Message}");
                    tr.Abort();
                }
            }
        }

        private void btnReadData_Click(object sender, EventArgs e)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;

            PromptEntityOptions peo = new PromptEntityOptions("\n请选择一个部件: ");
            PromptEntityResult per = ed.GetEntity(peo);

            if (per.Status != PromptStatus.OK) return;

            using (doc.LockDocument())
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    DrawerBox box = DrawerBox.ReadDataFromEntity(tr, per.ObjectId);
                    if (box != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("部件属性:");
                        sb.AppendLine("--------------------");
                        sb.AppendLine($"部件名称: {box.ComponentName}");
                        sb.AppendLine($"零件名称: {box.Name}");
                        sb.AppendLine($"材质: {box.Material}");
                        sb.AppendLine($"");
                        sb.AppendLine("几何尺寸:");
                        sb.AppendLine($"  - 长度: {box.Length}");
                        sb.AppendLine($"  - 高度: {box.Height}");
                        sb.AppendLine($"  - 板厚: {box.Thickness}");
                        sb.AppendLine($"");
                        sb.AppendLine("下料余量:");
                        sb.AppendLine($"  - X方向: {box.AllowanceX}");
                        sb.AppendLine($"  - Y方向: {box.AllowanceY}");
                        sb.AppendLine($"  - Z方向: {box.AllowanceZ}");

                        MessageBox.Show(sb.ToString(), "部件数据", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("未在此部件上找到有效的属性数据。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (System.Exception ex)
                {
                    ed.WriteMessage($"\n读取数据时出错: {ex.Message}");
                }
            }
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.btnReadData = new System.Windows.Forms.Button();
            this.btnEditData = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnReadData
            // 
            this.btnReadData.Location = new System.Drawing.Point(20, 20);
            this.btnReadData.Name = "btnReadData";
            this.btnReadData.Size = new System.Drawing.Size(150, 23);
            this.btnReadData.TabIndex = 0;
            this.btnReadData.Text = "读取部件属性";
            this.btnReadData.UseVisualStyleBackColor = true;
            this.btnReadData.Click += new System.EventHandler(this.btnReadData_Click);
            // 
            // btnEditData
            // 
            this.btnEditData.Location = new System.Drawing.Point(20, 50);
            this.btnEditData.Name = "btnEditData";
            this.btnEditData.Size = new System.Drawing.Size(150, 23);
            this.btnEditData.TabIndex = 1;
            this.btnEditData.Text = "附加/编辑属性";
            this.btnEditData.UseVisualStyleBackColor = true;
            this.btnEditData.Click += new System.EventHandler(this.btnEditData_Click);
            // 
            // PropertyPaletteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnEditData);
            this.Controls.Add(this.btnReadData);
            this.Name = "PropertyPaletteControl";
            this.Size = new System.Drawing.Size(200, 100);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btnReadData;
        private System.Windows.Forms.Button btnEditData;

        #endregion
    }
}
