
if exists(select 1 from sysobjects where xtype='u' and name = '_UdefTemplate')
	drop table _UdefTemplate
create table _UdefTemplate  
(
	_tableName		varchar(50) not null,
	_name			varchar(50) not null,
	_label			varchar(100),
	_dataType		varchar(20) not null,
	_tabIndex		int not null default(0),
	_defaultVal		varchar(255),
	_reserved		nvarchar(500),
	_tagLabel		varchar(50),
	_width			int not null default(0)
	PRIMARY KEY ( _tableName, _name)
)

if not exists(select 1 from _UdefTemplate where _tableName='_VoucherEntryUdef' and _name= 'price_gold')
insert into _UdefTemplate (_tableName,_name,_label,_dataType,_tabIndex,_defaultVal,_reserved,_tagLabel,_width)
values ('_VoucherEntryUdef','price_gold','黄金单价','decimal',1,'0','','price|gold',0);	
if not exists(select 1 from _UdefTemplate where _tableName='_VoucherEntryUdef' and _name= 'qty_gold')
insert into _UdefTemplate (_tableName,_name,_label,_dataType,_tabIndex,_defaultVal,_reserved,_tagLabel,_width)
values ('_VoucherEntryUdef','qty_gold','黄金数量','decimal',2,'0','','qty|gold',0);	
if not exists(select 1 from _UdefTemplate where _tableName='_VoucherEntryUdef' and _name= 'price_stone')
insert into _UdefTemplate (_tableName,_name,_label,_dataType,_tabIndex,_defaultVal,_reserved,_tagLabel,_width)
values ('_VoucherEntryUdef','price_stone','石头单价','decimal',3,'0','','price|stone',0);	
if not exists(select 1 from _UdefTemplate where _tableName='_VoucherEntryUdef' and _name= 'qty_stone')
insert into _UdefTemplate (_tableName,_name,_label,_dataType,_tabIndex,_defaultVal,_reserved,_tagLabel,_width)
values ('_VoucherEntryUdef','qty_stone','石头数量','decimal',4,'0','','qty|stone',0);	
if not exists(select 1 from _UdefTemplate where _tableName='_VoucherEntryUdef' and _name= 'remark')
insert into _UdefTemplate (_tableName,_name,_label,_dataType,_tabIndex,_defaultVal,_reserved,_tagLabel,_width)
values ('_VoucherEntryUdef','remark','备注','string',5,'','','|remark',200);	


if exists(select 1 from sysobjects where xtype='u' and name = '_VoucherEntryUdef')
	drop table _VoucherEntryUdef  
create table _VoucherEntryUdef  
	(
		_id			bigint not null default(0)	,
		_uniqueKey		varchar(50) not null,
		_price_gold			decimal(23,10)  not null default 0	,		
		_qty_gold			decimal(23,10)  not null default 0	,
		_price_stone		decimal(23,10)  not null default 0	,		
		_qty_stone			decimal(23,10)  not null default 0	,
		_remark				nvarchar(100)
		PRIMARY KEY ( _id,_uniqueKey)
	)
	
if not exists(select 1 from syscolumns where id=(select max(id) from sysobjects where xtype='u' and name='_AccountSubject') and name = '_flag')
	alter table _AccountSubject add _flag int not null default 0;


delete from _Auxiliary where _type = 4;
insert into _Auxiliary (_id,_type,_no,_name, _description)values (22,4,'gold','32768','黄金');
insert into _Auxiliary (_id,_type,_no,_name, _description)values (23,4,'stone','16384','石头');
insert into _Auxiliary (_id,_type,_no,_name, _description)values (24,4,'remark','8192','备注');

-- 防止与预置的重复
update _SerialNo set _number = 1000 where _key = 0;
