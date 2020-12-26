using Finance.Account.SDK.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Utils
{
    public class AutoRetryClient
    {
        static string rootPath = @"http://localhost:9000/api/";

        protected virtual string RootPath { get { return rootPath; } }

        protected virtual string Token{  get { return "token"; } }

        protected virtual string Appkey { get { return "appkey"; } }

        protected virtual string Secret { get { return "secret"; } }

        protected virtual string Ver { get { return "ver"; } }

        /// <summary>
        /// 单次请求的最大重试次数，默认值为3次。
        /// </summary>
        private int maxRetryCount = 3;
        /// <summary>
        /// 重试之前休眠时间，默认值为500毫秒。
        /// </summary>
        private int retryWaitTime = 100;

        ILogger logger = new DefaultLogger();

        public T Execute<T>(IFinanceRequest<T> request) where T : FinanceResponse
        {
            T rsp = default(T);
            Stopwatch watcher = new Stopwatch();
            int retryCounter = -1;
            while (retryCounter < maxRetryCount)
            {
                watcher.Restart();
                try
                {
                    retryCounter++;
                    var strRsp = "";
                    var plus = "";
                    if (request.GetType().IsSubclassOf(typeof(IdRequest)))
                    {
                        plus = ("/" + (request as IdRequest).id);                       
                    }
                    strRsp = Post(request.Method + plus, JsonConvert.SerializeObject(request));
                   
                    rsp = JsonConvert.DeserializeObject<T>(strRsp);
                    if (rsp.IsError)
                    {
                        watcher.Stop();
                        logger.Info("耗时" + watcher.ElapsedMilliseconds + "ms");
                        if (rsp.ErrMsg == "SERVICE_TIMEOUT")
                        {
                            var message = string.Format("调用超时:{0}，正在重试{1}次，超时信息：{2},耗时" + watcher.ElapsedMilliseconds + "ms", request.Method, retryCounter + 1, rsp.Body);
                            logger.Warn(message);
                            Thread.Sleep(retryWaitTime);
                            continue;
                        }
                        else
                        {
                            throw new FinanceApiException(rsp.Result,rsp.ErrMsg);
                        }
                    }
                    else
                    {
                        watcher.Stop();
                        logger.Info("耗时" + watcher.ElapsedMilliseconds + "ms");
                        break;
                    }
                }
                catch (WebException e)
                {
                    watcher.Stop();
                    //调用HTTP出错，重试
                    if (retryCounter < maxRetryCount)
                    {
                        var message = string.Format("网络异常:{0}，正在重试{1}次，异常信息：{2},耗时" + watcher.ElapsedMilliseconds + "ms,请求：{3}",
                            request.Method, retryCounter + 1, e.Message, JsonConvert.SerializeObject(request));
                        logger.Warn(message);
                        Thread.Sleep(retryWaitTime);
                        continue;
                    }
                    else
                    {
                        logger.Warn("耗时" + watcher.ElapsedMilliseconds + "ms");
                        throw e;
                    }
                }
            }
            return rsp;
        }
               

        public void SetMaxRetryCount(int maxRetryCount)
        {
            this.maxRetryCount = maxRetryCount;
        }

        public void SetRetryWaitTime(int retryWaitTime)
        {
            this.retryWaitTime = retryWaitTime;
        }

        public void SetLoger(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <param name="content">Post提交数据内容(utf-8编码的)</param>
        /// <returns></returns>
        string Post(string url, string content = "")
        {
            string result = "";

            JObject json = null;
            if (content == "")
                json = new JObject();
            else           
                json = JsonConvert.DeserializeObject<JObject>(content);
            json.Add("Token", Token);
            content = json.ToString();
            url += string.Format("?ver={0}&appkey={1}&sign={2}" ,Ver, Appkey,SignRequest(content));

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(RootPath + url);
            req.Method = "POST";
            req.ContentType = "application/json";

            #region 添加Post 参数
            
            logger.Info(string.Format("ulr:{0}\r\nbody:{1}",RootPath+url,content));
            byte[] data = Encoding.UTF8.GetBytes(content);        
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion


            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }



        string SignRequest(string body)
        {
            StringBuilder query = new StringBuilder(Secret);
            query.Append(body);       
            query.Append(Secret);
          
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString("X2"));
            }

            return result.ToString();
        }

        public string DownloadFile<T>(string fileName,IFinanceRequest<T> request) where T : FinanceResponse
        {
            string result = "";

            var content = JsonConvert.SerializeObject(request);
            JObject json = null;
            if (content == "")
                json = new JObject();
            else
                json = JsonConvert.DeserializeObject<JObject>(content);
            json.Add("Token", Token);
            content = json.ToString();
            var url = RootPath + request.Method + string.Format("?ver={0}&appkey={1}&sign={2}", Ver, Appkey, SignRequest(content));

            result = HttpDownloadFile(url, content, fileName);

            return result;
        }


        /// <summary>
        /// Http下载文件
        /// </summary>
        string HttpDownloadFile(string url, string content,string path)
        {          
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 1000 * 60 * 2;//2s

            byte[] data = Encoding.UTF8.GetBytes(content);
            request.ContentLength = data.Length;
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            //创建本地文件写入流
            Stream stream = new FileStream(path, FileMode.Create);
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
            return path;
        }

        public string UploadFile<T>(string fileName, IFinanceUploadFileRequest<T> request) where T : FinanceResponse
        {
            string result = "";
            var url = RootPath + request.Method + string.Format("?ver={0}&appkey={1}&name={2}&token={3}", Ver, Appkey, request.Name, Token);
            result = HttpUploadFile(url, fileName);
            return result;
        }

        string HttpUploadFile(string url, string path)
        {
            // 根据uri创建HttpWebRequest对象
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
            httpReq.Method = "POST";

            //对发送的数据不使用缓存
            httpReq.AllowWriteStreamBuffering = false;

            //设置获得响应的超时时间（半小时）
            httpReq.Timeout = 300000;
            //httpReq.Timeout = 5000;
            // httpReq.ReadWriteTimeout = 150000;
            httpReq.KeepAlive = true;
            httpReq.ProtocolVersion = HttpVersion.Version11;

            httpReq.ContentType = "application/json";
            httpReq.SendChunked = true;
            //在将 AllowWriteStreamBuffering 设置为 false 的情况下执行写操作时，必须将 ContentLength 设置为非负数，或者将 SendChunked 设置为 true。

            // 要上传的文件
            FileStream oFileStream = new FileStream(path.ToString(), FileMode.Open, FileAccess.Read);
            BinaryReader oBinaryReader = new BinaryReader(oFileStream);
            //每次上传4k
            int bufferLength = 1024 * 1024;
            byte[] buffer = new byte[bufferLength];

            //已上传的字节数
            long offset = 0;

            //开始上传时间
            int size = oBinaryReader.Read(buffer, 0, bufferLength);
            Stream postStream = httpReq.GetRequestStream();

            while (size > 0)
            {
                postStream.Write(buffer, 0, size);
                offset += size;
                size = oBinaryReader.Read(buffer, 0, bufferLength);
            }
            postStream.Flush();
            postStream.Close();

            //获取服务器端的响应
            WebResponse webRespon = httpReq.GetResponse();
            Stream s = webRespon.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            //读取服务器端返回的消息
            string sReturnString = sr.ReadLine();
            s.Close();
            sr.Close();
            return sReturnString;
        }
    }
}
