using Finance.Utils;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Finance
{
    /// <summary>
    /// 消息处理程序
    /// </summary>
    public class MessageHandler : DelegatingHandler
    {
        ILogger logger = Logger.GetLogger(typeof(MessageHandler));
        /// <summary>
        /// 重写发送HTTP请求到内部处理程序的方法
        /// </summary>
        /// <param name="request">请求信息</param>
        /// <param name="cancellationToken">取消操作的标记</param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 记录请求内容
            if (request.Content != null)
            {
                logger.Info(string.Format("请求Content:{0}\t{1}",request.RequestUri, request.Content.ReadAsStringAsync().Result));
            }


            // 发送HTTP请求到内部处理程序，在异步处理完成后记录响应内容
            return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>(
            (task) =>
            {                
                if (task.Result.Content == null)
                {
                    logger.Info(string.Format("响应\tStatusCode:{0}\tContent:null", task.Result.StatusCode));
                }
                else
                {
                    // 记录响应内容
                    logger.Info(string.Format("响应\tStatusCode:{0}\tContent:{1}", task.Result.StatusCode, task.Result.Content.ReadAsStringAsync().Result));
                }
                return task.Result;
                
            }
            );
        }
    }

}
