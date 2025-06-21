using Autodesk.Windows;
using System;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;
using yz.furniture.Commands;

namespace yz.furniture.Core
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
            CreatePropertyPanel(tab); // 新增家具属性面板
            CreateCNCPanel(tab); 
        }

        private static void CreateFurniturePartsPanel(RibbonTab tab)
        {
            RibbonPanelSource panelSource = new RibbonPanelSource();
            panelSource.Title = "家具部件";
            RibbonPanel panel = new RibbonPanel();
            panel.Source = panelSource;
            tab.Panels.Add(panel);

            RibbonButton button = new RibbonButton();
            button.Name = "部件制作";
            button.ShowText = true;
            button.Text = "部件制作";
            button.CommandParameter = "ShowComponentsPalette";
            button.CommandHandler = new ComponentsPaletteCommandHandler();

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

        // 新增三轴加工功能区和按钮
        private static void CreateCNCPanel(RibbonTab tab)
        {
            RibbonPanelSource panelSource = new RibbonPanelSource();
            panelSource.Title = "三轴加工";
            RibbonPanel panel = new RibbonPanel();
            panel.Source = panelSource;
            tab.Panels.Add(panel);

            RibbonButton button = new RibbonButton();
            button.Name = "绘制图纸";
            button.ShowText = true;
            button.Text = "绘制图纸";
            button.CommandHandler = new DrawingsPaletteCommandHandler();
            panelSource.Items.Add(button);
        }

        // 新增家具属性功能区和按钮
        private static void CreatePropertyPanel(RibbonTab tab)
        {
            RibbonPanelSource panelSource = new RibbonPanelSource();
            panelSource.Title = "家具属性";
            RibbonPanel panel = new RibbonPanel();
            panel.Source = panelSource;
            tab.Panels.Add(panel);

            RibbonButton button = new RibbonButton();
            button.Name = "属性编辑";
            button.ShowText = true;
            button.Text = "属性编辑";
            button.CommandHandler = new PropertyPaletteCommandHandler();
            panelSource.Items.Add(button);
        }
    }
}
