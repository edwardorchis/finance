using DotNetOpenAuth.OAuth2;
using Finance.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Finance
{
    /// <summary>
    /// 鉴权处理
    /// </summary>
    public class OAuth2Handler : DelegatingHandler
    {
        static string ResourceServerPrivateKey = "<RSAKeyValue><Modulus>4+59zObnTWbqDk4ULzjVbBg2gMZMC4+i8cUj0nEXD622WZyyzDv1kAy4U0KOs5+iCocXGDq0nKG2ovfHk/AGo36Pm4Ij+aq0qQyMr1wvArSiStBHtGl3WNaid1QkT/87tEIg4dNRAPhUlxF6APwTFnNGp/xOLhAo1rTVnjCS4PE=</Modulus><Exponent>AQAB</Exponent><P>77C1lSG09A5GKAHULA/wewJHoAkhVH2zY7ujEeX/7P29UdQWZUDxjJqSDZbqP+rQQLrzFF01GhcxVdql7gfBTw==</P><Q>83DzpsqgQ3LggKq5i7hdzYUegD8uWNicMA9c5GL+rAXpUce/sh87KO9vbCOwOw4N4sbWXlrbaUCLcVGr+uwpvw==</Q><DP>QacWZa3g4cSTJNwzYIpRJXBfbA90KK9xlozLwthL/H8X/zTnmX5ra0bfYIeIzE8mEcTjVh2dsPLPWaPVNVi8cw==</DP><DQ>dmqgKqbv1D9iE1R4kw1om5tAXfPd0Jv1Ra+DaRj6dqUdfIlkpvloJp5pnbmydNd+S6ybBCTAC++4pLOsq48LMw==</DQ><InverseQ>3vc7yC45UFDkQ4XtgaXJYniCYUJIIyloGGWIWDE0EwJeb7Ux82mgs+M8Yeb+9Tz5rwlhk4Sj6rpROSq03xb+Ew==</InverseQ><D>H9caBbyfxSVCPvtTQIF89tuvCXAqAVdwWLvEVEpuAUev+Ha2V2ds11GfkinzC06acUQLytuwjUzd2YgpfhYCpyKySiza6wMYx/cxwSFZI40SeK3JTIYnS0bmGIuo7ocL/4MMbOY1L0GhUSJgn24bfpBOHRQ1Py2DrE6R3tEGeK0=</D></RSAKeyValue>";
        static string ResourceServerPublicKey = "<RSAKeyValue><Modulus>4+59zObnTWbqDk4ULzjVbBg2gMZMC4+i8cUj0nEXD622WZyyzDv1kAy4U0KOs5+iCocXGDq0nKG2ovfHk/AGo36Pm4Ij+aq0qQyMr1wvArSiStBHtGl3WNaid1QkT/87tEIg4dNRAPhUlxF6APwTFnNGp/xOLhAo1rTVnjCS4PE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        static List<string> NoAuthMethod = new List<string> {
            "/user/login", "/accountctl/list", "/accountctl/manage"
        };
        static List<string> UploadFileMethod = new List<string> {
            "/template/upload"
        };

        private static void DecodeQueryPara(HttpRequestMessage request)
        {
            var query = request.GetQueryNameValuePairs();
            string token = "";
            foreach (var kv in query)
            {
                if (kv.Key == "token")
                {
                    token = kv.Value;
                    break;
                }
            }
            string[] user = OAuth2Handler.DecryptToken(token).Split('|');
            var userId = user[0];
            var userName = user[1];
            var tid = user[2];
            request.Properties.Add("UserId", userId);
            request.Properties.Add("UserName", userName);
            request.Properties.Add("Tid", tid);
        }

        ILogger logger = Logger.GetLogger(typeof(OAuth2Handler));
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string method = request.RequestUri.LocalPath;
            if (UploadFileMethod.Contains(method))
            {
                DecodeQueryPara(request);
                return await base.SendAsync(request, cancellationToken).ContinueWith(
                (task) =>
                {
                    return task.Result;
                });
            }
            var content = request.Content.ReadAsStringAsync().Result;
            var query = request.GetQueryNameValuePairs();
            try
            {
                ValidSign(query, content);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "AUTHENTICATION_ERROR");
                return request.CreateResponse(HttpStatusCode.OK, FinanceResult.AUTHENTICATION_ERROR);
            }
            try
            {
                var json = JsonConvert.DeserializeObject<JObject>(content);
                //string method = json["Method"].ToString();
                if (!NoAuthMethod.Contains(method))
                {
                    string token = json["Token"].ToString();
                    string[] user = DecryptToken(token).Split('|');
                    var userId = user[0];
                    var userName = user[1];
                    var tid = user[2];
                    request.Properties.Add("UserId", userId);
                    request.Properties.Add("UserName", userName);
                    request.Properties.Add("Tid", tid);
                }
                else if(method == "/user/login")
                {
                    request.Properties.Add("Tid", json["Tid"].ToString());
                }
                return await base.SendAsync(request, cancellationToken).ContinueWith(
                (task) =>
                {
                    return task.Result;
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "AUTHENTICATION_ERROR");
                return request.CreateResponse(HttpStatusCode.OK, FinanceResult.AUTHENTICATION_ERROR);
            }
        }

        public static string DecryptToken(string token)
        {
            byte[] b64_data = Convert.FromBase64String(token.Replace("k@", "").Replace('_', '+').Replace('-', '/'));
            byte[] data = GetResourceServerRsa().Decrypt(b64_data, true);
            return Encoding.UTF8.GetString(data);
        }
        public static Random rd = new Random(1);
        public static string CreateToken(long uid,string username, long tid, DateTime liftDate)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}|{1}|{2}|{3}",
                uid, username, tid, liftDate.ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo), rd.Next(1, 99999).ToString()));
            var priKey = GetResourceServerRsa();
            byte[] enData = priKey.Encrypt(data, true);
            var val = Convert.ToBase64String(enData, Base64FormattingOptions.None).Replace('+', '_').Replace('/', '-');
            return val;
        }
        public static RSACryptoServiceProvider GetAuthorizationServerRsa()
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            rSACryptoServiceProvider.FromXmlString(ResourceServerPublicKey);
            return rSACryptoServiceProvider;
        }
        public static RSACryptoServiceProvider GetResourceServerRsa()
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            rSACryptoServiceProvider.FromXmlString(ResourceServerPrivateKey);
            return rSACryptoServiceProvider;
        }
     
        public static ClaimsPrincipal CreatePrincipal(string userName)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, userName));          
            var claimsIdentity = new ClaimsIdentity(claims, "OAuth 2 Bearer");
            var principal = new ClaimsPrincipal(claimsIdentity);
            return principal;
        }
        public static bool AuthUser(HttpRequestMessage request)
        {
            using (RSACryptoServiceProvider authorizationServerRsa = GetAuthorizationServerRsa())
            {
                using (RSACryptoServiceProvider resourceServerRsa = GetResourceServerRsa())
                {
                    ResourceServer resourceServer = new ResourceServer(new StandardAccessTokenAnalyzer(authorizationServerRsa, resourceServerRsa));
                    var token = resourceServer.GetAccessToken(request, new string[0]);
                    if (token != null)
                    {
                        IPrincipal principal = CreatePrincipal(token.User);
                        HttpContext.Current.User = principal;
                        Thread.CurrentPrincipal = principal;
                        request.Headers.Add("UserName", token.User);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }


        void ValidSign(IEnumerable<KeyValuePair<string,string>> queryDicttionary, string content)
        {
            var app = GetApp();
            var kv = queryDicttionary.GetEnumerator();
            var sign = "";
            while (kv.MoveNext())
            {
                var cur = kv.Current;
                if (cur.Key == "sign")
                {
                    sign = cur.Value;
                    break;
                }
            }

            StringBuilder query = new StringBuilder(app.Secret);
            query.Append(content);
            query.Append(app.Secret);

            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));
         
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString("X2"));
            }

            var sign2 =  result.ToString();
            if (sign != sign2)
            {
                throw new FinanceException(FinanceResult.AUTHENTICATION_ERROR);
            }

        }

        AppEntity GetApp()
        {
            return new AppEntity
            {
                AppKey = "FinanceClient",
                Ver = "1.0",
                Secret= "lhKL@ti&KukX5FmB0o5P0TABaXhF8I!T"
            };

        }

        class AppEntity
        {
            public string AppKey { set; get; }
            public string Ver { set; get; }
            public string Secret { set; get; }
        }
    }
}
