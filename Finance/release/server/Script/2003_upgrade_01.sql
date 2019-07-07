if (exists (select * from sys.objects where name = 'sp_udefreport_voucher_udef'))
    drop proc sp_udefreport_voucher_udef
go
create proc sp_udefreport_voucher_udef
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
go


delete from [_UdefTemplate] where _tableName = 'sp_udefreport_voucher_udef';
INSERT INTO [_UdefTemplate]([_tableName],[_name],[_label],[_dataType],[_tabIndex],[_defaultVal],[_reserved],[_tagLabel],[_width])
VALUES('sp_udefreport_voucher_udef','yearBegin','会计年度', 'int' ,1, '$(currentYear)','','',0);
INSERT INTO [_UdefTemplate]([_tableName],[_name],[_label],[_dataType],[_tabIndex],[_defaultVal],[_reserved],[_tagLabel],[_width])
VALUES('sp_udefreport_voucher_udef','periodBegin','期间', 'int' ,2, '$(currentPeriod)','','',0);
INSERT INTO [_UdefTemplate]([_tableName],[_name],[_label],[_dataType],[_tabIndex],[_defaultVal],[_reserved],[_tagLabel],[_width])
VALUES('sp_udefreport_voucher_udef','yearEnd','到 会计年度', 'int' ,3, '$(currentYear)','','',0);
INSERT INTO [_UdefTemplate]([_tableName],[_name],[_label],[_dataType],[_tabIndex],[_defaultVal],[_reserved],[_tagLabel],[_width])
VALUES('sp_udefreport_voucher_udef','periodEnd','期间', 'int' ,4, '$(currentPeriod)','','',0);

go

IF EXISTS(Select 1 From Sysobjects Where Name='_MenuTableMap' And Xtype='u') DROP TABLE  _MenuTableMap
create table _MenuTableMap (
	_group			nvarchar(50)	,
	_name			nvarchar(50)	,
	_header			nvarchar(255)	,
	_financeForm	nvarchar(255)	,
	_mask		int  not null default 0	,
	_index		int  not null default 0,
	PRIMARY KEY ( _group,_name)
)

go

delete from [_MenuTableMap];

INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('base_setting','user_list','用户列表','FormUser',2, 1);
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('base_setting','account_subject','科目','FormAccountSubject',4, 2);
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('base_setting','auxiliary_manager','辅助资料','FormAuxiliary',8, 3);
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('base_setting','menu_edit','菜单','FormMenuEdit',2048, 4);
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('base_setting','udeftemplate_list','自定义模板','FormUdefTemplate',4096, 5);

INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('account','begin_balance','初始余额表','FormBeginBalance',16, 1);
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('account','voucher_input','凭证录入','FormVoucher',32, 2);
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('account','voucher_list','凭证列表','FormVoucherList',64, 3);
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('account','account_balance','科目余额表','FormAccountBalance',128, 4);
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('account','balance_sheet','资产负债表','FormBalanceSheet',256, 5);
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('account','settle','结账','',512, 6);
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_mask],[_index])VALUES('account','sp_udefreport_voucher_udef','数量统计表','FormUdefReport',1024, 7);

go


