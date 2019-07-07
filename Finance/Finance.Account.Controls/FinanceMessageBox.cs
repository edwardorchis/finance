using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Finance.Account.Controls
{
    public class FinanceMessageBox
    {
        public static void Error(string msg)
        {
            var popup = new FinanceMessageBoxPopup();
            popup.Message = msg;
            popup.ShowDialog();
        }
        public static void Info(string msg)
        {
            var popup = new FinanceMessageBoxPopup();
            popup.Message = msg;
            popup.ShowDialog();
        }

        public static MessageBoxResult Quest(string msg)
        {
            MessageBoxResult mbr = MessageBoxResult.None;
            var popup = new FinanceMessageBoxPopup("是","否","取消");
            popup.ButtonClickEvent += (num) => {
                switch (num)
                {
                    case 1:
                        mbr = MessageBoxResult.Yes;
                        break;
                    case 2:
                        mbr = MessageBoxResult.No;
                        break;
                    case 3:
                    case 0:
                        mbr = MessageBoxResult.Cancel;
                        break;
                }                
            };
            popup.Message = msg;
            popup.ShowDialog();
            return mbr;
        }

        public static void Progress(string msg,Action<ProgressEventArgs> callbackfunc)
        {
            var popup = new FinanceMessageBoxPopup();
            popup.Wait = true;
            popup.Message = msg;
            popup.ProgressEvent += (args) => {
                callbackfunc?.Invoke(args);
            };         
            popup.ShowDialog();
        }




        public class ProgressEventArgs
        {
            public string Message { set; get; }
            public bool Close { set; get; }
        }
    }

}
