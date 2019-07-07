

delete from _MenuTableMap where _group = 'base_setting' and _name = 'access_right';
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_index])VALUES('base_setting','access_right','权限设置','FormAccesRight',6);


go

if exists(select 1 from sysobjects where xtype='u' and name = '_AccessRight')
	drop table _AccessRight  
create table _AccessRight
(
	_id int not null,
	_group nvarchar(50),
	_name nvarchar(50),
	_mask int not null default(0)

	PRIMARY KEY ( _id,_group,_name)
)

go


delete from _UdefTemplate where _tableName='_InterfaceExec' and _reserved='sp_generatevoucherbybillno';
INSERT INTO [_UdefTemplate]([_tableName],[_name],[_label],[_dataType],[_tabIndex],[_defaultVal],[_reserved],[_tagLabel],[_width])
     VALUES('_InterfaceExec','billNo','单号','string',1,'','sp_generatevoucherbybillno','按单号生成',400);

go

if (exists (select * from sys.objects where name = 'sp_generatevoucherbybillno'))
    drop proc sp_generatevoucherbybillno

go

create proc sp_generatevoucherbybillno
(
	@billNo nvarchar(50)
)
as
-- header 
select  @billNo as _linkNo,'记' as _word, '备注（页面未体现，忽略）' as _note, '参考信息' as _reference, YEAR(GETDATE()) as [_year], MONTH(getdate()) as _period, GETDATE() as _businessDate,
getdate() as [_date], getdate() as _creatTime, 13594 as _creater, '出纳' as _cashier, '经办人' as _agent;

-- entry
select @billNo as _linkNo,1 as [_index],/*主营业务收入*/'6001.1001' as _accountSubjectNo,'销售产品A' as _explanation, 1000 as _amount, -1 as _direction, '5C46D0E37EA749CBB0C28C7C65DFD857' as _uniqueKey
union all 
select @billNo as _linkNo,2 as [_index],/*应收账款*/'1122.1001' as _accountSubjectNo,'应收客户甲' as _explanation, 1000 as _amount, 1 as _direction, 'B50E86494313480AA66849A0D091E46E' as _uniqueKey

-- udefenties

select  @billNo as _linkNo, '5C46D0E37EA749CBB0C28C7C65DFD857' as _uniqueKey, '0' as _price_gold, '0' as _qty_gold, '1' as _price_stone, '5' as _qty_stone ,'' as _remark


go

if not exists(select 1 from _UdefTemplate where _tableName='_VoucherEntryUdef' and _name= 'amount_gold')
insert into _UdefTemplate (_tableName,_name,_label,_dataType,_tabIndex,_defaultVal,_reserved,_tagLabel,_width)
values ('_VoucherEntryUdef','amount_gold','黄金金额','decimal',3,'0','','amount|gold',0);	
if not exists(select 1 from _UdefTemplate where _tableName='_VoucherEntryUdef' and _name= 'amount_stone')
insert into _UdefTemplate (_tableName,_name,_label,_dataType,_tabIndex,_defaultVal,_reserved,_tagLabel,_width)
values ('_VoucherEntryUdef','amount_stone','石头金额','decimal',6,'0','','amount|stone',0);	

go

if not exists(select 1 from syscolumns where id=(select max(id) from sysobjects where xtype='u' and name='_VoucherEntryUdef') and name = '_amount_gold')
	alter table _VoucherEntryUdef add _amount_gold int not null default 0;

if not exists(select 1 from syscolumns where id=(select max(id) from sysobjects where xtype='u' and name='_VoucherEntryUdef') and name = '_amount_stone')
	alter table _VoucherEntryUdef add _amount_stone int not null default 0;

go


