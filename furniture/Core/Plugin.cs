using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using System;
using yz.furniture.Core;

[assembly: ExtensionApplication(typeof(yz.furniture.Core.Plugin))]

namespace yz.furniture.Core
{
    public class Plugin : IExtensionApplication
    {
        public void Initialize()
        {
            // 不要立即创建UI，而是订阅Application.Idle事件
            Application.Idle += new EventHandler(Application_OnIdle);
        }

        private void Application_OnIdle(object sender, EventArgs e)
        {
            // 在第一次空闲事件触发时创建UI
            // 然后立即取消订阅，避免重复执行
            Application.Idle -= new EventHandler(Application_OnIdle);
            RibbonController.CreateRibbon();
        }

        public void Terminate()
        {
            // Cleanup
        }

        // 这些命令方法已经在各自的类中定义，这里不需要重复定义
        // DrawingUtils.DrawRectangle() -> [CommandMethod("_DrawRectangle")]
        // DrawingUtils.DrawBox() -> [CommandMethod("_DrawBox")] 
        // DrawerBox.CreateDrawerBox() -> [CommandMethod("_CreateDrawerBox")]
    }
}
