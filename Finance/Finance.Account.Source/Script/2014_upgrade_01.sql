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
select @billNo as _linkNo,1 as [_index],/*主营业务收入*/'6001.1001' as _accountSubjectNo,'销售产品A' as _explanation, 1000 as _amount, -1 as _direction, 
'5C46D0E37EA749CBB0C28C7C65DFD857' as _uniqueKey, '0' as _price_gold, '0' as _qty_gold, '1' as _price_stone, '5' as _qty_stone ,'' as _remark, 
'客户' as _item_type, '004' as _item_no, '客户丁' as _item_name, 10 as _act_price, 100 as _act_qty
union all 
select @billNo as _linkNo,2 as [_index],/*应收账款*/'1403' as _accountSubjectNo,'应收客户甲' as _explanation, 1000 as _amount, 1 as _direction,
 'B50E86494313480AA66849A0D091E46E' as _uniqueKey, '0' as _price_gold, '0' as _qty_gold, '1' as _price_stone, '5' as _qty_stone ,'' as _remark, 
 '客户' as _item_type, '004' as _item_no, '客户丁' as _item_name, 10 as _act_price, 100 as _act_qty

 go

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