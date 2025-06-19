using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Windows.Input;

namespace furniture
{
    public class DrawerBox
    {
        [CommandMethod("_CreateDrawerBox")]
        public static void CreateDrawerBox()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;

            // 确保在模型空间中操作
            if (Convert.ToInt16(Application.GetSystemVariable("TILEMODE")) == 1)
            {
                Application.SetSystemVariable("TILEMODE", 0);
            }

            using (doc.LockDocument())
            {
                // 获取长、宽、高
                PromptDoubleResult lengthResult = ed.GetDouble("\n请输入长度 (length): ");
                if (lengthResult.Status != PromptStatus.OK) return;
                double length = lengthResult.Value;

                PromptDoubleResult widthResult = ed.GetDouble("\n请输入宽度 (width): ");
                if (widthResult.Status != PromptStatus.OK) return;
                double width = widthResult.Value;

                PromptDoubleResult heightResult = ed.GetDouble("\n请输入高度 (height): ");
                if (heightResult.Status != PromptStatus.OK) return;
                double height = heightResult.Value;

                // 获取板厚
                PromptDoubleResult thicknessResult = ed.GetDouble("\n请输入板厚 (thickness): ");
                if (thicknessResult.Status != PromptStatus.OK) return;
                double thickness = thicknessResult.Value;

                Database db = doc.Database;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                        BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                        // Box 1: 左侧板, 原点(0,0,0)
                        Solid3d box1 = new Solid3d();
                        box1.SetDatabaseDefaults();
                        box1.CreateBox(thickness, length, height);

                        // Box 2: 右侧板
                        Solid3d box2 = new Solid3d();
                        box2.SetDatabaseDefaults();
                        box2.CreateBox(thickness, length, height);
                        box2.TransformBy(Matrix3d.Displacement(new Vector3d(width - thickness, 0, 0)));

                        // Box 3: 前板
                        Solid3d box3 = new Solid3d();
                        box3.SetDatabaseDefaults();
                        box3.CreateBox(width - 2 * thickness, thickness, height);
                        box3.TransformBy(Matrix3d.Displacement(new Vector3d(thickness, 0, 0)));

                        // Box 4: 后板
                        Solid3d box4 = new Solid3d();
                        box4.SetDatabaseDefaults();
                        box4.CreateBox(width - 2 * thickness, thickness, height);
                        box4.TransformBy(Matrix3d.Displacement(new Vector3d(thickness, length - thickness, 0)));

                        // Box 5: 底板
                        Solid3d box5 = new Solid3d();
                        box5.SetDatabaseDefaults();
                        box5.CreateBox(width - 2 * thickness, length, thickness);
                        box5.TransformBy(Matrix3d.Displacement(new Vector3d(thickness, 0, 0)));

                        btr.AppendEntity(box1);
                        tr.AddNewlyCreatedDBObject(box1, true);
                        btr.AppendEntity(box2);
                        tr.AddNewlyCreatedDBObject(box2, true);
                        btr.AppendEntity(box3);
                        tr.AddNewlyCreatedDBObject(box3, true);
                        btr.AppendEntity(box4);
                        tr.AddNewlyCreatedDBObject(box4, true);
                        btr.AppendEntity(box5);
                        tr.AddNewlyCreatedDBObject(box5, true);

                        tr.Commit();
                        ed.WriteMessage("\n抽屉框创建成功。");
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

    public class CreateDrawerBoxCommandHandler : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            try
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                if (doc != null)
                {
                    doc.SendStringToExecute("_CreateDrawerBox ", true, false, true);
                }
            }
            catch (System.Exception ex)
            {
                Application.ShowAlertDialog($"命令执行出错：{ex.Message}");
            }
        }
    }
}
