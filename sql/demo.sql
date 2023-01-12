USE [master]
GO
/****** Object:  Database [demo]    Script Date: 2023-01-13 01:22:35 ******/
CREATE DATABASE [demo]
GO
ALTER DATABASE [demo] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [demo].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [demo] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [demo] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [demo] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [demo] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [demo] SET ARITHABORT OFF 
GO
ALTER DATABASE [demo] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [demo] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [demo] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [demo] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [demo] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [demo] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [demo] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [demo] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [demo] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [demo] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [demo] SET  ENABLE_BROKER 
GO
ALTER DATABASE [demo] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [demo] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [demo] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [demo] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [demo] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [demo] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [demo] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [demo] SET RECOVERY FULL 
GO
ALTER DATABASE [demo] SET  MULTI_USER 
GO
ALTER DATABASE [demo] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [demo] SET DB_CHAINING OFF 
GO
ALTER DATABASE [demo] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [demo] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [demo]
GO
/****** Object:  StoredProcedure [dbo].[sp_collect_for_voucher]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[sp_collect_for_voucher]
(
	@yearBegin int,
	@periodBegin int,
	@yearEnd int,
	@periodEnd int
)
as
-- header 
select  '关联单号' as _linkNo,'记' as _word, '备注（页面未体现，忽略）' as _note, '参考信息' as _reference, 2019 as [_year], 11 as _period, getdate() as _businessDate,
getdate() as [_date], getdate() as _creatTime, 13594 as _creater, '出纳' as _cashier, '经办人' as _agent;
-- entry
select '关联单号' as _linkNo,1 as [_index],/*主营业务收入*/'6001.1001' as _accountSubjectNo,'销售产品A' as _explanation, 1000 as _amount, -1 as _direction, 
'5C46D0E37EA749CBB0C28C7C65DFD857' as _uniqueKey, '0' as _price_gold, '0' as _qty_gold, '1' as _price_stone, '5' as _qty_stone ,'' as _remark,
 '客户' as _item_type, '004' as _item_no, '客户丁' as _item_name, 10 as _act_price, 100 as _act_qty
union all 
select '关联单号' as _linkNo,2 as [_index],/*应收账款*/'1122.1001' as _accountSubjectNo,'应收客户甲' as _explanation, 1000 as _amount, 1 as _direction,
 'B50E86494313480AA66849A0D091E46E' as _uniqueKey, '0' as _price_gold, '0' as _qty_gold, '1' as _price_stone, '5' as _qty_stone ,'' as _remark,
  '客户' as _item_type, '004' as _item_no, '客户丁' as _item_name, 10 as _act_price, 100 as _act_qty

GO
/****** Object:  StoredProcedure [dbo].[sp_generatevoucherbybillno]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[sp_generatevoucherbybillno]
(
	@billNo nvarchar(50)
)
as
-- header 
select  @billNo as _linkNo,'记' as _word, '备注（页面未体现，忽略）' as _note, '参考信息' as _reference, YEAR(GETDATE()) as [_year], MONTH(getdate()) as _period, GETDATE() as _businessDate,
getdate() as [_date], getdate() as _creatTime, 13594 as _creater, '出纳' as _cashier, '经办人' as _agent;
-- entry
select @billNo as _linkNo,1 as [_index],/*主营业务收入*/'6001.1001' as _accountSubjectNo,'销售产品A' as _explanation, 1000 as _amount, -1 as _direction, 
'5C46D0E37EA749CBB0C28C7C65DFD857' as _uniqueKey, '0' as _price_gold, '0' as _qty_gold, '1' as _price_stone, '5' as _qty_stone ,'' as _remark, 
'客户' as _item_type, '004' as _item_no, '客户丁' as _item_name, 10 as _act_price, 100 as _act_qty
union all 
select @billNo as _linkNo,2 as [_index],/*应收账款*/'1403' as _accountSubjectNo,'应收客户甲' as _explanation, 1000 as _amount, 1 as _direction,
 'B50E86494313480AA66849A0D091E46E' as _uniqueKey, '0' as _price_gold, '0' as _qty_gold, '1' as _price_stone, '5' as _qty_stone ,'' as _remark, 
 '客户' as _item_type, '004' as _item_no, '客户丁' as _item_name, 10 as _act_price, 100 as _act_qty
 
