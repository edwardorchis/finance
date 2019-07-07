

namespace Finance.Account.SDK
{
    public interface IFinanceRequest<T> where T : FinanceResponse
    {  
        /// <summary>
        /// 方法名
        /// </summary>
        string Method { get; }
    }
}