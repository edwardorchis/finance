
delete from _SystemProfile where _category='Hidden' and _key = 'BConnectString';
insert into _SystemProfile (_category,_key,_value,_description) values ('Hidden','BConnectString','Data Source=localhost;Initial Catalog=test;User ID=sa;Password=123456','业务系统数据库连接');

go

delete from _MenuTableMap where _group = 'account' and _name = '_InterfaceExec';
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('account','_InterfaceExec','引入凭证','FormInterface',8192, 8);

go

delete from [_UdefTemplate] where _tableName = '_InterfaceExec';
INSERT INTO [_UdefTemplate]([_tableName],[_name],[_label],[_dataType],[_tabIndex],[_defaultVal],[_reserved],[_tagLabel],[_width])
VALUES('_InterfaceExec','yearBegin','会计年度', 'int' ,1, '$(currentYear)','sp_collect_for_voucher','',0);
INSERT INTO [_UdefTemplate]([_tableName],[_name],[_label],[_dataType],[_tabIndex],[_defaultVal],[_reserved],[_tagLabel],[_width])
VALUES('_InterfaceExec','periodBegin','期间', 'int' ,2, '$(currentPeriod)','sp_collect_for_voucher','',0);
INSERT INTO [_UdefTemplate]([_tableName],[_name],[_label],[_dataType],[_tabIndex],[_defaultVal],[_reserved],[_tagLabel],[_width])
VALUES('_InterfaceExec','yearEnd','到 会计年度', 'int' ,3, '$(currentYear)','sp_collect_for_voucher','',0);
INSERT INTO [_UdefTemplate]([_tableName],[_name],[_label],[_dataType],[_tabIndex],[_defaultVal],[_reserved],[_tagLabel],[_width])
VALUES('_InterfaceExec','periodEnd','期间', 'int' ,4, '$(currentPeriod)','sp_collect_for_voucher','',0);

go

-- 关联单号
if not exists(select * from syscolumns where id=object_id('_VoucherEntry') and name='_linkNo') 
	alter table _VoucherEntry add _linkNo nvarchar(50) 

go

-- 自动生成科目时通过科目代码获取明细科目名称
if (exists (select * from sys.objects where name = 'sp_getaccountsubjectname'))
    drop proc sp_getaccountsubjectname

go

create proc sp_getaccountsubjectname
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

go

-- 收集生成凭证信息
if (exists (select * from sys.objects where name = 'sp_collect_for_voucher'))
    drop proc sp_collect_for_voucher

go

create proc sp_collect_for_voucher
(
	@yearBegin int,
	@periodBegin int,
	@yearEnd int,
	@periodEnd int
)
as
-- header 
select  '关联单号' as _linkNo,'记' as _word, '备注（页面未体现，忽略）' as _note, '参考信息' as _reference, 2019 as [_year], 4 as _period, getdate() as _businessDate,
getdate() as [_date], getdate() as _creatTime, 13594 as _creater, '出纳' as _cashier, '经办人' as _agent;

-- entry
select '关联单号' as _linkNo,1 as [_index],/*主营业务收入*/'6001.1001' as _accountSubjectNo,'销售产品A' as _explanation, 1000 as _amount, -1 as _direction, '5C46D0E37EA749CBB0C28C7C65DFD857' as _uniqueKey
union all 
select '关联单号' as _linkNo,2 as [_index],/*应收账款*/'1122.1001' as _accountSubjectNo,'应收客户甲' as _explanation, 1000 as _amount, 1 as _direction, 'B50E86494313480AA66849A0D091E46E' as _uniqueKey

-- udefenties

select  '关联单号' as _linkNo, '5C46D0E37EA749CBB0C28C7C65DFD857' as _uniqueKey, '0' as _price_gold, '0' as _qty_gold, '1' as _price_stone, '5' as _qty_stone ,'' as _remark

go


IF EXISTS(Select 1 From Sysobjects Where Name='_TaskResult' And Xtype='u') DROP TABLE  _TaskResult
create table _TaskResult (
	_taskId			nvarchar(50)	,
	_taskType		nvarchar(50)	,
	_createTime		datetime	,
	_lastRefreshTime	datetime	,
	_progRate	int not null default 0,
	_status		int  not null default 0	,
	_reserved     nvarchar(255),
	_result		nvarchar(1000),
	PRIMARY KEY ( _taskId)
)

go

