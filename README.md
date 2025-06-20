本项目为 AutoCAD 2018 开发的 .dll 插件，主要用于自动化建模家具部件。

## 编译与加载
- 编译命令：
  ```
  dotnet restore
  dotnet build
  ```
- 编译后生成的 DLL 路径：
  `furniture\bin\Debug\net47\furniture.dll`
- 在 AutoCAD 中通过 LISP 脚本加载：
  ```lisp
  (command "_netload" "D:\BaiduSyncdisk\software\y-furniture\furniture\bin\Debug\net47\furniture.dll")
  (princ)
  ```

## 三轴加工功能实现说明

### 主要逻辑
- 在功能区（Ribbon）新增“三轴加工”面板，包含“绘制图纸”命令按钮。
- 点击“绘制图纸”后，弹出常驻 Palette（停靠面板），面板上以按钮形式列出各种型号的子命令。
- 用户点击不同型号按钮，即可调用对应的绘图命令，自动绘制所选型号的图纸。
- Palette 支持后续扩展，可灵活添加更多型号及其绘图逻辑。

### 相关代码文件
- `RibbonController.cs`：功能区及“三轴加工”面板、按钮的创建。
- `DrawingsPaletteCommandHandler.cs`：Ribbon 按钮命令处理器，负责弹出 Palette。
- `DrawingsPaletteControl.cs`：Palette UI 控件，负责显示型号按钮及处理点击事件。
- （可扩展）如需实现具体型号的绘图命令，可在 `DrawingUtils.cs` 或新建命令类中实现。

# y-furniture 抽屉左侧板自动建模要点
## 主要流程
1. **参数输入**：
   - 长度（length）
   - 高度（height）
   - 板厚（thickness）
   - 圆弧半径（radius，已固定为1）

2. **轮廓绘制**：
   - 在世界坐标XY平面创建多段线，轮廓为：
     - 下边直线
     - 左右边直线
     - 上边为外凸半圆弧（bulge = -1）
   - 板的高度需减去板厚/2，确保外凸圆弧后整体高度正确

3. **面域与拉伸**：
   - 由多段线生成面域
   - 沿Z轴方向拉伸，生成3D实体
   - 拉伸后整体绕X轴+90°旋转，实体最终位于ZX平面，圆弧朝上

4. **清理**：
   - 拉伸后自动删除用于生成的多段线，保持图形整洁

## 注意事项
- **bulge参数**：bulge为-1时，生成的半圆弧为外凸，且圆心在板外侧。
- **圆弧半径**：当前实现中圆弧半径固定为1，如需自定义可调整bulge算法。
- **高度修正**：板高需减去板厚/2，否则外凸圆弧会导致实际高度超出设计值。
- **UCS**：无需UCS变换，所有操作均在世界坐标系下完成。
- **多段线清理**：建模后应及时删除辅助多段线，避免图形杂乱。

## 代码片段参考
```csharp
// 多段线轮廓
pline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0); // 左下
pline.AddVertexAt(1, new Point2d(0, height), -1, 0, 0); // 上半圆弧
pline.AddVertexAt(2, new Point2d(thickness, height), 0, 0, 0); // 右上
pline.AddVertexAt(3, new Point2d(thickness, 0), 0, 0, 0); // 右下
pline.Closed = true;

// 拉伸后旋转到ZX平面
solid.TransformBy(Matrix3d.Rotation(Math.PI/2, Vector3d.XAxis, Point3d.Origin));
```

## 建议
- 如需创建其它带圆弧的板件，可参考本流程，调整轮廓点和bulge参数。
- 若需不同半径或非半圆弧，可用bulge公式：`bulge = tan(弧度/4)`。
- 保持建模流程简洁，便于维护和复用。

## 交互体验建议
- 所有参数输入对话框建议支持“上下箭头”键在输入框之间切换焦点，提升用户输入效率。
  - 示例：在宽度、高度等输入框中，按上下键可循环切换焦点。
  - 代码实现可参考 KeyDown 事件处理方式。
