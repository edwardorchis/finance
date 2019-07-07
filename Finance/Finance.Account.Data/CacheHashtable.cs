using System.Collections;

namespace Finance.Account.Data
{
    public class CacheHashtable
    {
        Hashtable htSyn = Hashtable.Synchronized(new Hashtable());      
        public dynamic Get(CacheHashkey key)
        {          
            if(htSyn.ContainsKey(key))
                return htSyn[key];
            throw new FinanceAccountDataException(FinanceAccountDataErrorCode.CACHE_DATA_NOT_EXIST);
        }

        public void Set(CacheHashkey key, dynamic value)
        {
            if (htSyn.ContainsKey(key))
                htSyn[key] = value;
            else
                htSyn.Add(key, value);
        }

        public void Remove(CacheHashkey key)
        {
            if (htSyn.ContainsKey(key))
                htSyn.Remove(key);
        }

        public bool ContainsKey(CacheHashkey key)
        {
            return htSyn.ContainsKey(key);
        }
    }
}
