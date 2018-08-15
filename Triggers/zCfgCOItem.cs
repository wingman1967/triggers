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
        private DateTime due_date;
        private string config_type;
        private string cust_po;
        private string orderline_notes;
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
        public DateTime DueDate
        {
            get { return due_date; }
            set { due_date = value; }
        }
        public string ConfigType
        {
            get { return config_type; }
            set { config_type = value; }
        }
        public string CustPO
        {
            get { return cust_po; }
            set { cust_po = value; }
        }
        public string OrderLineNotes
        {
            get { return orderline_notes; }
            set { orderline_notes = value; }
        }
        public static void ClearCOItem(ref zCfgCOitem coitem)
        {
            coitem.ConfigType = "";
            coitem.CO_Line = 1;
            coitem.CO_Num = "";
            coitem.CustPO = "";
            coitem.Desc = "";
            coitem.Discount = 0;
            coitem.DueDate = System.DateTime.Now;
            coitem.Item = "";
            coitem.OrderLineNotes = "";
            coitem.PriorityLevel = 0;
            coitem.QTY = 0;
            coitem.Serial = "";
            coitem.Smartpart = "";
            coitem.UnitCost = 0;
            coitem.UnitPrice = 0;
        }
    }
}
