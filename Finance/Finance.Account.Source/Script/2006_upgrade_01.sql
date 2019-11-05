-- 结转模板表
if exists(select 1 from sysobjects where xtype='u' and name = '_CarriedForwardTemplate')
	drop table _CarriedForwardTemplate  
create table _CarriedForwardTemplate
(
	_id int not null,
	_index int not null,
	_src  int not null,
	_dst  int not null
	PRIMARY KEY ( _id,_index)
)

go

delete from _Auxiliary where _type = 5;
delete from _CarriedForwardTemplate;
-- 默认结转方式
begin transaction tran_Auxiliary

declare @id int
select @id = _number from _SerialNo where _key = 0
select @id = isnull(@id, 1)
insert into _Auxiliary (_id,_type,_no,_name,_description) values(@id, 5, 'income','结转收入', 'income')
insert into _Auxiliary (_id,_type,_no,_name,_description) values(@id+1, 5, 'cost','结转成本、费用和税金', 'cost')
insert into _Auxiliary (_id,_type,_no,_name,_description) values(@id+2, 5, 'investment','结转投资收益', 'investment')
insert into _Auxiliary (_id,_type,_no,_name,_description) values(@id+3, 5, 'profits','年度结转利润分配', 'profits')
update _SerialNo set _number = @id + 4 where _key = 0



INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id,1,154,131)
INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id,2,165,131)
INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id,3,160,131)

INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id+1,1,171,131)
INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id+1,2,172,131)
INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id+1,3,176,131)
INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id+1,4,219,131)
INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id+1,5,147,131)
INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id+1,6,230,131)
INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id+1,7,189,131)
INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id+1,8,198,131)
INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id+1,9,214,131)
INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id+1,10,90,131)

INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id+2,1,159,131)

INSERT INTO [_CarriedForwardTemplate] ([_id],[_index],[_src],[_dst]) VALUES(@id+3,1,134,131)
if @@ERROR!=0
    rollback transaction
else
	commit transaction tran_Auxiliary

go


delete from _MenuTableMap where _group = 'base_setting' and _name = 'carriedforward_template';
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_index])VALUES('base_setting','carriedforward_template','结转方式模板','FormCarriedForwardTemplate',7);

delete from _MenuTableMap where _group = 'account' and _name = 'carriedforward';
INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_index])VALUES('account','carriedforward','月度结转','FormCarriedForward',10);


go


delete from _MenuTableMap where _group = 'account' and _name = 'profit_sheet'

INSERT INTO [_MenuTableMap]
           ([_group]
           ,[_name]
           ,[_header]
           ,[_financeForm]
           ,[_index])
 VALUES
           ('account'
           ,'profit_sheet'
           ,'利润表'
           ,'FormProfitSheet'         
           ,9)
    