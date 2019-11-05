using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static Finance.Account.Controls.Commons.Consts;

namespace Finance.Account.Controls.Commons
{
    public static class Consts
    {
        internal static Brush BLACK_BRUSH = new SolidColorBrush(Colors.Black);
        internal static Brush WHITE_BRUSH = new SolidColorBrush(Colors.White);
        internal static Brush HIGHLIGHT_BRUSH = new SolidColorBrush(Colors.AliceBlue);

        //定义事件参数类
        public class RowChangeEventArgs : EventArgs
        {
            public string OldKey { set; get; }
            public string NewKey { set; get; }
            /// <summary>
            /// true  焦点返回上一
            /// </summary>
            public bool Cancel { set; get; }
        }
        //定义delegate
        public delegate List<AccountSubjectObj> GetAccountSubjectListEventHandler();
        public delegate List<AuxiliaryObj> GetAuxiliaryObjListEventHandler();
        public delegate void DataChangedEventHandler(object sender, DataChangedArgs e);
        public delegate void RowChangeEventHandler(object sender, RowChangeEventArgs e);
        public delegate void CellKeyDownEventHandler(object sender, KeyEventArgs e);
        public delegate void MessageEventHandler(MessageLevel level, string msg);
        public delegate void BeginLayoutEventHandler();
        public delegate void EndLayoutEventHandler();

        public static void LogDebug(string msg, params object[] args)
        {
            if (args.Length > 0)
            {
                msg = string.Format(msg, args);
                msg += "\r\n" + GetStackTraceModelName();
            }
            FinanceControlEventsManager.Instance.OnMessageEventHandlerEvent(MessageLevel.INFO, msg);
        }

        public static void LogError(string msg, params object[] args)
        {
            if (args.Length > 0)
            {
                msg = string.Format(msg, args);
                msg += "\r\n" + GetStackTraceModelName();
            }
            FinanceControlEventsManager.Instance.OnMessageEventHandlerEvent(MessageLevel.ERR, msg);
        }


        public static BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, bitmap.RawFormat);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        /// <summary>
        /// @Author:      HTL
        /// @Email:       Huangyuan413026@163.com
        /// @DateTime:    2015-06-03 19:54:49
        /// @Description: 获取当前堆栈的上级调用方法列表,直到最终调用者,只会返回调用的各方法,而不会返回具体的出错行数，可参考：微软真是个十足的混蛋啊！让我们跟踪Exception到行把！（不明真相群众请入） 
        /// </summary>
        /// <returns></returns>
        public static string GetStackTraceModelName()
        {
            //当前堆栈信息
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1,true);
            System.Diagnostics.StackFrame[] sfs = st.GetFrames();
            //过虑的方法名称,以下方法将不会出现在返回的方法调用列表中
            string _filterdName = "ResponseWrite,ResponseWriteError,";
            string _fullName = string.Empty, _methodName = string.Empty;
            for (int i = 1; i < sfs.Length; ++i)
            {
                //非用户代码,系统方法及后面的都是系统调用，不获取用户代码调用结束
                if (System.Diagnostics.StackFrame.OFFSET_UNKNOWN == sfs[i].GetILOffset()) break;
                _methodName = sfs[i].GetMethod().Name;//方法名称
                                                      //sfs[i].GetFileLineNumber();//没有PDB文件的情况下将始终返回0
                if (_filterdName.Contains(_methodName)) continue;
                _fullName = _methodName + "()->" + _fullName;
            }
            st = null;
            sfs = null;
            _filterdName = _methodName = null;
            _filterdName = st.GetFrame(0).GetFileLineNumber() + _filterdName;
            return _fullName.TrimEnd('-', '>');
        }
    }

    public class UserDefineInputItem
    {
        public string Label { set; get; }

        public Type DataType { set; get; }

        public object DataValue { set; get; }

        public int TabIndex { set; get; }

        public string Name { set; get; }

        public string TagLabel { set; get; }

        public string Unit { set; get; }

        public int Width { set; get; }
    }

    public class DataChangedArgs
    {
        public string Name { set; get; }
        public Type DataType { set; get; }
        public object OldValue { set; get; }
        public object NewValue { set; get; }
        public bool Cancel { set; get; }
    }

    public enum MessageLevel
    {
        INVALID,
        INFO,
        WARN,
        ERR
    }
    public enum FinanceAccountControlErrorCode
    {
        SUCCESS = 0,
        /// <summary>
        /// 取消
        /// </summary>
        CANCEL,
        /// <summary>
        /// 不存在
        /// </summary>
        NOT_EXIST,
        /// <summary>
        /// 不支持
        /// </summary>
        NOT_SUPPORT,
        /// <summary>
        /// 未知的系统错误
        /// </summary>
        SYSTEM_ERROR,
    }

    public class FinanceAccountControlException : Exception
    {
        string msg = "";
        public FinanceAccountControlException(FinanceAccountControlErrorCode ErrCode, string Message = "")
        {
            HResult = (int)ErrCode;
            msg = Message;
        }

        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(msg))
                    return ((FinanceAccountControlErrorCode)HResult).ToString();
                else
                    return msg;
            }
        }
    }

    public enum AccountFlag
    {
        FLAG_NULL = 0,
  
        /// <summary>
        /// 前八位做UserDefine控制, 最后一位做辅助核算，倒数第二位做数量核算
        /// </summary>
        FLAG_USDEF_MASK = 65283, //1111 1111 0000 0011


        FLAG_MAX = 65535, //1111 1111 1111 1111
    }

    public class FinanceControlEventsManager
    {
        public static FinanceControlEventsManager Instance { get; } = new FinanceControlEventsManager();

        public event GetAccountSubjectListEventHandler GetAccountSubjectListEvent;
        public event GetAuxiliaryObjListEventHandler GetAuxiliaryObjListEvent;
        public event MessageEventHandler MessageEventHandlerEvent;

        public List<AccountSubjectObj> OnGetAccountSubjectListEvent()
        {
            return GetAccountSubjectListEvent?.Invoke();
        }

        public List<AuxiliaryObj> OnGetAuxiliaryObjListEvent()
        {
            return GetAuxiliaryObjListEvent?.Invoke();
        }

        public void OnMessageEventHandlerEvent(MessageLevel level, string msg)
        {
            MessageEventHandlerEvent?.Invoke(level, msg);
        }
    }

}