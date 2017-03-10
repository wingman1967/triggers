using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace ConfigureOneFlag
{
    class zCfgItem
    {
        private string order_num;
        private int order_line_num;
        private int seq;
        private string smartpart_num;
        private string item_num;
        private string description;
        private decimal cost;
        private decimal price;
        private decimal sell_price;
        private decimal weight;
        private string uom;

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
        public int Sequence
        {
            get { return seq; }
            set { seq = value; }
        }
        public string Smartpart
        {
            get { return smartpart_num; }
            set { smartpart_num = value; }
        }
        public string Item
        {
            get { return item_num; }
            set { item_num = value; }
        }
        public string Desc
        {
            get { return description; }
            set { description = value; }
        }
        public decimal ItemCost
        {
            get { return cost; }
            set { cost = value; }
        }
        public decimal ItemPrice
        {
            get { return price; }
            set { price = value; }
        }
        public decimal ItemSellPrice
        {
            get { return sell_price; }
            set { sell_price = value; }
        }
        public decimal ItemWeight
        {
            get { return weight; }
            set { weight = value; }
        }
        public string UnitOfMeasure
        {
            get { return uom; }
            set { uom = value; }
        }
    }
}
