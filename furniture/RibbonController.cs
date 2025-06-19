using Autodesk.Windows;
using System;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;

namespace furniture
{
    public class RibbonController
    {
        public static void CreateRibbon()
        {
            RibbonControl ribbon = ComponentManager.Ribbon;
            if (ribbon == null) return;

            RibbonTab tab = ribbon.FindTab("FURNITURE_DESIGN_TAB");
            if (tab != null)
            {
                ribbon.Tabs.Remove(tab);
            }

            tab = new RibbonTab();
            tab.Title = "家具设计";
            tab.Id = "FURNITURE_DESIGN_TAB";
            ribbon.Tabs.Add(tab);

            CreateGeneralPartsPanel(tab);
            CreateFurniturePartsPanel(tab);
        }

        private static void CreateFurniturePartsPanel(RibbonTab tab)
        {
            RibbonPanelSource panelSource = new RibbonPanelSource();
            panelSource.Title = "家具部件";
            RibbonPanel panel = new RibbonPanel();
            panel.Source = panelSource;
            tab.Panels.Add(panel);

            RibbonButton button = new RibbonButton();
            button.Name = "抽屉框制作";
            button.ShowText = true;
            button.Text = "抽屉框制作";
            button.CommandHandler = new CreateDrawerBoxCommandHandler();

            panelSource.Items.Add(button);
        }

        private static void CreateGeneralPartsPanel(RibbonTab tab)
        {
            RibbonPanelSource panelSource = new RibbonPanelSource();
            panelSource.Title = "通用件";
            RibbonPanel panel = new RibbonPanel();
            panel.Source = panelSource;
            tab.Panels.Add(panel);

            RibbonButton button = new RibbonButton();
            button.Name = "画制矩形";
            button.ShowText = true;
            button.Text = "画制矩形";
            // 直接关联命令处理器，而不是发送字符串
            button.CommandHandler = new DrawRectangleCommandHandler();

            panelSource.Items.Add(button);

            RibbonButton boxButton = new RibbonButton();
            boxButton.Name = "画制长方体";
            boxButton.ShowText = true;
            boxButton.Text = "画制长方体";
            boxButton.CommandHandler = new DrawBoxCommandHandler();
            panelSource.Items.Add(boxButton);
        }
    }

    // 为“画制矩形”按钮创建的特定命令处理器
    public class DrawRectangleCommandHandler : ICommand
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
                    using (doc.LockDocument())
                    {
                        doc.SendStringToExecute("_DrawRectangle ", true, false, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Application.ShowAlertDialog($"执行命令出错：{ex.Message}");
            }
        }
    }

    public class DrawBoxCommandHandler : ICommand
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
                    using (doc.LockDocument())
                    {
                        doc.SendStringToExecute("_DrawBox ", true, false, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Application.ShowAlertDialog($"执行命令出错：{ex.Message}");
            }
        }
    }
}
