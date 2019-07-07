using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Finance.Utils
{
    public interface IImportHandler
    {
        void SetTid(long tid);
        /// <summary>
        /// 设置导入文件名
        /// </summary>
        /// <returns></returns>
        string GetDTLFileName();
        /// <summary>
        /// 数据导入到内存中后，在插入数据库前处理解码等
        /// 会按照ds中table的index顺序从小到大进行提交，表名为DataTable.TableName
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        void Deconde(ref DataSet ds);
        /// <summary>
        /// 提交前处理函数
        /// </summary>
        /// <param name="tran">事务处理对象，不要在lib中提交和rollback</param>
        /// <returns></returns>
        void ActionBeforeCommit(dynamic tran);
        /// <summary>
        /// 事务提交后做些处理
        /// </summary>
        /// <returns></returns>
        void ActionAfterCommit();
    }
 
    public enum DTLMode
    {
        /// <summary>
        /// 只插入不存在的记录
        /// </summary>
        INSERTNOTEXIST,
        /// <summary>
        /// 覆盖，删除主键冲突的记录，插入新的
        /// </summary>
        COVER,
        /// <summary>
        /// 按照主键更新传入的字段
        /// </summary>
        UPDATE        
    }

   

}
