
if not exists(select 1 from syscolumns where id=(select max(id) from sysobjects where xtype='u' and name='_VoucherEntry')
 and name ='_uniqueKey')
	alter table _VoucherEntry add _uniqueKey varchar(50);
