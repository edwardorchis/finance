using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Utils
{
    public enum FinanceResult
    {
        SUCCESS = 0,
        /// <summary>
        /// 没有导入的实现
        /// </summary>
        [DescripVals("没有导入的实现")]
        NULL_DTL = 1000,
        /// <summary>
        /// 文件不存在
        /// </summary>
        [DescripVals("文件不存在")]
        FILE_NOT_EXISST,
        /// <summary>
        /// 记录不存在
        /// </summary>
        [DescripVals("记录不存在")]
        RECORD_NOT_EXIST,
        /// <summary>
        /// 记录已存在
        /// </summary>
        [DescripVals("记录已存在")]
        RECORD_EXIST,
        /// <summary>
        /// 不完美的数据
        /// </summary>
        [DescripVals("不完美的数据")]
        IMPERFECT_DATA,
        /// <summary>
        /// 超时
        /// </summary>
        [DescripVals("超时")]
        SERVICE_TIMEOUT,
        /// <summary>
        /// 空的请求
        /// </summary>
        [DescripVals("空的请求")]
        NULL,
        /// <summary>
        /// 当前状态不符合预期
        /// </summary>
        [DescripVals("当前状态不符合预期")]
        INCORRECT_STATE,
        /// <summary>
        /// 借贷不平衡
        /// </summary>
        [DescripVals("借贷不平衡")]
        AMMOUNT_IMBALANCE,
        /// <summary>
        /// 有关联的业务
        /// </summary>
        [DescripVals("有关联的业务")]
        LINKED_DATA,
        /// <summary>
        /// 不支持
        /// </summary>
        [DescripVals("不支持")]
        NOT_SUPPORT =   3000,
        /// <summary>
        /// 未知的系统错误
        /// </summary>
        [DescripVals("未知的系统错误")]
        SYSTEM_ERROR =   3001,
        /// <summary>
        /// 无效的签名
        /// </summary>
        [DescripVals("无效的签名")]
        AUTHENTICATION_ERROR
    }

    

    public class FinanceException : Exception
    {
        string msg = "";
        public FinanceException(FinanceResult ErrCode, string Message = "")
        {
            HResult = (int)ErrCode;
            msg = Message;
        }
       
        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(msg))
                {
                    return DescripValsHelper.ToDescription(typeof(FinanceResult), ((FinanceResult)HResult).ToString());
                }                  
                else
                    return msg;
            }
        }

    }

    public class DescripValsHelper
    {
        Dictionary<string, string> allDescriptions = DescripValsHelper.GetEnumDescriptions(typeof(FinanceResult));
        /// <summary>
        /// 获取枚举下的所有注释
        /// </summary>
        /// <param name="Enumtype">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEnumDescriptions(Type Enumtype)
        {
            Type type = typeof(DescripValsAttribute);
            Dictionary<string, string> returnList = new Dictionary<string, string>();
            foreach (FieldInfo fi in Enumtype.GetFields())
            {
                object[] arr = fi.GetCustomAttributes(type, true);
                if (arr.Length > 0)
                {
                    returnList.Add(((DescripValsAttribute)arr[0]).description, fi.Name);
                }
            }

            return returnList;
        }

        /// <summary>
        /// 获取枚举下某一注释
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="value">某一枚举字符串形式</param>
        /// <returns></returns>
        public static string ToDescription(Type type, string value)
        {

            FieldInfo info = type.GetField(value);
            DescripValsAttribute attr = info.GetCustomAttributes(typeof(DescripValsAttribute), true)[0] as DescripValsAttribute;

            if (attr != null)
            {
                return attr.description;
            }
            else
            {
                return null;
            }
        }
    }
    /// <summary>
    /// 自定义属性--用于在枚举中注释
    /// </summary>

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class DescripValsAttribute : Attribute
    {
        public string description;
        public DescripValsAttribute(string text)
        {
            description = text;
        }
    }
}
