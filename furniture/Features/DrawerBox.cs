using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yz.furniture.Features
{
    /// <summary>
    /// 代表一个抽屉侧板的数据模型。
    /// 这个类包含了创建侧板所需的所有几何参数和附加业务数据。
    /// </summary>
    public class DrawerBox
    {
        #region Properties

        // --- 附加数据 (Metadata) ---
        public string ComponentName { get; set; } // 部件名称 (e.g., "餐桌")
        public string Name { get; set; } // 零件名称 (e.g., "左侧板")
        public string Material { get; set; }

        // --- 加工参数 (Machining Parameters) ---
        public double AllowanceX { get; set; }
        public double AllowanceY { get; set; }
        public double AllowanceZ { get; set; }

        // --- 几何参数 (Parameters) ---
        public double Length { get; set; }
        public double Height { get; set; }
        public double Thickness { get; set; }

        // --- CAD数据库中的实体ID (Entity Reference) ---
        /// <summary>
        /// 存储此对象在AutoCAD数据库中对应的Solid3d实体的ID。
        /// 这个ID是运行时的引用，不应被序列化。
        /// </summary>
        [JsonIgnore]
        public ObjectId EntityId { get; private set; }

        /// <summary>
        /// 将此数据对象与一个已存在的CAD实体进行关联。
        /// </summary>
        /// <param name="entityId">要关联的实体的ObjectId。</param>
        public void AssociateWithEntity(ObjectId entityId)
        {
            this.EntityId = entityId;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// 初始化一个新的 DrawerBox 数据对象。
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="height">高度</param>
        /// <param name="thickness">板厚</param>
        /// <param name="name">零件名称</param>
        /// <param name="material">材质</param>
        public DrawerBox(double length, double height, double thickness, string name = "抽屉侧板", string material = "默认材质", string componentName = "未命名部件")
        {
            Length = length;
            Height = height;
            Thickness = thickness;
            Name = name;
            Material = material;
            ComponentName = componentName;

            // 默认余量为0
            AllowanceX = 0;
            AllowanceY = 0;
            AllowanceZ = 0;

            EntityId = ObjectId.Null; // 初始状态下没有对应的实体
        }

        #endregion

        #region Methods for Drawing and Data Handling

        /// <summary>
        /// 根据当前对象的属性，在AutoCAD中创建或更新一个三维实体。
        /// </summary>
        /// <param name="tr">活动的数据库事务</param>
        /// <param name="btr">要添加实体的块表记录（通常是模型空间）</param>
        /// <returns>成功则返回新创建实体的ObjectId，失败则返回ObjectId.Null。</returns>
        public ObjectId CreateEntity(Transaction tr, BlockTableRecord btr)
        {
            // 如果已经有一个实体了，先删除它，以便重新创建
            if (!EntityId.IsNull && !EntityId.IsErased)
            {
                var oldEntity = tr.GetObject(EntityId, OpenMode.ForWrite);
                oldEntity.Erase();
            }

            // 板高进行微调，同原逻辑
            double adjustedHeight = this.Height - this.Thickness / 2.0;

            // 创建轮廓线（在XY平面）
            Polyline pline = new Polyline();
            pline.SetDatabaseDefaults();
            pline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);                     // 左下角
            pline.AddVertexAt(1, new Point2d(0, adjustedHeight), -1, 0, 0);       // 左上角到右上角的半圆弧
            pline.AddVertexAt(2, new Point2d(this.Thickness, adjustedHeight), 0, 0, 0); // 右上角
            pline.AddVertexAt(3, new Point2d(this.Thickness, 0), 0, 0, 0);         // 右下角
            pline.Closed = true;

            // 创建面域
            DBObjectCollection curves = new DBObjectCollection();
            curves.Add(pline);
            DBObjectCollection regions = Region.CreateFromCurves(curves);
            
            if (regions.Count > 0)
            {
                Region region = regions[0] as Region;
                
                // 拉伸面域创建实体
                Solid3d solid = new Solid3d();
                solid.SetDatabaseDefaults();
                solid.Extrude(region, this.Length, 0.0);

                // 旋转到ZX平面（绕X轴+90°）
                Matrix3d toZX = Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin);
                solid.TransformBy(toZX);

                // 将新创建的实体添加到块表记录和事务中
                btr.AppendEntity(solid);
                tr.AddNewlyCreatedDBObject(solid, true);

                // 清理临时对象
                pline.Dispose();
                region.Dispose();
                
                // 记录实体ID，便于后续关联
                this.EntityId = solid.ObjectId;

                // 返回新实体的ID
                return solid.ObjectId;
            }

            pline.Dispose();
            return ObjectId.Null;
        }

        private const string AppName = "YZ_FURNITURE_DATA";
        private const int MaxXDataStringLength = 255;

        /// <summary>
        /// 将当前DrawerBox对象的数据序列化为JSON，并写入到指定实体的XData中。
        /// 此方法会覆盖实体上原有的YZ_FURNITURE_DATA，但保留其他应用程序的XData。
        /// </summary>
        /// <param name="tr">活动的数据库事务</param>
        /// <param name="entityId">要附加数据的实体的ID</param>
        public void WriteDataToEntity(Transaction tr, ObjectId entityId)
        {
            // 确保AppName已在数据库中注册
            Database db = entityId.Database;
            RegAppTable rat = tr.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
            if (!rat.Has(AppName))
            {
                rat.UpgradeOpen();
                var ratr = new RegAppTableRecord { Name = AppName };
                rat.Add(ratr);
                tr.AddNewlyCreatedDBObject(ratr, true);
            }

            DBObject obj = tr.GetObject(entityId, OpenMode.ForWrite);

            // 序列化当前对象为JSON字符串
            string jsonData = JsonConvert.SerializeObject(this);

            // 创建一个新的XData列表
            var newXData = new List<TypedValue>();

            // 1. 添加我们的AppName和JSON数据
            newXData.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, AppName));
            // 将JSON字符串分割成多个255字符的块进行存储
            for (int i = 0; i < jsonData.Length; i += MaxXDataStringLength)
            {
                string chunk = jsonData.Substring(i, Math.Min(MaxXDataStringLength, jsonData.Length - i));
                newXData.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, chunk));
            }

            // 2. 保留其他应用程序的XData
            ResultBuffer oldRb = obj.XData;
            if (oldRb != null)
            {
                bool ourDataBlock = false;
                foreach (TypedValue tv in oldRb)
                {
                    if (tv.TypeCode == (int)DxfCode.ExtendedDataRegAppName)
                    {
                        ourDataBlock = (tv.Value.ToString() == AppName);
                    }
                    if (!ourDataBlock)
                    {
                        newXData.Add(tv);
                    }
                }
                oldRb.Dispose();
            }

            obj.XData = new ResultBuffer(newXData.ToArray());
        }

        /// <summary>
        /// 从一个给定的CAD实体中读取JSON格式的XData，并创建一个DrawerBox对象。
        /// </summary>
        /// <param name="tr">活动的数据库事务</param>
        /// <param name="entityId">包含数据的实体的ID</param>
        /// <returns>填充了数据的DrawerBox对象，如果读取失败则返回null。</returns>
        public static DrawerBox ReadDataFromEntity(Transaction tr, ObjectId entityId)
        {
            if (entityId.IsNull || entityId.IsErased) return null;

            DBObject obj = tr.GetObject(entityId, OpenMode.ForRead);
            ResultBuffer rb = obj.XData;

            if (rb == null) return null;

            DrawerBox drawerBox = null;
            try
            {
                var values = rb.AsArray();
                bool foundApp = false;
                var jsonBuilder = new StringBuilder();

                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i].TypeCode == (int)DxfCode.ExtendedDataRegAppName)
                    {
                        if (values[i].Value.ToString() == AppName)
                        {
                            foundApp = true;
                        }
                        else
                        {
                            // 如果我们已经找到了自己的AppName，但又遇到了另一个AppName，说明我们的数据块结束了
                            if (foundApp) break;
                        }
                    }
                    else if (foundApp && values[i].TypeCode == (int)DxfCode.ExtendedDataAsciiString)
                    {
                        jsonBuilder.Append(values[i].Value.ToString());
                    }
                }

                if (foundApp && jsonBuilder.Length > 0)
                {
                    drawerBox = JsonConvert.DeserializeObject<DrawerBox>(jsonBuilder.ToString());
                    if (drawerBox != null)
                    {
                        drawerBox.AssociateWithEntity(entityId);
                    }
                }
            }
            catch
            {
                drawerBox = null; // 解析失败
            }
            finally
            {
                rb.Dispose();
            }

            return drawerBox;
        }

        #endregion
    }
}
