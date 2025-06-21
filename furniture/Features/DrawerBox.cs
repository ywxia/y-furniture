using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Windows.Input;

namespace yz.furniture.Features
{
    public class DrawerBox
    {
        public static void CreateDrawerBox()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;

            // 确保在模型空间中操作
            if (Convert.ToInt16(Application.GetSystemVariable("TILEMODE")) == 0)
            {
                Application.SetSystemVariable("TILEMODE", 1);
            }

            using (doc.LockDocument())
            {
                // 获取长、宽、高
                PromptDoubleResult lengthResult = ed.GetDouble("\n请输入长度 (length): ");
                if (lengthResult.Status != PromptStatus.OK) return;
                double length = lengthResult.Value;

                PromptDoubleResult heightResult = ed.GetDouble("\n请输入高度 (height): ");
                if (heightResult.Status != PromptStatus.OK) return;
                double height = heightResult.Value;

                // 获取板厚
                PromptDoubleResult thicknessResult = ed.GetDouble("\n请输入板厚 (thickness): ");
                if (thicknessResult.Status != PromptStatus.OK) return;
                double thickness = thicknessResult.Value;

                // 圆角半径直接设为1
                double radius = 1.0;

                // 板高减去板厚/2
                height = height - thickness / 2.0;

                Database db = doc.Database;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    try
                    {
                        // 创建轮廓线（在XY平面）
                        Polyline pline = new Polyline();
                        pline.SetDatabaseDefaults();
                        pline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);                     // 左下角
                        pline.AddVertexAt(1, new Point2d(0, height), -1, 0, 0);               // 左上角到右上角的半圆弧，bulge为-1向外凸
                        pline.AddVertexAt(2, new Point2d(thickness, height), 0, 0, 0);        // 右上角
                        pline.AddVertexAt(3, new Point2d(thickness, 0), 0, 0, 0);             // 右下角
                        pline.Closed = true;

                        btr.AppendEntity(pline);
                        tr.AddNewlyCreatedDBObject(pline, true);

                        // 创建面域
                        DBObjectCollection curves = new DBObjectCollection();
                        curves.Add(pline);
                        DBObjectCollection regions = Region.CreateFromCurves(curves);
                        
                        if (regions.Count > 0)
                        {
                            Region region = regions[0] as Region;
                            // 拉伸面域创建实体（沿Y轴方向拉伸）
                            Solid3d solid = new Solid3d();
                            solid.SetDatabaseDefaults();
                            solid.Extrude(region, length, 0.0);
                            // 旋转到ZX平面（绕X轴+90°）
                            Matrix3d toZX = Matrix3d.Rotation(Math.PI/2, Vector3d.XAxis, Point3d.Origin);
                            solid.TransformBy(toZX);
                            // 添加实体到图形中
                            btr.AppendEntity(solid);
                            tr.AddNewlyCreatedDBObject(solid, true);
                            // 删除用于生成的多段线
                            pline.Erase();
                            region.Dispose();
                        }
                        else
                        {
                            ed.WriteMessage("\n无法从多段线创建面域");
                        }

                        tr.Commit();
                        ed.WriteMessage("\n左侧板创建成功。");
                    }
                    catch (System.Exception ex)
                    {
                        ed.WriteMessage("\n创建失败: " + ex.Message);
                        tr.Abort();
                    }
                }
            }
        }
    }
}
