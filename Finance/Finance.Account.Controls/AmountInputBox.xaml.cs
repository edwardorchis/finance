using Finance.Account.Controls.Commons;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Finance.Account.Controls
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    internal partial class AmountInputBox : UserControl
    {
        public class BeforeInputEventArgs
        {
            public bool Disable { set; get; }
        }

        public delegate void BeforeInputEventHandler(object sender, BeforeInputEventArgs e);
        public BeforeInputEventHandler BeforeInputEvent;
        //小数位数
        static int DECIMAL_DIGITS = 2;
        //个位的索引位置
        static int UNITS_DIGIT_INDEX = 12;
        //总的cell数
        static int MAX_CELL_INDEX = UNITS_DIGIT_INDEX + DECIMAL_DIGITS;

        Brush oldBackground = null;

        int m_HeadIndex = 12;

        enum EditModel
        {
            INTEGER,
            FRACTIONAL
        }

        EditModel m_EditModel = EditModel.FRACTIONAL;

        RadioButton[] cells = new RadioButton[MAX_CELL_INDEX+1];
        public AmountInputBox()
        {
            InitializeComponent();
            int i = 0;
            foreach (UIElement element in grid.Children)
            {
                if (element is RadioButton)
                {
                    cells[i] = (RadioButton)element;
                    ++i;
                }
            }

            this.KeyDown += AmountInputBox_KeyDown;
            this.KeyUp += AmountInputBox_KeyUp;
            this.GotFocus += AmountInputBox_GotFocus;
            this.LostFocus += AmountInputBox_LostFocus;
            InitCells();
            
        }
        public new int TabIndex
        {
            set
            {
                cells[UNITS_DIGIT_INDEX].TabIndex = value;
            }
            get
            {
                return cells[UNITS_DIGIT_INDEX].TabIndex;
            }
        }

        public decimal Value {
            set { SetValue(value);
                if (value == 0.00M)
                    InitCells();
            }
            get { return GetValue(); }
        }

        public bool IsReadOnly { set; get; }
        
        private void AmountInputBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsEnabled)
            {
                e.Handled = true;
                return;
            }
            foreach (RadioButton element in cells)
            {
                element.IsChecked = false;
            }
            if (oldBackground != null)
                this.Background = oldBackground;
            else
                this.Background = Consts.WHITE_BRUSH;
            if (GetValue() == 0.00M)
                InitCells();
        }

        private void AmountInputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
                return;
            oldBackground = this.Background;
            this.Background = Consts.HIGHLIGHT_BRUSH;
            cells[UNITS_DIGIT_INDEX].Focus();
            Commons.Keyboard.Press(Key.OemPlus);
         
        }

        private void Cell_GotFocus(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
                return;
            (sender as RadioButton).IsChecked = true;            
        }

        private void AmountInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsReadOnly)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                Commons.Keyboard.Press(Key.Tab);
                e.Handled = true;
                return;
            }
            else if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {                   
                Input((int)e.Key - 34);
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                Input((int)e.Key - 74);
            }
            else if (e.Key == Key.Escape || e.Key == Key.Subtract)
            {
                InitCells();
            }
            else if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
            {
                GetForcusCell();
                if (m_EditModel == EditModel.INTEGER)
                {
                    m_EditModel = EditModel.FRACTIONAL;
                    cells[UNITS_DIGIT_INDEX + 1].IsChecked = true;
                }
            }           
            else
            {
                var index = GetCellIndex(GetForcusCell());
                if (e.Key == Key.Back)
                {
                    //回退，删除前一
                    DeleteCellValue(index - 1);
                }
                else if (e.Key == Key.Delete)
                {
                    //删除当前
                    DeleteCellValue(index);
                }
                else if (e.Key == Key.Left)
                {
                    //左移一位
                    if (index > 0)
                    {
                        cells[index - 1].IsChecked = true;
                    }
                }
                else if (e.Key == Key.Right)
                {
                    //右移一位
                    if (index < MAX_CELL_INDEX)
                    {
                        cells[index + 1].IsChecked = true;
                    }
                }
                else if (e.Key == Key.Home)
                {
                    if (m_HeadIndex > 0)
                    {
                        cells[m_HeadIndex - 1].IsChecked = true;
                    }
                }
                else if (e.Key == Key.End)
                {
                    cells[UNITS_DIGIT_INDEX].IsChecked = true;
                }
                else
                {

                    return;
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// 删除index位置的值
        /// </summary>
        /// <param name="index"></param>
        private void DeleteCellValue(int index)
        {
            if (m_EditModel == EditModel.FRACTIONAL)
            {
                while (index < MAX_CELL_INDEX)
                {
                    cells[index].Content = cells[index + 1].Content;
                    index++;
                }
                cells[MAX_CELL_INDEX].Content = "0";
                return;
            }
            while (index > 0)
            {
                cells[index].Content = cells[index - 1].Content;
                index--;
                
            }
            cells[index].Content = null;
            m_HeadIndex++;
        }

        private void AmountInputBox_KeyUp(object sender, KeyEventArgs e)
        {
            //忽略动作
            e.Handled = true;
        }


        void InitCells()
        {
            int i = 0;
            while (i < MAX_CELL_INDEX+1)
            {
                cells[i].Content = null;
                cells[i].Foreground = this.Foreground;
                i++;
            }
            
            //标志是初始的值
            cells[UNITS_DIGIT_INDEX].Tag = "0";
            if (this.Background == null)
                this.Background = Consts.WHITE_BRUSH;
            m_HeadIndex = UNITS_DIGIT_INDEX;
        }

        void Input(int val)
        {
          
            var args = new BeforeInputEventArgs();
            BeforeInputEvent?.Invoke(this, args);
            if (args.Disable)
            {                
                return;
            }
          
            var cur = GetForcusCell();
            int index = GetCellIndex(cur);
            if (m_EditModel == EditModel.FRACTIONAL)
            {
                InputFractional(index,val.ToString());
                return;
            }
            
            if (index == UNITS_DIGIT_INDEX)
            {
                if (cur.Content == null)
                {
                    FillCell(index, val.ToString());
                    cur.Tag = null;
                    return;
                }
                else if(cur.Tag !=null )
                {
                    if (string.IsNullOrEmpty(cur.Content.ToString()) 
                        || cur.Tag.ToString() == "0")
                    {
                        FillCell(index, val.ToString());
                        cur.Tag = null;
                        return;
                    }
                }
            }

            bool bEmpty = false;
            int idxEmpty = UNITS_DIGIT_INDEX;
            while (idxEmpty > index)
            {
                var cellEmpty = cells[idxEmpty];
                if (cellEmpty.Content == null)
                {
                    bEmpty = true;
                    break;
                }
                else if(cellEmpty.Tag !=null && cellEmpty.Tag.ToString() == "0")
                {
                    bEmpty = true;
                    break;                  
                }
                idxEmpty--;
            }

            if (bEmpty)
            {
                cells[idxEmpty].IsChecked = true;
                m_HeadIndex = idxEmpty;
                FillCell(idxEmpty, val.ToString());
                if (cells[idxEmpty].Tag != null)
                {
                    if (idxEmpty == UNITS_DIGIT_INDEX && cells[idxEmpty].Tag.ToString() == "0")
                    {
                        cells[idxEmpty].Tag = null;
                    }
                }
            }
            else if (--m_HeadIndex >= 0)
            {
                int idx = m_HeadIndex;
                while (idx < index)
                {
                    FillCell(idx, cells[++idx].Content.ToString());
                }
                FillCell(idx, val.ToString());
            }           
        }
        void InputFractional(int index, string val)
        {
            //小数，光标在谁头上，值给谁
            cells[index].Content = val;           
            if (index < MAX_CELL_INDEX)
            {
                //光标向右移动一位
                cells[index+1].IsChecked = true;
            }
        }

        bool FillCell(int index,string val)
        {           
            var cur = cells[index];
            cur.Content = val;
            return true;           
        }

        RadioButton GetForcusCell()
        {
            m_EditModel = EditModel.INTEGER;
            for (int i = 0; i < MAX_CELL_INDEX + 1; i++)
            {
                if (cells[i].IsChecked == true)
                {
                    if (i > UNITS_DIGIT_INDEX)
                        m_EditModel = EditModel.FRACTIONAL;
                    
                    return cells[i];
                }
            }
            return cells[UNITS_DIGIT_INDEX];
        }

        int GetCellIndex(RadioButton  cell)
        {
            for(int i =0;i< MAX_CELL_INDEX + 1; i++)
            {
                if (cell == cells[i])
                    return i;
            }
            return 0;
        }

        private void SetValue(decimal val)
        {
            var strVal = val.ToString();
            //小数部分
            var tmp =new char[DECIMAL_DIGITS];
            int i = 0;
            while (i < DECIMAL_DIGITS)
            {
                tmp[i] = '0';
                i++;
            }
            var strFractional = new string(tmp) ;

            //整数部分的长度
            var intLen = 0;
            var pointPos = strVal.IndexOf('.');
            if (pointPos < 0)
            {
                intLen = strVal.Length;
            }
            else
            {
                intLen = pointPos;
                var strF = strVal.Substring(pointPos + 1);
                if (strF.Length > DECIMAL_DIGITS-1)
                {
                    strFractional = strF.Substring(0, DECIMAL_DIGITS);
                }
                else
                {
                    i = 0;
                    while (i < strF.Length)
                    {
                        tmp[i] = strF[i];
                        i++;
                    }
                    strFractional = new string(tmp);
                }
            }
            if (intLen > UNITS_DIGIT_INDEX + 1)
            {
                throw new OverflowException();
            }

            InitCells();

            m_HeadIndex = UNITS_DIGIT_INDEX - intLen + 1;
            i = 0;
            while (i < intLen)
            {
                cells[m_HeadIndex + i].Content = strVal[i] + "";
                i++;
            }
            cells[UNITS_DIGIT_INDEX].Tag = null;

            i = UNITS_DIGIT_INDEX;
            while (i < MAX_CELL_INDEX)
            {
                cells[i+1].Content = strFractional[i-UNITS_DIGIT_INDEX] + "";
                i++;
            }
        }

        

        private decimal GetValue()
        {
            int i = 0;
            var tmp = new char[MAX_CELL_INDEX+3];//有个小数点，多一位，再来一位‘\0’
            int index = 0;
            while (i < MAX_CELL_INDEX + 1)
            {
                var content = cells[i].Content;
                if (content != null)
                {
                    if (content.ToString() == "0" && index == 0 && i < UNITS_DIGIT_INDEX)
                    {
                        //整数部分最前头的0不处理
                    }
                    else
                    {
                        tmp[index] = content.ToString()[0];
                        index++;                       
                    }
                }
                if (i == UNITS_DIGIT_INDEX)
                {
                    tmp[index] = '.';
                    index++;
                }
                i++;
            }            
            var strVal = new string(tmp);
            decimal rsp = 0.00M;
            decimal.TryParse(strVal, out rsp);
            return rsp;
        }

    }
}
