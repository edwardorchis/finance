using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Finance.Account.Controls.Commons
{
    public class AdornerHelper
    {
        #region 是否有Adorner
        public static int GetHasAdorner(DependencyObject obj)
        {
            return (int)obj.GetValue(HasAdornerProperty);
        }

        public static void SetHasAdorner(DependencyObject obj, int level)
        {
            obj.SetValue(HasAdornerProperty, level);
        }

        // Using a DependencyProperty as the backing store for HasAdorner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasAdornerProperty =
            DependencyProperty.RegisterAttached("HasAdorner", typeof(int), typeof(AdornerHelper), new PropertyMetadata(0, PropertyChangedCallBack));

        private static void PropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var val = (int)e.NewValue;           
            var element = d as Visual;

            if (element != null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(element);                   
                if (adornerLayer != null)
                {
                    Adorner[] adorners = adornerLayer.GetAdorners(element as UIElement);
                    if (adorners != null)
                        for (int i = 0; i < adorners.Count(); i++)
                        {
                            adornerLayer.Remove(adorners[i]);
                        }
                    if (val > 0)
                    {
                        adornerLayer.Add(new NotifyAdorner((element as UIElement), val));
                    }
                }
            }
        }
        #endregion

        #region MyRegion


        public static bool GetIsShowAdorner(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsShowAdornerProperty);
        }

        public static void SetIsShowAdorner(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowAdornerProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsShowAdorner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowAdornerProperty =
            DependencyProperty.RegisterAttached("IsShowAdorner", typeof(bool), typeof(AdornerHelper), new PropertyMetadata(false, IsShowChangedCallBack));

        private static void IsShowChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;

            if (element != null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(element);
                if (adornerLayer != null)
                {
                    var adorners = adornerLayer.GetAdorners(element);
                    if (adorners != null && adorners.Count() != 0)
                    {
                        var adorner = adorners.FirstOrDefault() as NotifyAdorner;

                        if (adorner == null)
                        {
                            return;
                        }

                        if ((bool)e.NewValue)
                        {
                            adorner.ShowAdorner();
                        }
                        else
                        {
                            adorner.HideAdorner();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
