using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceClient
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) {
        //    if (object.Equals(storage, value)) { return false; }
        //    storage = value;
        //    this.OnPropertyChanged(propertyName);
        //    return true;
        //}
        //private void OnPropertyChanged([CallerMemberName] string propertyName=null) {
        //    var eventHandler = this.PropertyChanged;
        //    if (eventHandler != null) {
        //        eventHandler(this,new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        private string GetProperyName(string methodName)
        {
            if (methodName.StartsWith("get_") || methodName.StartsWith("set_") ||
                methodName.StartsWith("put_"))
            {
                return methodName.Substring("get_".Length);
            }
            throw new Exception(methodName + " not a method of Property");
        }

        protected bool SetProperty<T>(ref T storage, T value)
        {
            if (object.Equals(storage, value)) { return false; }
            storage = value;
            string propertyName = GetProperyName(new System.Diagnostics.StackTrace(true).GetFrame(1).GetMethod().Name);
            this.OnPropertyChanged(propertyName);
            return true;
        }
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
