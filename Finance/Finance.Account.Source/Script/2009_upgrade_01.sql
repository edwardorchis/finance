-- 科目增加辅助核算和数量核算

delete from _MenuTableMap where _group = 'base_setting' and _name = 'accountitem';
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_index])VALUES('base_setting','accountitem','辅助核算','FormAccountItem',8);

go

begin transaction tran_Auxiliary
declare @id int
select @id = _number from _SerialNo where _key = 0
select @id = isnull(@id, 1)
delete from _Auxiliary where _type = 0;
insert into _Auxiliary (_id,_type,_no,_groupId, _name, _description)values (@id,	0,'1', 0,'科目组','');
insert into _Auxiliary (_id,_type,_no,_groupId, _name, _description)values (@id +1,	0,'2', 0,'凭证字','');
insert into _Auxiliary (_id,_type,_no,_groupId, _name, _description)values (@id +2,	0,'3', 0,'凭证摘要','');
insert into _Auxiliary (_id,_type,_no,_groupId, _name, _description)values (@id +3,	0,'4', 0,'凭证掩码','');
insert into _Auxiliary (_id,_type,_no,_groupId, _name, _description)values (@id +4,	0,'5', 1,'结转方式','');
insert into _Auxiliary (_id,_type,_no,_groupId, _name, _description)values (@id +5,	0,'6', 2,'供应商','');
insert into _Auxiliary (_id,_type,_no,_groupId, _name, _description)values (@id +6,	0,'7', 2,'客户','');
insert into _Auxiliary (_id,_type,_no,_groupId, _name, _description)values (@id +7,	0,'8', 2,'产品','');
insert into _Auxiliary (_id,_type,_no,_groupId, _name, _description)values (@id +8,	0,'9', 2,'其他附加项','');
update _SerialNo set _number = @id + 9 where _key = 0
if @@ERROR!=0
    rollback transaction
else
	commit transaction tran_Auxiliary

go

if not exists(select 1 from syscolumns where id=(select max(id) from sysobjects where xtype='u' and name='_AccountSubject') and name = '_actItemGrp')
	alter table _AccountSubject add _actItemGrp nvarchar(50);

if not exists(select 1 from syscolumns where id=(select max(id) from sysobjects where xtype='u' and name='_AccountSubject') and name = '_actUint')
	alter table _AccountSubject add _actUint nvarchar(50);

go

if not exists(select 1 from syscolumns where id=(select max(id) from sysobjects where xtype='u' and name='_VoucherEntryUdef') and name = '_actItemGrp')
	alter table _VoucherEntryUdef add _actItemGrp int not null default(0);

if not exists(select 1 from syscolumns where id=(select max(id) from sysobjects where xtype='u' and name='_VoucherEntryUdef') and name = '_act_price')
	alter table _VoucherEntryUdef add _act_price decimal(20,2) not null default(0);

if not exists(select 1 from syscolumns where id=(select max(id) from sysobjects where xtype='u' and name='_VoucherEntryUdef') and name = '_act_qty')
	alter table _VoucherEntryUdef add _act_qty  decimal(20,2) not null default(0);

go