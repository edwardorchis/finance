# API文档

#### 0. 公共部分

 ###### 请求url

*  {apiRoot}/{method}?ver={version}&appkey={appkey}&sign={sign}

| 参数名 | 说明 | 示例 |
| ------ | ------ | ------ |
| apiRoot | 根地址 | http://localhost:9000 |
| method | 方法名 | /voucher/save  |
| version | 版本号 | 1.0 |
| appkey | AppKey | FinanceClient  |
| sign | 签名 | 5D3AA349E953AF0C15E38C147B15EE35  |

 ###### 参数

| 参数名 | 必选 | 类型 | 说明 |
| ------ | ------ | ------ | ------|
| Content | 是 | Content | 内容 |
| Token | 是 | string  | 鉴权令牌，登录后获取 |

 ###### 返回

| 参数名 | 必选 | 类型 | 说明 |
| ------ | ------ | ------ | ------|
| Result | 是 | int | 结果码 |
| ErrMsg | 否 | string | 错误描述 |
| Solution | 否 | string | 解决建议 |

 ###### 结果码

| 结果码 | 描述 | 说明 |
| ------ | ------ | ------ |
| 0 | SUCCESS | 成功 |
| 1000 | NULL_DTL | 没有导入的实现 |
| 1001 | FILE_NOT_EXISST | 文件不存在 |
| 1002 | RECORD_NOT_EXIST | 记录不存在 |
| 1003 | RECORD_EXIST | 记录已存在 |
| 1004 | IMPERFECT_DATA | 不完美的数据 |
| 1005 | SERVICE_TIMEOUT | 超时 |
| 1006 | NULL | 空的请求 |
| 1007 | INCORRECT_STATE | 当前状态不符合预期 |
| 1008 | AMMOUNT_IMBALANCE | 借贷不平衡 |
| 1009 | LINKED_DATA | 有关联的业务 |
| 3000 | NOT_SUPPORT | 不支持 |
| 3001 | SYSTEM_ERROR | 未知的系统错误 |
| 3002 | AUTHENTICATION_ERROR | 无效的签名 |

 ###### 签名算法

```c#
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
```


#### 1. 保存凭证

 ###### 方法

*  /voucher/save

 ###### 请求方式

*  POST

 ###### 参数

*  Content 

| 参数名 | 必选 | 类型 | 说明 |
| ------ | ------ | ------ | ------|
| header | 是 | VoucherHeader | 表头 |
| entries | 是 | List<VoucherEntry> | 分录 |

*  VoucherHeader 

| 参数名 | 必选 | 类型 | 说明 |
| ------ | ------ | ------ | ------|
| id | 是 | int | ID（为0即为新增，否则为修改的内码） |
| word | 是 | string | 凭证字：“记” |
| note | 否 | string | 备注 |
| reference | 否 | string | 参考信息 |
| year | 是 | int | 年度 |
| period | 是 | int | 期间 |
| businessDate | 是 | dateTime | 业务日期（2019-03-05T00:00:00） |
| date | 是 | dateTime | 日期（2019-03-05T00:00:00） |
| creatTime | 否 | dateTime | 创建时间（2019-03-05T00:00:00） |
| creater | 是 | int | 创建人 |
| cashier | 否 | string | 出纳 |
| agent | 否 | string | 经办人 |

*  VoucherEntry

| 参数名 | 必选 | 类型 | 说明 |
| ------ | ------ | ------ | ------|
| index | 是 | int | 分录号 |
| accountSubjectNo | 是 | string | 科目代号 |
| explanation | 否 | string | 摘要 |
| amount |  是 | decimal | 金额 |
| direction |  是 | int | 方向：1（借方），-1（贷方 ） |

 ###### 返回

| 参数名 | 必选 | 类型 | 说明 |
| ------ | ------ | ------ | ------|
| Id | 条件 | int | 如果成功，返回凭证内码 |

 ###### 示例

- url : http://localhost:9000/voucher/save?ver=1.0&appkey=FinanceClient&sign=5D3AA349E953AF0C15E38C147B15EE35
- body:

```Json
{
	"Content" : {
		"header" : {
			"id" : 0,
			"word" : "记",
			"no" : 1,
			"serialNo" : 3,
			"note" : null,
			"reference" : "参考信息",
			"year" : 2019,
			"period" : 3,
			"businessDate" : "2019-03-05T00:00:00",
			"date" : "2019-03-05T00:00:00",
			"creatTime" : "2019-03-05T23:46:01.710907+08:00",
			"creater" : 13594,
			"cashier" : "出纳",
			"agent" : "经办",
			"poster" : 0,
			"checker" : 0,
			"checkTime" : null,
			"postTime" : null,
			"status" : 0
		},
		"entries" : [{				
				"index" : 1,
				"accountSubjectId" : 18,
				"explanation" : "摘要",
				"amount" : 200.0,
				"direction" : 1
			}, {
				"index" : 2,
				"accountSubjectId" : 19,
				"explanation" : "摘要",
				"amount" : 200.0,
				"direction" : -1
			}
		]
	},
	"Token" : "3AIavxSAoaJhuBB1o4hc6fw4hNlpC4KoXgfT1mpu1hyQiA_lqKNKOkNDtC_mKdHiWEAHooTE1vZAtsYhz4g_jNoNR5JBkp9UHXZSYFbSuj0j38muKaMbFhTRMEZ_xqd4znSY_Fcp8V96hto4VpLWqh0__yHKFoRZQ0DGS5HcrpQ="
}
```

- 返回

  ```Json
  {
  	"id" : 233,
  	"Result" : 0,
  	"ErrMsg" : null,
  	"Solution" : null
  }
  ```

  