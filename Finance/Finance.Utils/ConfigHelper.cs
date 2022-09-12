using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Finance.Utils
{
    public class ConfigHelper
    {
        private static readonly ConfigHelper inst = new ConfigHelper();

        static ConfigHelper() { }
        private ConfigHelper() { }
        public static ConfigHelper Instance {  get { return inst; } }

        public string Path { set { file = value; } }
    
        private string file = AppDomain.CurrentDomain.BaseDirectory + "/Finance.exe.config";
        public string XmlReadConnectionString(string name)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(file);
            XmlNode root = xDoc.SelectSingleNode("configuration");
            XmlNode node = root.SelectSingleNode("connectionStrings/add[@name='"+ name  + "']");
            XmlElement el = node as XmlElement;
            return el.GetAttribute("connectionString");
        }

        public string XmlReadAppSetting(string key)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(file);
            XmlNode root = xDoc.SelectSingleNode("configuration");
            XmlNode node = root.SelectSingleNode("appSettings/add[@key='" + key + "']");
            XmlElement el = node as XmlElement;
            return el.GetAttribute("value");
        }


        /// <summary>
        /// 修改AppSettings中配置
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">相应值</param>
        public static bool SetConfigValue(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] != null)
                    config.AppSettings.Settings[key].Value = value;
                else
                    config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 获取AppSettings中某一节点值
        /// </summary>
        /// <param name="key"></param>
        public static string GetConfigValue(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] != null)
                return config.AppSettings.Settings[key].Value;
            else
                return string.Empty;
        }
      

        public static string XmlReadAppSetting(string file, string key)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(file);
            XmlNode root = xDoc.SelectSingleNode("configuration");
            XmlNode node = root.SelectSingleNode("appSettings/add[@key='" + key + "']");
            XmlElement el = node as XmlElement;
            return el.GetAttribute("value");
        }
    }
}
