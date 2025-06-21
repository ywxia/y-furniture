using System;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;

namespace yz.furniture.Commands
{
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
