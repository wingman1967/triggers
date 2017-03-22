using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Accessor class for the cfg parmValue object (C1 Inputs)
    /// </summary>
    public class zCfgParmVal
    {
        private string order_num;
        private int order_line_num;
        private string name;
        private string val;
        private string type;
        private string label;
        private int priority_level;
        public string CO_Num
        {
            get { return order_num; }
            set { order_num = value; }
        }
        public int CO_Line
        {
            get { return order_line_num; }
            set { order_line_num = value; }
        }
        public string CName
        {
            get { return name; }
            set { name = value; }
        }
        public string CValue
        {
            get { return val; }
            set { val = value; }
        }
        public string CType
        {
            get { return type; }
            set { type = value; }
        }
        public string CLabel
        {
            get { return label; }
            set { label = value; }
        }
        public int PriorityLevel
        {
            get { return priority_level; }
            set { priority_level = value; }
        }
    }
}
