using Finance.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Finance.Account.Source
{
    public class Generator
    {
        /// <summary>
        /// 作为主键的关键字
        /// </summary>
        static string[] primaryKeyFlags = {"id","index"};

        public static string GenerateSql(Type info)
        {
            StringBuilder sb = new StringBuilder();
            var members = info.GetProperties();
            foreach (var mi in members)
            {
                Type miType = mi.PropertyType;
                if (miType.IsGenericType)
                {
                    Type[] genericArgTypes = miType.GetGenericArguments();
                    miType = genericArgTypes[0];
                }

                var members2 = miType.GetProperties();
                sb.AppendLine(String.Format("IF EXISTS(Select 1 From Sysobjects Where Name='{0}' And Xtype='u') DROP TABLE  {0}", "_" + miType.Name));
                sb.AppendLine(String.Format("create table _{0} (", miType.Name));
                string primaryKey = string.Empty;
                foreach (var mi2 in members2)
                {
                    sb.AppendLine(String.Format("\t_{0}\t\t\t{1}\t,", mi2.Name, typeString(mi2)));
                    if (primaryKeyFlags.Contains(mi2.Name))
                    {
                        primaryKey += "_" + mi2.Name + ",";
                    }
                }
                if (primaryKey != string.Empty)
                {
                    primaryKey = "\tPRIMARY KEY ( " + primaryKey.Substring(0, primaryKey.Length - 1);
                    primaryKey += ")";
                    sb.AppendLine(primaryKey);
                }
                sb.AppendLine(")");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static void GenerateSql(Type info, int orderNo = 0)
        {            
            FileHelper.Write(GenerateSql(info), getSourcePath() + "Script\\" + orderNo.ToString("000") + "_" + info.Name+".sql");
        }

        public static string getSourcePath()
        {
            string relativePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            return Path.GetFullPath(relativePath);
        }

        static string typeString(PropertyInfo mi)
        {
            string str = string.Empty;
            if (mi.Name == "timeStamp")
                str = "timestamp not null";
            else if (mi.PropertyType == typeof(string) || mi.PropertyType == typeof(char))
                str = "nvarchar(255)";
            else if (mi.PropertyType == typeof(long))
                str = "bigint not null default(0)";
            else if (mi.PropertyType == typeof(short) || mi.PropertyType == typeof(int)
                || mi.PropertyType == typeof(ushort) || mi.PropertyType == typeof(uint) || mi.PropertyType == typeof(ulong))
                str = "int  not null default 0";
            else if (mi.PropertyType == typeof(decimal) || mi.PropertyType == typeof(float) || mi.PropertyType == typeof(double))
                str = "decimal(23,10)  not null default 0";
            else if (mi.PropertyType == typeof(bool))
                str = "int  not null default 0";
            else if (mi.PropertyType == typeof(sbyte) || mi.PropertyType == typeof(byte))
                str = "image";
            else if (mi.PropertyType == typeof(DateTime))
                str = "DateTime";
            else
                str = "nvarchar(255)";
            return str;
        }

    }
}
