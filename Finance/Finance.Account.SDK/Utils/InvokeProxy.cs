using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net.Http;
using System.Threading;
using Finance.Account.SDK.Request;
using Finance.Utils;

namespace Finance.Account.SDK.Utils
{
    public class InvokeProxy
    {
        static string rootPath = @"http://localhost:9000/api/";

        protected virtual string RootPath { get { return rootPath; } }

        protected virtual string Token { get { return "token"; } }

        protected virtual string Appkey { get { return "appkey"; } }

        protected virtual string Secret { get { return "secret"; } }

        protected virtual string Ver { get { return "ver"; } }

        protected virtual IDictionary<string, object> Cookie { get; }

        ILogger logger = new DefaultLogger();

        public T Execute<T>(IFinanceRequest<T> request) where T : FinanceResponse
        {
            try {
                string invokeName = request.Method.TrimStart('/').TrimEnd('/');
                string[] ms = invokeName.Split('/');
                if (ms.Length != 2) {
                    return null;
                }
                string path = string.Format("Finance.Controller.{0}controller,Finance", ms[0]);
                logger.Info(path);

                //加载类型
                Type type = Type.GetType(path, true, true);
                //根据类型创建实例
                object obj = Activator.CreateInstance(type, true);
                object[] initParams = { Cookie };
                type.GetMethod("InvokeInit").Invoke(obj, initParams);

                //加载方法参数类型及方法
                MethodInfo method = type.GetMethod(ms[1], BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                logger.Info(method.Name);
                Type requestType = request.GetType();
                if (requestType.GetProperties().Length == 1) {
                    return (T)method.Invoke(obj, null);
                }

                List<object> lstPara = new List<object>();
                if (requestType.IsSubclassOf(typeof(IdRequest))) {
                    lstPara.Add(requestType.GetProperty("id").GetValue(request, null));
                } else {
                    lstPara.Add(request);
                }
                //类型转换并返回
                return (T)method.Invoke(obj, lstPara.ToArray());
            }
            catch (Exception ex) {
                throw new FinanceApiException(ex.HResult, ex.Message);
            }
        }

        public void SetLoger(ILogger logger)
        {
            this.logger = logger;
        }
        public string DownloadFile<T>(string fileName, IFinanceRequest<T> request) where T : FinanceResponse
        {
            string result = "";


            return result;
        }
        public string UploadFile<T>(string fileName, IFinanceUploadFileRequest<T> request) where T : FinanceResponse
        {
            string result = "";
            return result;
        }
    }
}
