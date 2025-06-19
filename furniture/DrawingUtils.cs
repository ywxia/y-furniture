using System;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace furniture
{
    public static class DrawingUtils
    {
        [CommandMethod("_DrawRectangle")]
        public static void DrawRectangle()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            using (doc.LockDocument())
            {
                double width = 0;
                double height = 0;

                using (var form = new Form())
                {
                    form.Text = "输入矩形参数";
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.Width = 300;
                    form.Height = 220; // Adjusted height
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.MaximizeBox = false;
                    form.MinimizeBox = false;

                    var widthLabel = new Label() { Left = 20, Top = 23, Text = "宽度:", AutoSize = true };
                    var widthTextBox = new TextBox() { Left = 80, Top = 20, Width = 180 };

                    var heightLabel = new Label() { Left = 20, Top = 63, Text = "高度:", AutoSize = true };
                    var heightTextBox = new TextBox() { Left = 80, Top = 60, Width = 180 };

                    var okButton = new Button() { Text = "确定", Left = 100, Width = 80, Top = 120 };
                    okButton.Click += (sender, e) => {
                        try
                        {
                            width = double.Parse(widthTextBox.Text);
                            height = double.Parse(heightTextBox.Text);
                            form.DialogResult = DialogResult.OK;
                            form.Close();
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("请输入有效的数字。\\n" + ex.Message);
                            return;
                        }
                    };

                    var cancelButton = new Button() { Text = "取消", Left = 190, Width = 80, Top = 120 };
                    cancelButton.Click += (sender, e) => {
                        form.DialogResult = DialogResult.Cancel;
                        form.Close();
                    };

                    form.Controls.Add(widthLabel);
                    form.Controls.Add(widthTextBox);
                    form.Controls.Add(heightLabel);
                    form.Controls.Add(heightTextBox);
                    form.Controls.Add(okButton);
                    form.Controls.Add(cancelButton);
                    form.AcceptButton = okButton;
                    form.CancelButton = cancelButton;

                    if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) != DialogResult.OK)
                    {
                        return;
                    }
                }

                Database db = doc.Database;
                Editor ed = doc.Editor;

                PromptPointOptions ppo = new PromptPointOptions("\n请选择矩形的插入点:");
                PromptPointResult ppr = ed.GetPoint(ppo);
                if (ppr.Status != PromptStatus.OK) return;
                Point3d insertionPoint = ppr.Value;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    Point3d pt1 = insertionPoint;
                    Point3d pt2 = new Point3d(insertionPoint.X + width, insertionPoint.Y, insertionPoint.Z);
                    Point3d pt3 = new Point3d(insertionPoint.X + width, insertionPoint.Y + height, insertionPoint.Z);
                    Point3d pt4 = new Point3d(insertionPoint.X, insertionPoint.Y + height, insertionPoint.Z);

                    Polyline pl = new Polyline();
                    pl.AddVertexAt(0, new Point2d(pt1.X, pt1.Y), 0, 0, 0);
                    pl.AddVertexAt(1, new Point2d(pt2.X, pt2.Y), 0, 0, 0);
                    pl.AddVertexAt(2, new Point2d(pt3.X, pt3.Y), 0, 0, 0);
                    pl.AddVertexAt(3, new Point2d(pt4.X, pt4.Y), 0, 0, 0);
                    pl.Closed = true;

                    btr.AppendEntity(pl);
                    tr.AddNewlyCreatedDBObject(pl, true);
                    tr.Commit();

                    ed.WriteMessage($"\n已在 ({Math.Round(pt1.X, 2)}, {Math.Round(pt1.Y, 2)}) 绘制 {width}x{height} 的矩形。");
                }
            }
        }

        [CommandMethod("_DrawBox")]
        public static void DrawBox()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;

            using (doc.LockDocument())
            {
                double width = 0;
                double depth = 0;
                double height = 0;

                using (var form = new Form())
                {
                    form.Text = "输入长方体参数";
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.Width = 420;
                    form.Height = 240;
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.MaximizeBox = false;
                    form.MinimizeBox = false;

                    var widthLabel = new Label() { Left = 20, Top = 23, Text = "宽度(X):", AutoSize = true };
                    var widthTextBox = new TextBox() { Left = 120, Top = 20, Width = 180, TabIndex = 0 };
                    var widthMeasureBtn = new Button() { Text = "...", Left = 310, Top = 20, Width = 30, TabIndex = 1 };

                    var depthLabel = new Label() { Left = 20, Top = 63, Text = "深度(Y):", AutoSize = true };
                    var depthTextBox = new TextBox() { Left = 120, Top = 60, Width = 180, TabIndex = 2 };
                    var depthMeasureBtn = new Button() { Text = "...", Left = 310, Top = 60, Width = 30, TabIndex = 3 };

                    var heightLabel = new Label() { Left = 20, Top = 103, Text = "高度(Z):", AutoSize = true };
                    var heightTextBox = new TextBox() { Left = 120, Top = 100, Width = 180, TabIndex = 4 };
                    var heightMeasureBtn = new Button() { Text = "...", Left = 310, Top = 100, Width = 30, TabIndex = 5 };

                    Action<TextBox> getDistAction = (textBox) => {
                        form.Hide();
                        PromptPointOptions ppo1 = new PromptPointOptions("\n请选择第一点:");
                        ppo1.AllowNone = false;
                        ppo1.Keywords.Clear();
                        PromptPointResult ppr1 = ed.GetPoint(ppo1);
                        if (ppr1.Status != PromptStatus.OK) { form.Show(); form.Activate(); return; }

                        PromptPointOptions ppo2 = new PromptPointOptions("\n请选择第二点:");
                        ppo2.AllowNone = false;
                        ppo2.Keywords.Clear();
                        ppo2.UseBasePoint = true;
                        ppo2.BasePoint = ppr1.Value;
                        PromptPointResult ppr2 = ed.GetPoint(ppo2);
                        if (ppr2.Status != PromptStatus.OK) { form.Show(); form.Activate(); return; }
                        
                        textBox.Text = ppr1.Value.DistanceTo(ppr2.Value).ToString();
                        form.Show();
                        form.Activate();
                    };

                    widthMeasureBtn.Click += (s, e) => getDistAction(widthTextBox);
                    depthMeasureBtn.Click += (s, e) => getDistAction(depthTextBox);
                    heightMeasureBtn.Click += (s, e) => getDistAction(heightTextBox);

                    KeyEventHandler textBox_KeyDown = (s, ev) => {
                        var currentTextBox = s as TextBox;
                        if (currentTextBox == null) return;

                        if (ev.KeyCode == Keys.Down)
                        {
                            if (currentTextBox == widthTextBox) depthTextBox.Focus();
                            else if (currentTextBox == depthTextBox) heightTextBox.Focus();
                            else if (currentTextBox == heightTextBox) widthTextBox.Focus(); // Loop
                            ev.Handled = true;
                            ev.SuppressKeyPress = true;
                        }
                        else if (ev.KeyCode == Keys.Up)
                        {
                            if (currentTextBox == widthTextBox) heightTextBox.Focus(); // Loop
                            else if (currentTextBox == depthTextBox) widthTextBox.Focus();
                            else if (currentTextBox == heightTextBox) depthTextBox.Focus();
                            ev.Handled = true;
                            ev.SuppressKeyPress = true;
                        }
                    };

                    widthTextBox.KeyDown += textBox_KeyDown;
                    depthTextBox.KeyDown += textBox_KeyDown;
                    heightTextBox.KeyDown += textBox_KeyDown;

                    var okButton = new Button() { Text = "确定", Left = 200, Width = 80, Top = 160, TabIndex = 6 };
                    okButton.Click += (sender, e) => {
                        try
                        {
                            width = double.Parse(widthTextBox.Text);
                            depth = double.Parse(depthTextBox.Text);
                            height = double.Parse(heightTextBox.Text);
                            form.DialogResult = DialogResult.OK;
                            form.Close();
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("请输入有效的数字。\n" + ex.Message);
                            return;
                        }
                    };

                    var cancelButton = new Button() { Text = "取消", Left = 290, Width = 80, Top = 160, TabIndex = 7 };
                    cancelButton.Click += (sender, e) => {
                        form.DialogResult = DialogResult.Cancel;
                        form.Close();
                    };
                    
                    form.Shown += (sender, e) => {
                        widthTextBox.Focus();
                    };

                    form.Controls.Add(widthLabel);
                    form.Controls.Add(widthTextBox);
                    form.Controls.Add(widthMeasureBtn);
                    form.Controls.Add(depthLabel);
                    form.Controls.Add(depthTextBox);
                    form.Controls.Add(depthMeasureBtn);
                    form.Controls.Add(heightLabel);
                    form.Controls.Add(heightTextBox);
                    form.Controls.Add(heightMeasureBtn);
                    form.Controls.Add(okButton);
                    form.Controls.Add(cancelButton);
                    form.AcceptButton = okButton;
                    form.CancelButton = cancelButton;

                    if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) != DialogResult.OK)
                    {
                        return;
                    }
                }

                Database db = doc.Database;

                PromptPointOptions ppo = new PromptPointOptions("\n请选择长方体的插入点:");
                ppo.AllowNone = false;
                ppo.Keywords.Clear();
                PromptPointResult ppr = ed.GetPoint(ppo);
                if (ppr.Status != PromptStatus.OK) return;
                Point3d insertionPoint = ppr.Value;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    Solid3d box = new Solid3d();
                    box.CreateBox(width, depth, height);
                    box.TransformBy(Matrix3d.Displacement(insertionPoint - Point3d.Origin + new Vector3d(width / 2, depth / 2, height / 2)));

                    btr.AppendEntity(box);
                    tr.AddNewlyCreatedDBObject(box, true);
                    tr.Commit();

                    ed.WriteMessage($"\n已在 ({Math.Round(insertionPoint.X, 2)}, {Math.Round(insertionPoint.Y, 2)}) 绘制 {width}x{depth}x{height} 的长方体。");
                }
            }
        }
    }
}
