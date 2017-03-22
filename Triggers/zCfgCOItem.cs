using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Accessor class for the CO Item
    /// </summary>
    public class zCfgCOitem
    {
        private string order_num;
        private int order_line_num;
        private string ser_num;
        private string item_num;
        private string smartpart_num;
        private string description;
        private decimal unit_price;
        private decimal unit_cost;
        private decimal discount_amt;
        private decimal quantity;
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
        public string Serial
        {
            get { return ser_num; }
            set { ser_num = value; }
        }
        public string Item
        {
            get { return item_num; }
            set { item_num = value; }
        }
        public string Smartpart
        {
            get { return smartpart_num; }
            set { smartpart_num = value; }
        }
        public string Desc
        {
            get { return description; }
            set { description = value; }
        }
        public decimal UnitPrice
        {
            get { return unit_price; }
            set { unit_price = value; }
        }
        public decimal UnitCost
        {
            get { return unit_cost; }
            set { unit_cost = value; }
        }
        public decimal Discount
        {
            get { return discount_amt; }
            set { discount_amt = value; }
        }
        public decimal QTY
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public int PriorityLevel
        {
            get { return priority_level; }
            set { priority_level = value; }
        }
    }
}
