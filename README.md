# y-furniture 插件说明

本项目为 AutoCAD 2018 的 .dll 插件，主要用于自动化建模家具部件。

## 目录

1. 编译与加载
2. 三轴加工功能
   - 主要逻辑
   - 相关代码文件
   - 306型餐桌绘制要点
   - 注意事项
   - 关键代码位置
3. 抽屉左侧板自动建模
   - 主要流程
   - 注意事项
   - 代码片段参考
   - 建议
4. 交互体验建议

---

## 1. 编译与加载

- 编译命令：
  ```
  dotnet restore
  dotnet build
  ```
- DLL 路径：`furniture\bin\Debug\net47\furniture.dll`
- AutoCAD 加载方式（LISP 脚本）：
  ```lisp
  (command "_netload" "D:\\BaiduSyncdisk\\software\\y-furniture\\furniture\\bin\\Debug\\net47\\furniture.dll")
  (princ)
  ```

---

## 2. 三轴加工功能

### 主要逻辑

- 在功能区（Ribbon）新增“三轴加工”面板，包含“绘制图纸”命令按钮。
- 点击“绘制图纸”后，弹出常驻 Palette（停靠面板），面板上以按钮形式列出各种型号的子命令。
- 用户点击不同型号按钮，即可调用对应的绘图命令，自动绘制所选型号的图纸。
- Palette 支持后续扩展，可灵活添加更多型号及其绘图逻辑。

### 相关代码文件

- `RibbonController.cs`：功能区及“三轴加工”面板、按钮的创建。
- `DrawingsPaletteCommandHandler.cs`：Ribbon 按钮命令处理器，负责弹出 Palette。
- `DrawingsPaletteControl.cs`：Palette UI 控件，负责显示型号按钮及处理点击事件。
- `DrawingUtils.cs` 或新建命令类：具体型号的绘图命令实现。

### 306型餐桌绘制要点

- 支持交互输入餐桌长度、宽度，输入框可用上下键切换。
- 自动绘制四角为四分之一圆弧的标准圆角矩形，bulge 精确，顺序正确。
- 四角各绘制独立四分之一圆弧，便于三轴加工。
- 四角自动排列两组同心圆（直径9.5/12），并通过 x/y 方向镜像，确保八组分布一致。
- y 方向居中三组同心圆，左右对称镜像，便于定位。
- 自动绘制 4 条水平方向直线，自动排列。
- 所有图元均自动排列、镜像，确保与加工图纸一致。

### 串带门板绘制要点

- **参数化输入**：通过弹出的 `DoorInputForm` 对话框，用户可自定义门板长度、宽度、串带边距、串带下边距和串带长度。对话框支持使用“上/下箭头”键切换焦点。
- **主体绘制**：根据输入参数，首先在原点(0,0,0)绘制门板的矩形外框。
- **串带阵列**：
  - 自动计算串带的排列数量和间距，以确保间距在 `[300, 500]` 的合理范围内。
  - 沿X轴方向，等距离排列多组“串带”（每组由一条直线和一条U型多段线构成）。
  - 对排列距离小于300mm的特殊情况做了处理，保证至少在两端绘制串带。
- **水平线排列**：
  - 在门板内部，沿Y轴方向均匀排列4条水平直线。
  - 直线的水平范围由串带边距和固定值（60mm）共同决定。

### 注意事项

- bulge 参数需精确，圆角顺序与 AutoCAD 多段线方向一致。
- 圆心、切点、镜像算法需严格按图纸逻辑，避免偏差。
- 所有参数均可交互输入，便于不同尺寸扩展。
- Palette UI 支持多型号扩展，便于后续维护。
- 可扩展尺寸标注、图层管理等功能。

### 关键代码位置

- `DrawingUtils.cs`：主建模与绘图逻辑（如 `DrawTable306`、`DrawDoorWithGroove` 方法及辅助函数）。
- `DrawingsPaletteControl.cs`：Palette UI 与型号按钮（如“串带门板绘制”按钮的事件处理）。
- `DoorInputForm.cs`：串带门板的参数输入对话框。
- `DrawingsPaletteCommandHandler.cs`：Palette 命令处理。
- `RibbonController.cs`：功能区与按钮注册。

---

## 3. 抽屉左侧板自动建模

### 主要流程

1. **参数输入**：长度（length）、高度（height）、板厚（thickness）、圆弧半径（radius，已固定为1）
2. **轮廓绘制**：在 XY 平面创建多段线，包含下边、左右边直线，上边为外凸半圆弧（bulge = -1），高度需减去板厚/2
3. **面域与拉伸**：由多段线生成面域，沿 Z 轴拉伸为 3D 实体，拉伸后绕 X 轴 +90° 旋转到 ZX 平面
4. **清理**：拉伸后自动删除辅助多段线

### 注意事项

- bulge 为 -1 时，生成的半圆弧为外凸，圆心在板外侧
- 圆弧半径当前固定为 1，如需自定义可调整 bulge 算法
- 板高需减去板厚/2，否则外凸圆弧会导致实际高度超出设计值
- 所有操作均在世界坐标系下完成，无需 UCS 变换
- 建模后应及时删除辅助多段线

### 代码片段参考

```csharp
// 多段线轮廓
pline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0); // 左下
pline.AddVertexAt(1, new Point2d(0, height), -1, 0, 0); // 上半圆弧
pline.AddVertexAt(2, new Point2d(thickness, height), 0, 0, 0); // 右上
pline.AddVertexAt(3, new Point2d(thickness, 0), 0, 0, 0); // 右下
pline.Closed = true;

// 拉伸后旋转到 ZX 平面
solid.TransformBy(Matrix3d.Rotation(Math.PI/2, Vector3d.XAxis, Point3d.Origin));
```

### 建议

- 创建其它带圆弧的板件时，可参考本流程，调整轮廓点和 bulge 参数
- 若需不同半径或非半圆弧，可用 bulge 公式：`bulge = tan(弧度/4)`
- 保持建模流程简洁，便于维护和复用

---

## 4. 交互体验建议

- 所有参数输入对话框建议支持“上下箭头”键在输入框之间切换焦点，提升输入效率
  - 示例：在宽度、高度等输入框中，按上下键可循环切换焦点
  - 代码实现可参考 KeyDown 事件处理方式

---

如需补充其它重要事项，可继续补充“注意事项”或“建议”部分，保持结构清晰、重点突出。这样便于新成员快速理解项目结构和开发要点。
