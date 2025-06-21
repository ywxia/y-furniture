using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Windows.Input;
using yz.furniture.UI.Palettes;

namespace yz.furniture.Commands
{
    public class ComponentsPaletteCommandHandler : ICommand
    {
        public bool CanExecute(object parameter) => true;
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            ComponentsPaletteManager.ShowPalette();
        }
    }

    public static class ComponentsPaletteManager
    {
        private static PaletteSet _paletteSet;

        public static void ShowPalette()
        {
            if (_paletteSet == null)
            {
                _paletteSet = new PaletteSet("部件制作", "ShowComponentsPalette", new System.Guid("B8E84B6F-988A-459C-A249-2E75B35C8D4B"));
                _paletteSet.Add("Components", new ComponentsPaletteControl());
                _paletteSet.Style = PaletteSetStyles.ShowPropertiesMenu |
                                    PaletteSetStyles.ShowAutoHideButton |
                                    PaletteSetStyles.ShowCloseButton;
                _paletteSet.MinimumSize = new System.Drawing.Size(200, 150);
            }

            _paletteSet.Visible = true;
        }
    }
}