GO
/****** Object:  StoredProcedure [dbo].[sp_getaccountsubjectname]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[sp_getaccountsubjectname]
(
	@accountSubjectNo nvarchar(50)	
)
as
declare @sales int
select @sales = PATINDEX ( '6001.%' , @accountSubjectNo) 
if (@sales = 1)
	select '销售产品A'
declare @receivables int
select @receivables = PATINDEX ( '1122.%' , @accountSubjectNo) 
if (@receivables = 1)
	select '应收客户甲'

GO
/****** Object:  StoredProcedure [dbo].[sp_udefreport_voucher_udef]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[sp_udefreport_voucher_udef]
(
	@yearBegin int,
	@periodBegin int,
	@yearEnd int,
	@periodEnd int
)
as
	select 'ID' as _id, '凭证字号' as _voucher_no, '黄金单价' as _price_gold, '黄金数量' as _qty_gold, '石头单价' as _price_stone, '石头数量' as _qty_stone, '备注' as _remark;
    select h._word + '-' + convert(nvarchar(10), h._no) as _voucher_no,e._id,
	convert(decimal(18,2), e._price_gold) as _price_gold,convert(decimal(18,2),e._qty_gold) as _qty_gold,
	convert(decimal(18,2), e._price_stone) as _price_stone,convert(decimal(18,2),e._qty_stone) as _qty_stone,
	e._remark 
	from _VoucherEntryUdef as e
	inner join _VoucherHeader h on e._id = h._id
	where h._year >= @yearBegin and h._period >= @periodBegin and h._year <= @yearEnd and h._period <= @periodEnd 

GO
/****** Object:  StoredProcedure [dbo].[sp_voucher_print_v1]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[sp_voucher_print_v1]
(
	@id int
)
as
select v._word as Word, v._no as [No], CONVERT(varchar(100), v._date, 23) as [Date],v._agent as Agent, v._cashier as Cashier,
isnull(u1._userName, '') as Checker, isnull(u2._userName,'') as Creater, isnull(u3._userName,'') as Poster, 
dbo.func_convert_tostring(d.DebitsTotal) as DebitsTotal,dbo.func_convert_tostring(c.CreditTotal) as CreditTotal,
dbo.func_convert_toupper(d.DebitsTotal) as ChnTotal,
(select top 1 _value from _SystemProfile where _category='Base' and _key = 'CompanyName') as CompanyName
 from _VoucherHeader v 
 left join _UserInfo u1 on v._checker = u1._userId
 left join _UserInfo u2 on v._creater = u2._userId
 left join _UserInfo u3 on v._poster = u3._userId
 left join (select _id, sum(_amount) as DebitsTotal from _VoucherEntry where _id =@id and _direction = 1 group by _id) d on v._id = d._id
 left join (select _id, sum(_amount) as CreditTotal from _VoucherEntry where _id =@id and _direction = -1 group by _id) c on v._id = c._id
 where v._id = @id
select _explanation as Explanation, a._fullName as AccountSubjectFullName, 
dbo.func_convert_tostring(d.DebitsAmount) DebitsAmount, dbo.func_convert_tostring(c.CreditAmount) as CreditAmount
from _VoucherEntry v
left join _AccountSubject a on v._accountSubjectId = a._id
 left join (select _index, sum(_amount) as DebitsAmount from _VoucherEntry where _id =@id and _direction = 1 group by _index) d on v._index = d._index
 left join (select _index, sum(_amount) as CreditAmount from _VoucherEntry where _id =@id and _direction = -1 group by _index) c on v._index = c._index
where v._id = @id

GO
/****** Object:  UserDefinedFunction [dbo].[func_convert_tostring]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_convert_tostring](@dec decimal) 
RETURNS varchar(21) 
as
BEGIN
	declare @result varchar(21)
	select @result = isnull(CAST(cast(@dec as dec(18, 2)) as varchar(21)),'') 
  RETURN  @result
END

GO
/****** Object:  UserDefinedFunction [dbo].[func_convert_toupper]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_convert_toupper](@n_LowerMoney decimal(18,2))
RETURNS varchar(200)
AS
BEGIN
  Declare @v_LowerStr VARCHAR(200) -- 小写金额
  Declare @v_UpperPart VARCHAR(200)
  Declare @v_UpperStr VARCHAR(200) -- 大写金额
  Declare @i_I int
 
  --四舍五入为指定的精度并删除数据左右空格
  select @v_LowerStr = LTRIM(RTRIM(STR(@n_LowerMoney,20,2))) ,@i_I = 1,@v_UpperStr = ''
 
  while ( @i_I <= len(@v_LowerStr))
  begin
      select @v_UpperPart = case substring(@v_LowerStr,len(@v_LowerStr) - @i_I + 1,1)
                            WHEN  '.' THEN  '元'
                            WHEN  '0' THEN  '零'
                            WHEN  '1' THEN  '壹'
                            WHEN  '2' THEN  '贰'
                            WHEN  '3' THEN  '叁'
                            WHEN  '4' THEN  '肆'
                            WHEN  '5' THEN  '伍'
                            WHEN  '6' THEN  '陆'
                            WHEN  '7' THEN  '柒'
                            WHEN  '8' THEN  '捌'
                            WHEN  '9' THEN  '玖'
                            WHEN  '-' THEN  '负'
                            END
                          +
                            case @i_I
                            WHEN  1  THEN  '分'
                            WHEN  2  THEN  '角'
                            WHEN  3  THEN  ''
                            WHEN  4  THEN  ''
                            WHEN  5  THEN  '拾'
                            WHEN  6  THEN  '佰'
                            WHEN  7  THEN  '仟'
                            WHEN  8  THEN  '万'
                            WHEN  9  THEN  '拾'
                            WHEN  10  THEN  '佰'
                            WHEN  11  THEN  '仟'
                            WHEN  12  THEN  '亿'
                            WHEN  13  THEN  '拾'
                            WHEN  14  THEN  '佰'
                            WHEN  15  THEN  '仟'
                            WHEN  16  THEN  '万'
                            ELSE ''
                            END
     select @v_UpperStr = @v_UpperPart + @v_UpperStr
     select @i_I = @i_I + 1
  end
 
  select @v_UpperStr = REPLACE(@v_UpperStr,'零拾','零')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零佰','零')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零仟','零')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零零零','零')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零零','零')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零角零分','整')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零分','整')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零角','零')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零亿零万零元','亿元')
  select @v_UpperStr = REPLACE(@v_UpperStr,'亿零万零元','亿元')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零亿零万','亿')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零万零元','万元')
  select @v_UpperStr = REPLACE(@v_UpperStr,'万零元','万元')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零亿','亿')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零万','万')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零元','元')
  select @v_UpperStr = REPLACE(@v_UpperStr,'零零','零')
   
  --特殊处理负数  负仟肆佰叁拾元整
  select @v_UpperStr = REPLACE(@v_UpperStr,'负分',' ')
  select @v_UpperStr = REPLACE(@v_UpperStr,'负角',' ')
  select @v_UpperStr = REPLACE(@v_UpperStr,'负拾','负')
  select @v_UpperStr = REPLACE(@v_UpperStr,'负佰','负')
  select @v_UpperStr = REPLACE(@v_UpperStr,'负仟','负')
  select @v_UpperStr = REPLACE(@v_UpperStr,'负万','负')
 
  -- 对壹元以下的金额的处理
  if ( substring(@v_UpperStr,1,1)='元' )
  begin
       select @v_UpperStr = substring(@v_UpperStr,2,(len(@v_UpperStr) - 1))
  end
 
  if (substring(@v_UpperStr,1,1)= '零')
  begin
       select @v_UpperStr = substring(@v_UpperStr,2,(len(@v_UpperStr) - 1))
  end
 
  if (substring(@v_UpperStr,1,1)='角')
  begin
       select @v_UpperStr = substring(@v_UpperStr,2,(len(@v_UpperStr) - 1))
  end
 
  if ( substring(@v_UpperStr,1,1)='分')
  begin
       select @v_UpperStr = substring(@v_UpperStr,2,(len(@v_UpperStr) - 1))
  end
  if (substring(@v_UpperStr,1,1)='整')
  begin
       select @v_UpperStr = '零元整'
  end
  RETURN (@v_UpperStr)
end

GO
/****** Object:  Table [dbo].[_AccessRight]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_AccessRight](
	[_id] [int] NOT NULL,
	[_group] [nvarchar](50) NOT NULL,
	[_name] [nvarchar](50) NOT NULL,
	[_mask] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[_id] ASC,
	[_group] ASC,
	[_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_AccountBalance]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_AccountBalance](
	[_year] [bigint] NOT NULL,
	[_period] [bigint] NOT NULL,
	[_accountSubjectId] [bigint] NOT NULL,
	[_debitsAmount] [decimal](23, 10) NOT NULL,
	[_creditAmount] [decimal](23, 10) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_AccountSubject]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_AccountSubject](
	[_id] [bigint] NOT NULL,
	[_no] [nvarchar](255) NULL,
	[_name] [nvarchar](255) NULL,
	[_fullName] [nvarchar](255) NULL,
	[_parentId] [bigint] NOT NULL,
	[_rootId] [bigint] NOT NULL,
	[_groupId] [bigint] NOT NULL,
	[_level] [int] NOT NULL,
	[_isHasChild] [int] NOT NULL,
	[_direction] [int] NOT NULL,
	[_isDeleted] [int] NOT NULL,
	[_flag] [int] NOT NULL,
	[_actItemGrp] [nvarchar](255) NULL,
	[_actUint] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_Auxiliary]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_Auxiliary](
	[_id] [bigint] NOT NULL,
	[_type] [bigint] NOT NULL,
	[_no] [nvarchar](255) NULL,
	[_name] [nvarchar](255) NULL,
	[_description] [nvarchar](255) NULL,
	[_parentId] [bigint] NOT NULL,
	[_groupId] [int] NOT NULL,
	[_isUnused] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_BeginBalance]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_BeginBalance](
	[_accountSubjectId] [bigint] NOT NULL,
	[_debitsAmount] [decimal](23, 10) NOT NULL,
	[_creditAmount] [decimal](23, 10) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_CarriedForwardTemplate]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_CarriedForwardTemplate](
	[_id] [int] NOT NULL,
	[_index] [int] NOT NULL,
	[_src] [int] NOT NULL,
	[_dst] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[_id] ASC,
	[_index] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_ExcelTemplate]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_ExcelTemplate](
	[_name] [nvarchar](255) NULL,
	[_a] [nvarchar](255) NULL,
	[_b] [nvarchar](255) NULL,
	[_c] [nvarchar](255) NULL,
	[_d] [nvarchar](255) NULL,
	[_e] [nvarchar](255) NULL,
	[_f] [nvarchar](255) NULL,
	[_g] [nvarchar](255) NULL,
	[_h] [nvarchar](255) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_MenuTableMap]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_MenuTableMap](
	[_group] [nvarchar](50) NOT NULL,
	[_name] [nvarchar](50) NOT NULL,
	[_header] [nvarchar](255) NULL,
	[_financeForm] [nvarchar](255) NULL,
	[_mask] [int] NOT NULL,
	[_index] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[_group] ASC,
	[_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_OperationLog]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_OperationLog](
	[_time] [datetime] NULL,
	[_username] [nvarchar](255) NULL,
	[_category] [nvarchar](255) NULL,
	[_operation] [nvarchar](255) NULL,
	[_message] [nvarchar](255) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_SerialNo]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_SerialNo](
	[_key] [int] NOT NULL,
	[_ex] [nvarchar](255) NULL,
	[_number] [bigint] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_String]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_String](
	[_Chars] [nvarchar](255) NULL,
	[_Length] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_SystemProfile]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_SystemProfile](
	[_category] [nvarchar](255) NULL,
	[_key] [nvarchar](255) NULL,
	[_value] [nvarchar](255) NULL,
	[_description] [nvarchar](255) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_TaskResult]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_TaskResult](
	[_taskId] [nvarchar](50) NOT NULL,
	[_taskType] [nvarchar](50) NULL,
	[_createTime] [datetime] NULL,
	[_lastRefreshTime] [datetime] NULL,
	[_progRate] [int] NOT NULL,
	[_status] [int] NOT NULL,
	[_reserved] [nvarchar](255) NULL,
	[_result] [nvarchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[_taskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_TimeStampArticle]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_TimeStampArticle](
	[_id] [bigint] NOT NULL,
	[_value] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_UdefTemplate]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[_UdefTemplate](
	[_tableName] [varchar](50) NOT NULL,
	[_name] [varchar](50) NOT NULL,
	[_label] [varchar](100) NULL,
	[_dataType] [varchar](20) NOT NULL,
	[_tabIndex] [int] NOT NULL,
	[_defaultVal] [varchar](255) NULL,
	[_reserved] [nvarchar](500) NULL,
	[_tagLabel] [varchar](50) NULL,
	[_width] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[_tableName] ASC,
	[_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[_UserInfo]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_UserInfo](
	[_userId] [bigint] NOT NULL,
	[_userName] [nvarchar](255) NULL,
	[_passWord] [nvarchar](255) NULL,
	[_isDeleted] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_VoucherEntry]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_VoucherEntry](
	[_id] [bigint] NOT NULL,
	[_index] [bigint] NOT NULL,
	[_accountSubjectId] [bigint] NOT NULL,
	[_accountSubjectNo] [nvarchar](255) NULL,
	[_explanation] [nvarchar](255) NULL,
	[_amount] [decimal](23, 10) NOT NULL,
	[_direction] [int] NOT NULL,
	[_uniqueKey] [nvarchar](255) NULL,
	[_linkNo] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[_id] ASC,
	[_index] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_VoucherEntryUdef]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[_VoucherEntryUdef](
	[_id] [bigint] NOT NULL,
	[_uniqueKey] [varchar](50) NOT NULL,
	[_price_gold] [decimal](23, 10) NOT NULL,
	[_qty_gold] [decimal](23, 10) NOT NULL,
	[_price_stone] [decimal](23, 10) NOT NULL,
	[_qty_stone] [decimal](23, 10) NOT NULL,
	[_remark] [nvarchar](100) NULL,
	[_amount_gold] [int] NOT NULL,
	[_amount_stone] [int] NOT NULL,
	[_actItemGrp] [int] NOT NULL,
	[_act_price] [decimal](20, 2) NOT NULL,
	[_act_qty] [decimal](20, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[_id] ASC,
	[_uniqueKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[_VoucherHeader]    Script Date: 2023-01-13 01:22:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_VoucherHeader](
	[_id] [bigint] NOT NULL,
	[_word] [nvarchar](255) NULL,
	[_no] [bigint] NOT NULL,
	[_serialNo] [bigint] NOT NULL,
	[_note] [nvarchar](255) NULL,
	[_reference] [nvarchar](255) NULL,
	[_year] [int] NOT NULL,
	[_period] [int] NOT NULL,
	[_businessDate] [datetime] NULL,
	[_date] [datetime] NULL,
	[_creatTime] [datetime] NULL,
	[_creater] [bigint] NOT NULL,
	[_cashier] [nvarchar](255) NULL,
	[_agent] [nvarchar](255) NULL,
	[_poster] [bigint] NOT NULL,
	[_checker] [bigint] NOT NULL,
	[_checkTime] [nvarchar](255) NULL,
	[_postTime] [nvarchar](255) NULL,
	[_status] [int] NOT NULL,
	[_linkNo] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[_AccessRight] ADD  DEFAULT ((0)) FOR [_mask]
GO
ALTER TABLE [dbo].[_AccountBalance] ADD  DEFAULT ((0)) FOR [_year]
GO
ALTER TABLE [dbo].[_AccountBalance] ADD  DEFAULT ((0)) FOR [_period]
GO
ALTER TABLE [dbo].[_AccountBalance] ADD  DEFAULT ((0)) FOR [_accountSubjectId]
GO
ALTER TABLE [dbo].[_AccountBalance] ADD  DEFAULT ((0)) FOR [_debitsAmount]
GO
ALTER TABLE [dbo].[_AccountBalance] ADD  DEFAULT ((0)) FOR [_creditAmount]
GO
ALTER TABLE [dbo].[_AccountSubject] ADD  DEFAULT ((0)) FOR [_id]
GO
ALTER TABLE [dbo].[_AccountSubject] ADD  DEFAULT ((0)) FOR [_parentId]
GO
ALTER TABLE [dbo].[_AccountSubject] ADD  DEFAULT ((0)) FOR [_rootId]
GO
ALTER TABLE [dbo].[_AccountSubject] ADD  DEFAULT ((0)) FOR [_groupId]
GO
ALTER TABLE [dbo].[_AccountSubject] ADD  DEFAULT ((0)) FOR [_level]
GO
ALTER TABLE [dbo].[_AccountSubject] ADD  DEFAULT ((0)) FOR [_isHasChild]
GO
ALTER TABLE [dbo].[_AccountSubject] ADD  DEFAULT ((0)) FOR [_direction]
GO
ALTER TABLE [dbo].[_AccountSubject] ADD  DEFAULT ((0)) FOR [_isDeleted]
GO
ALTER TABLE [dbo].[_AccountSubject] ADD  DEFAULT ((0)) FOR [_flag]
GO
ALTER TABLE [dbo].[_Auxiliary] ADD  DEFAULT ((0)) FOR [_id]
GO
ALTER TABLE [dbo].[_Auxiliary] ADD  DEFAULT ((0)) FOR [_type]
GO
ALTER TABLE [dbo].[_Auxiliary] ADD  DEFAULT ((0)) FOR [_parentId]
GO
ALTER TABLE [dbo].[_Auxiliary] ADD  DEFAULT ((0)) FOR [_groupId]
GO
ALTER TABLE [dbo].[_Auxiliary] ADD  DEFAULT ((0)) FOR [_isUnused]
GO
ALTER TABLE [dbo].[_BeginBalance] ADD  DEFAULT ((0)) FOR [_accountSubjectId]
GO
ALTER TABLE [dbo].[_BeginBalance] ADD  DEFAULT ((0)) FOR [_debitsAmount]
GO
ALTER TABLE [dbo].[_BeginBalance] ADD  DEFAULT ((0)) FOR [_creditAmount]
GO
ALTER TABLE [dbo].[_MenuTableMap] ADD  DEFAULT ((0)) FOR [_mask]
GO
ALTER TABLE [dbo].[_MenuTableMap] ADD  DEFAULT ((0)) FOR [_index]
GO
ALTER TABLE [dbo].[_SerialNo] ADD  DEFAULT ((0)) FOR [_key]
GO
ALTER TABLE [dbo].[_SerialNo] ADD  DEFAULT ((0)) FOR [_number]
GO
ALTER TABLE [dbo].[_String] ADD  DEFAULT ((0)) FOR [_Length]
GO
ALTER TABLE [dbo].[_TaskResult] ADD  DEFAULT ((0)) FOR [_progRate]
GO
ALTER TABLE [dbo].[_TaskResult] ADD  DEFAULT ((0)) FOR [_status]
GO
ALTER TABLE [dbo].[_TimeStampArticle] ADD  DEFAULT ((0)) FOR [_id]
GO
ALTER TABLE [dbo].[_TimeStampArticle] ADD  DEFAULT ((0)) FOR [_value]
GO
ALTER TABLE [dbo].[_UdefTemplate] ADD  DEFAULT ((0)) FOR [_tabIndex]
GO
ALTER TABLE [dbo].[_UdefTemplate] ADD  DEFAULT ((0)) FOR [_width]
GO
ALTER TABLE [dbo].[_UserInfo] ADD  DEFAULT ((0)) FOR [_userId]
GO
ALTER TABLE [dbo].[_UserInfo] ADD  DEFAULT ((0)) FOR [_isDeleted]
GO
ALTER TABLE [dbo].[_VoucherEntry] ADD  DEFAULT ((0)) FOR [_id]
GO
ALTER TABLE [dbo].[_VoucherEntry] ADD  DEFAULT ((0)) FOR [_index]
GO
ALTER TABLE [dbo].[_VoucherEntry] ADD  DEFAULT ((0)) FOR [_accountSubjectId]
GO
ALTER TABLE [dbo].[_VoucherEntry] ADD  DEFAULT ((0)) FOR [_amount]
GO
ALTER TABLE [dbo].[_VoucherEntry] ADD  DEFAULT ((0)) FOR [_direction]
GO
ALTER TABLE [dbo].[_VoucherEntryUdef] ADD  DEFAULT ((0)) FOR [_id]
GO
ALTER TABLE [dbo].[_VoucherEntryUdef] ADD  DEFAULT ((0)) FOR [_price_gold]
GO
ALTER TABLE [dbo].[_VoucherEntryUdef] ADD  DEFAULT ((0)) FOR [_qty_gold]
GO
ALTER TABLE [dbo].[_VoucherEntryUdef] ADD  DEFAULT ((0)) FOR [_price_stone]
GO
ALTER TABLE [dbo].[_VoucherEntryUdef] ADD  DEFAULT ((0)) FOR [_qty_stone]
GO
ALTER TABLE [dbo].[_VoucherEntryUdef] ADD  DEFAULT ((0)) FOR [_amount_gold]
GO
ALTER TABLE [dbo].[_VoucherEntryUdef] ADD  DEFAULT ((0)) FOR [_amount_stone]
GO
ALTER TABLE [dbo].[_VoucherEntryUdef] ADD  DEFAULT ((0)) FOR [_actItemGrp]
GO
ALTER TABLE [dbo].[_VoucherEntryUdef] ADD  DEFAULT ((0)) FOR [_act_price]
GO
ALTER TABLE [dbo].[_VoucherEntryUdef] ADD  DEFAULT ((0)) FOR [_act_qty]
GO
ALTER TABLE [dbo].[_VoucherHeader] ADD  DEFAULT ((0)) FOR [_id]
GO
ALTER TABLE [dbo].[_VoucherHeader] ADD  DEFAULT ((0)) FOR [_no]
GO
ALTER TABLE [dbo].[_VoucherHeader] ADD  DEFAULT ((0)) FOR [_serialNo]
GO
ALTER TABLE [dbo].[_VoucherHeader] ADD  DEFAULT ((0)) FOR [_year]
GO
ALTER TABLE [dbo].[_VoucherHeader] ADD  DEFAULT ((0)) FOR [_period]
GO
ALTER TABLE [dbo].[_VoucherHeader] ADD  DEFAULT ((0)) FOR [_creater]
GO
ALTER TABLE [dbo].[_VoucherHeader] ADD  DEFAULT ((0)) FOR [_poster]
GO
ALTER TABLE [dbo].[_VoucherHeader] ADD  DEFAULT ((0)) FOR [_checker]
GO
ALTER TABLE [dbo].[_VoucherHeader] ADD  DEFAULT ((0)) FOR [_status]
GO
USE [master]
GO
ALTER DATABASE [demo] SET  READ_WRITE 
GO
