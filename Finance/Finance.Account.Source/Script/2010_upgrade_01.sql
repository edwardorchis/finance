
if (exists(select 1 from _SystemProfile where _category = 'Base' and _key = 'CompanyName' ))
	delete from _SystemProfile where _category='Base' and _key = 'CompanyName'
insert into _SystemProfile(_category,_key,_value,_description) values('Base', 'CompanyName', 'Finance', '公司名称')
go

if (exists (select 1 from sys.objects where name = 'func_convert_tostring'))
    drop function func_convert_tostring
go
CREATE FUNCTION func_convert_tostring(@dec decimal) 
RETURNS varchar(21) 
as
BEGIN
	declare @result varchar(21)
	select @result = isnull(CAST(cast(@dec as dec(18, 2)) as varchar(21)),'') 
  RETURN  @result
END

go
if (exists (select 1 from sys.objects where name = 'func_convert_toupper'))
    drop function func_convert_toupper
go
CREATE FUNCTION dbo.func_convert_toupper(@n_LowerMoney decimal(18,2))
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
go

if (exists (select 1 from sys.objects where name = 'sp_voucher_print_v1'))
    drop proc sp_voucher_print_v1
go
create proc dbo.sp_voucher_print_v1
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

go




