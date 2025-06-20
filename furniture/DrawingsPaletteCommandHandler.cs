using System;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Windows;

namespace furniture
{
    public class DrawingsPaletteCommandHandler : ICommand
    {
        public bool CanExecute(object parameter) => true;
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            DrawingsPaletteManager.ShowPalette();
        }
    }

    public static class DrawingsPaletteManager
    {
        private static PaletteSet _paletteSet;
        public static void ShowPalette()
        {
            if (_paletteSet == null)
            {
                _paletteSet = new PaletteSet("三轴加工 - 绘制图纸");
                _paletteSet.Size = new System.Drawing.Size(320, 480);
                _paletteSet.DockEnabled = (DockSides.Left | DockSides.Right);
                _paletteSet.Add("图纸列表", new DrawingsPaletteControl());
            }
            _paletteSet.Visible = true;
        }
    }
}
