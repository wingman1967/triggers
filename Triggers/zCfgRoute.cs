using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Accessor class for the cfg route object (C1 routing module data)
    /// </summary>
    public class zCfgRoute
    {
        private string order_num;
        private int order_line_num;
        private int seq;
        private string type;
        private string bom_id;
        private string item_num;
        private string smartpart_num;
        private int oper_num;
        private string wc;
        private string description;
        private double labor_hrs;
        private double setup_hrs;
        private double labor_rate;
        private string notes;
        private string mach_name;
        private double run_time;
        private string matl_item_num;
        private string matl_smartpart_num;
        private decimal matl_quantity;
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
        public int Seq
        {
            get { return seq; }
            set { seq = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public string BOM_ID
        {
            get { return bom_id; }
            set { bom_id = value; }
        }
        public string ItemNum
        {
            get { return item_num; }
            set { item_num = value; }
        }
        public string SmartpartNum
        {
            get { return smartpart_num; }
            set { smartpart_num = value; }
        }
        public int OPERATION
        {
            get { return oper_num; }
            set { oper_num = value; }
        }
        public string WC
        {
            get { return wc; }
            set { wc = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public double Labor_Hours
        {
            get { return labor_hrs; }
            set { labor_hrs = value; }
        }
        public double Setup_Hours
        {
            get { return setup_hrs; }
            set { setup_hrs = value; }
        }
        public double Labor_Rate
        {
            get { return labor_rate; }
            set { labor_rate = value; }
        }
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }
        public string Machine_Name
        {
            get { return mach_name; }
            set { mach_name = value; }
        }
        public double Run_Time
        {
            get { return run_time; }
            set { run_time = value; }
        }
        public string MatlItemNum
        {
            get { return matl_item_num; }
            set { matl_item_num = value; }
        }
        public string MatlSmartpartNum
        {
            get { return matl_smartpart_num; }
            set { matl_smartpart_num = value; }
        }
        public decimal MatlQty
        {
            get { return matl_quantity; }
            set { matl_quantity = value; }
        }
    }
}
