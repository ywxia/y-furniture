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
                            MessageBox.Show("请输入有效的数字。\n" + ex.Message);
                            return;
                        }
                    };

                    var cancelButton = new Button() { Text = "取消", Left = 190, Width = 80, Top = 120 };
                    cancelButton.Click += (sender, e) => {
                        form.DialogResult = DialogResult.Cancel;
                        form.Close();
                    };

                    // 支持上下箭头切换输入框
                    KeyEventHandler textBox_KeyDown = (s, ev) => {
                        var currentTextBox = s as TextBox;
                        if (currentTextBox == null) return;
                        if (ev.KeyCode == Keys.Down)
                        {
                            if (currentTextBox == widthTextBox) heightTextBox.Focus();
                            else if (currentTextBox == heightTextBox) widthTextBox.Focus(); // 循环
                            ev.Handled = true;
                            ev.SuppressKeyPress = true;
                        }
                        else if (ev.KeyCode == Keys.Up)
                        {
                            if (currentTextBox == widthTextBox) heightTextBox.Focus(); // 循环
                            else if (currentTextBox == heightTextBox) widthTextBox.Focus();
                            ev.Handled = true;
                            ev.SuppressKeyPress = true;
                        }
                    };
                    widthTextBox.KeyDown += textBox_KeyDown;
                    heightTextBox.KeyDown += textBox_KeyDown;

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

        [CommandMethod("_DrawTable306")]
        public static void DrawTable306()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            using (doc.LockDocument())
            {
                double length = 0;
                double width = 0;
                using (var form = new Form())
                {
                    form.Text = "306型餐桌参数";
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.Width = 300;
                    form.Height = 180;
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.MaximizeBox = false;
                    form.MinimizeBox = false;

                    var lengthLabel = new Label() { Left = 20, Top = 23, Text = "长度:", AutoSize = true };
                    var lengthTextBox = new TextBox() { Left = 80, Top = 20, Width = 180 };
                    var widthLabel = new Label() { Left = 20, Top = 63, Text = "宽度:", AutoSize = true };
                    var widthTextBox = new TextBox() { Left = 80, Top = 60, Width = 180 };
                    var okButton = new Button() { Text = "确定", Left = 100, Width = 80, Top = 100 };
                    okButton.Click += (sender, e) => {
                        try
                        {
                            length = double.Parse(lengthTextBox.Text);
                            width = double.Parse(widthTextBox.Text);
                            form.DialogResult = DialogResult.OK;
                            form.Close();
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("请输入有效的数字。\n" + ex.Message);
                            return;
                        }
                    };
                    var cancelButton = new Button() { Text = "取消", Left = 190, Width = 80, Top = 100 };
                    cancelButton.Click += (sender, e) => {
                        form.DialogResult = DialogResult.Cancel;
                        form.Close();
                    };
                    KeyEventHandler textBox_KeyDown = (s, ev) => {
                        var currentTextBox = s as TextBox;
                        if (currentTextBox == null) return;
                        if (ev.KeyCode == Keys.Down)
                        {
                            if (currentTextBox == lengthTextBox) widthTextBox.Focus();
                            else if (currentTextBox == widthTextBox) lengthTextBox.Focus();
                            ev.Handled = true;
                            ev.SuppressKeyPress = true;
                        }
                        else if (ev.KeyCode == Keys.Up)
                        {
                            if (currentTextBox == lengthTextBox) widthTextBox.Focus();
                            else if (currentTextBox == widthTextBox) lengthTextBox.Focus();
                            ev.Handled = true;
                            ev.SuppressKeyPress = true;
                        }
                    };
                    lengthTextBox.KeyDown += textBox_KeyDown;
                    widthTextBox.KeyDown += textBox_KeyDown;
                    form.Controls.Add(lengthLabel);
                    form.Controls.Add(lengthTextBox);
                    form.Controls.Add(widthLabel);
                    form.Controls.Add(widthTextBox);
                    form.Controls.Add(okButton);
                    form.Controls.Add(cancelButton);
                    form.AcceptButton = okButton;
                    form.CancelButton = cancelButton;
                    if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) != DialogResult.OK)
                        return;
                }
                Database db = doc.Database;
                Editor ed = doc.Editor;
                Point2d p0 = new Point2d(1, 1); // 左下
                double r = 35.0;
                // 四角圆心
                Point2d c0 = new Point2d(p0.X + r, p0.Y + r); // 左下
                Point2d c1 = new Point2d(p0.X + length - r, p0.Y + r); // 右下
                Point2d c2 = new Point2d(p0.X + length - r, p0.Y + width - r); // 右上
                Point2d c3 = new Point2d(p0.X + r, p0.Y + width - r); // 左上
                // 8个切点
                Point2d[] pts = new Point2d[8];
                pts[0] = new Point2d(c0.X, p0.Y); // 下边左
                pts[1] = new Point2d(c1.X, p0.Y); // 下边右
                pts[2] = new Point2d(p0.X + length, c1.Y); // 右边下
                pts[3] = new Point2d(p0.X + length, c2.Y); // 右边上
                pts[4] = new Point2d(c2.X, p0.Y + width); // 上边右
                pts[5] = new Point2d(c3.X, p0.Y + width); // 上边左
                pts[6] = new Point2d(p0.X, c3.Y); // 左边上
                pts[7] = new Point2d(p0.X, c0.Y); // 左边下
                // bulge = tan(PI/8) = 0.4142
                double bulge = Math.Tan(Math.PI / 8);
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    Polyline pl = new Polyline();
                    // 顺时针依次：下直线-右下圆弧-右直线-右上圆弧-上直线-左上圆弧-左直线-左下圆弧
                    pl.AddVertexAt(0, pts[0], 0, 0, 0); // 下边左
                    pl.AddVertexAt(1, pts[1], bulge, 0, 0); // 右下圆弧
                    pl.AddVertexAt(2, pts[2], 0, 0, 0); // 右边下
                    pl.AddVertexAt(3, pts[3], bulge, 0, 0); // 右上圆弧
                    pl.AddVertexAt(4, pts[4], 0, 0, 0); // 上边右
                    pl.AddVertexAt(5, pts[5], bulge, 0, 0); // 左上圆弧
                    pl.AddVertexAt(6, pts[6], 0, 0, 0); // 左边上
                    pl.AddVertexAt(7, pts[7], bulge, 0, 0); // 左下圆弧
                    pl.Closed = true;
                    btr.AppendEntity(pl);
                    tr.AddNewlyCreatedDBObject(pl, true);
                    tr.Commit();
                    ed.WriteMessage($"\n已在 (1,1,0) 绘制 {length}x{width} 的306型餐桌，并四角倒圆角r=35。");
                }
                // 额外绘制四个独立圆角（四分之一圆弧）
                using (Transaction tr2 = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr2.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr2.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    // 左下
                    AddQuarterArc(btr, new Point2d(1 + r, 1 + r), r, Math.PI, 1.5 * Math.PI);
                    // 右下
                    AddQuarterArc(btr, new Point2d(1 + length - r, 1 + r), r, 1.5 * Math.PI, 0);
                    // 右上
                    AddQuarterArc(btr, new Point2d(1 + length - r, 1 + width - r), r, 0, 0.5 * Math.PI);
                    // 左上
                    AddQuarterArc(btr, new Point2d(1 + r, 1 + width - r), r, 0.5 * Math.PI, Math.PI);
                    tr2.Commit();
                }
                // 画四角八组同心圆（x、y方向镜像）
                using (Transaction tr3 = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr3.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr3.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    double cx = 1 + 170;
                    double cy = 1 + 170;
                    double angle = 45.0 * Math.PI / 180.0;
                    double x2 = cx + 160 * Math.Cos(angle);
                    double y2 = cy + 160 * Math.Sin(angle);
                    // 获取矩形中心
                    double centerX = 1 + length / 2.0;
                    double centerY = 1 + width / 2.0;
                    // 原始两组
                    Point3d[] baseCenters = new Point3d[] {
                        new Point3d(cx, cy, 0),
                        new Point3d(x2, y2, 0)
                    };
                    // 镜像函数
                    Point3d MirrorX(Point3d pt) => new Point3d(2 * centerX - pt.X, pt.Y, 0);
                    Point3d MirrorY(Point3d pt) => new Point3d(pt.X, 2 * centerY - pt.Y, 0);
                    Point3d MirrorXY(Point3d pt) => new Point3d(2 * centerX - pt.X, 2 * centerY - pt.Y, 0);
                    // 所有八个圆心
                    var allCenters = new System.Collections.Generic.List<Point3d>();
                    foreach (var pt in baseCenters)
                    {
                        allCenters.Add(pt);
                        allCenters.Add(MirrorX(pt));
                        allCenters.Add(MirrorY(pt));
                        allCenters.Add(MirrorXY(pt));
                    }
                    // 去重（防止重合）
                    var uniqueCenters = new System.Collections.Generic.List<Point3d>();
                    foreach (var pt in allCenters)
                    {
                        bool exists = false;
                        foreach (var u in uniqueCenters)
                        {
                            if (Math.Abs(u.X - pt.X) < 1e-6 && Math.Abs(u.Y - pt.Y) < 1e-6)
                            { exists = true; break; }
                        }
                        if (!exists) uniqueCenters.Add(pt);
                    }
                    // 绘制
                    foreach (var pt in uniqueCenters)
                        AddDoubleCircle(btr, pt, 9.5 / 2, 12 / 2);
                    tr3.Commit();
                }
                // 画三组同心圆并y轴镜像（中间组y=(1+width)/2，其余±260）
                using (Transaction tr4 = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr4.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr4.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    double xL = 1 + 435;
                    double xR = 1 + length - 435;
                    double yCenter = 1 + width / 2.0;
                    double[] ys = new double[] { yCenter - 260, yCenter, yCenter + 260 };
                    foreach (var y in ys)
                    {
                        // 左侧
                        AddDoubleCircle(btr, new Point3d(xL, y, 0), 9.5 / 2, 12 / 2);
                        // 右侧镜像
                        AddDoubleCircle(btr, new Point3d(xR, y, 0), 9.5 / 2, 12 / 2);
                    }
                    tr4.Commit();
                }
                // 画水平方向多条直线（4条，间距150）
                using (Transaction tr5 = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr5.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr5.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    double startX = 236;
                    double baseY = 1 + width / 2.0 - 225;
                    double lineLen = length - 470;
                    int lineCount = 4;
                    double gap = 150;
                    for (int i = 0; i < lineCount; i++)
                    {
                        double y = baseY + i * gap;
                        var line = new Autodesk.AutoCAD.DatabaseServices.Line(
                            new Point3d(startX, y, 0),
                            new Point3d(startX + lineLen, y, 0)
                        );
                        btr.AppendEntity(line);
                        tr5.AddNewlyCreatedDBObject(line, true);
                    }
                    tr5.Commit();
                }
            }
        }
        // 计算偏移点
        private static Point2d OffsetPoint(Point2d from, Point2d to, double dist)
        {
            Vector2d v = to - from;
            v = v.GetNormal() * dist;
            return from + v;
        }
        // 计算标准圆角bulge
        private static double GetFilletBulge(double r, Point2d corner, Point2d next, Point2d prev)
        {
            Vector2d v1 = (prev - corner).GetNormal();
            Vector2d v2 = (next - corner).GetNormal();
            double angle = (Math.PI - v1.GetAngleTo(v2)) / 2.0;
            return Math.Tan(angle / 2.0);
        }
        // 绘制四分之一圆弧
        private static void AddQuarterArc(BlockTableRecord btr, Point2d center, double r, double startAngle, double endAngle)
        {
            using (var arc = new Autodesk.AutoCAD.DatabaseServices.Arc(
                new Point3d(center.X, center.Y, 0), r, startAngle, endAngle))
            {
                btr.AppendEntity(arc);
                arc.Database.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(arc, true);
            }
        }
        // 绘制一组同心圆
        private static void AddDoubleCircle(BlockTableRecord btr, Point3d center, double r1, double r2)
        {
            using (var c1 = new Autodesk.AutoCAD.DatabaseServices.Circle(center, Vector3d.ZAxis, r1))
            using (var c2 = new Autodesk.AutoCAD.DatabaseServices.Circle(center, Vector3d.ZAxis, r2))
            {
                btr.AppendEntity(c1);
                btr.AppendEntity(c2);
                c1.Database.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(c1, true);
                    c2.Database.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(c2, true);
            }
        }

        public static void DrawDoorWithGroove(double length, double width, double sideMargin, double grooveBottom, double grooveLength)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 绘制门板外框
                Point2d pt1 = new Point2d(0, 0);
                Point2d pt2 = new Point2d(length, 0);
                Point2d pt3 = new Point2d(length, width);
                Point2d pt4 = new Point2d(0, width);

                Polyline pl = new Polyline();
                pl.AddVertexAt(0, pt1, 0, 0, 0);
                pl.AddVertexAt(1, pt2, 0, 0, 0);
                pl.AddVertexAt(2, pt3, 0, 0, 0);
                pl.AddVertexAt(3, pt4, 0, 0, 0);
                pl.Closed = true;

                btr.AppendEntity(pl);
                tr.AddNewlyCreatedDBObject(pl, true);

                // 计算排列参数
                double totalDistance = length - (sideMargin * 2);
                int groupCount;

                // 如果总排列距离小于最小间距要求，则只在两端各放一组
                if (totalDistance < 300)
                {
                    groupCount = 2;
                }
                // 如果总排列距离在[300, 500]之间，也只放两组
                else if (totalDistance <= 500)
                {
                    groupCount = 2;
                }
                else
                {
                    double minSpacing = 300;
                    double maxSpacing = 500;
                    int minGroups = (int)Math.Ceiling(totalDistance / maxSpacing) + 1;
                    int maxGroups = (int)Math.Floor(totalDistance / minSpacing) + 1;
                    groupCount = (int)Math.Round((double)(minGroups + maxGroups) / 2.0);
                }

                if (groupCount < 2) groupCount = 2;
                
                double spacing = totalDistance / (groupCount - 1);

                // 循环绘制串带
                for (int i = 0; i < groupCount; i++)
                {
                    double currentX = sideMargin + i * spacing;
                    DrawGrooveSet(btr, tr, new Point3d(currentX, grooveBottom, 0), grooveLength);
                }

                // 绘制4条水平线
                double lineSpacingY = width / 5.0;
                double lineStartX = sideMargin + 60;
                double lineEndX = length - sideMargin - 60;

                for (int i = 1; i <= 4; i++)
                {
                    double currentY = i * lineSpacingY;
                    Point3d startPt = new Point3d(lineStartX, currentY, 0);
                    Point3d endPt = new Point3d(lineEndX, currentY, 0);
                    Line horizontalLine = new Line(startPt, endPt);
                    btr.AppendEntity(horizontalLine);
                    tr.AddNewlyCreatedDBObject(horizontalLine, true);
                }

                tr.Commit();

                ed.WriteMessage($"\n已绘制一个 {length}x{width} 的门板，并添加了 {groupCount} 组串带和4条水平线。");
            }
        }

        private static void DrawGrooveSet(BlockTableRecord btr, Transaction tr, Point3d startPoint, double grooveLength)
        {
            double grooveBottom = startPoint.Y;
            double sideMargin = startPoint.X;

            // 1. 画直线
            Point3d lineStart = new Point3d(sideMargin, grooveBottom, 0);
            Point3d lineEnd = new Point3d(sideMargin, grooveBottom + grooveLength, 0);
            Line line = new Line(lineStart, lineEnd);
            btr.AppendEntity(line);
            tr.AddNewlyCreatedDBObject(line, true);

            // 2. 画多段线
            Polyline pLine2 = new Polyline();
            pLine2.AddVertexAt(0, new Point2d(sideMargin - 0.05, grooveBottom + grooveLength), 0, 0, 0);
            pLine2.AddVertexAt(1, new Point2d(sideMargin - 0.05, grooveBottom), 0, 0, 0);
            pLine2.AddVertexAt(2, new Point2d(sideMargin + 0.05, grooveBottom), 0, 0, 0);
            pLine2.AddVertexAt(3, new Point2d(sideMargin + 0.05, grooveBottom + grooveLength), 0, 0, 0);
            btr.AppendEntity(pLine2);
            tr.AddNewlyCreatedDBObject(pLine2, true);
        }
    }
}
