using Autodesk.AutoCAD.Windows;
using System;
using System.Windows.Input;
using yz.furniture.UI.Palettes;

namespace yz.furniture.Commands
{
    public class PropertyPaletteCommandHandler : ICommand
    {
        public bool CanExecute(object parameter) => true;
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            PropertyPaletteManager.ShowPalette();
        }
    }

    public static class PropertyPaletteManager
    {
        private static PaletteSet _paletteSet;

        public static void ShowPalette()
        {
            if (_paletteSet == null)
            {
                // 为新的面板创建一个唯一的GUID
                _paletteSet = new PaletteSet("家具属性", "ShowPropertyPalette", new System.Guid("A1B2C3D4-E5F6-7890-1234-567890ABCDEF"));
                _paletteSet.Add("Properties", new PropertyPaletteControl());
                _paletteSet.Style = PaletteSetStyles.ShowPropertiesMenu |
                                    PaletteSetStyles.ShowAutoHideButton |
                                    PaletteSetStyles.ShowCloseButton;
                _paletteSet.MinimumSize = new System.Drawing.Size(200, 150);
            }

            _paletteSet.Visible = true;
        }
    }
}
