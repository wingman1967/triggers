using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Repository for all database functions and calls on behalf of the assembly
    /// </summary>
    class DatabaseFactory
    {
        static string SQLCommand;
        static string connectionString = "server = grcdslsql0.dom.grc; database = SL_MAN_App; enlist=false; User ID = sa_config; Password = options23";
        public static void WriteRecordBOM(ref zCfgBOM bom)
        {
            SQLCommand = "Insert Into GR_CfgBOM (order_num,order_line_num,seq,parent_id,id,item_num,smartpart_num,unit_price,unit_cost,discount_amt,quantity) values (" + (char)39 + bom.CO_Num + (char)39 + "," + (char)39 + bom.CO_Line + (char)39 + "," + (char)39 + bom.Sequence + (char)39 + "," + (char)39 + bom.Parent + (char)39 + "," + (char)39 + bom.Identifier + (char)39 + "," + (char)39 + bom.Item + (char)39 + "," + (char)39 + bom.Smartpart + (char)39 + "," + (char)39 + bom.UnitPrice + (char)39 + "," + (char)39 + bom.UnitCost + (char)39 + "," + (char)39 + bom.Discount + (char)39 + "," + (char)39 + bom.QTY + (char)39 + ")";
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
        }
        public static void WriteRecordCItem(ref zCfgItem item)
        {
            string validateDescription = item.Desc;
            if (validateDescription.Length > 40)
            {
                validateDescription = validateDescription.Substring(0, 40);
            }

            SQLCommand = "Insert Into GR_CfgItem (order_num,order_line_num,seq,smartpart_num,item_num,description,cost,price,sell_price,weight,uom) values (" + (char)39 + item.CO_Num + (char)39 + "," + (char)39 + item.CO_Line + (char)39 + "," + (char)39 + item.Sequence + (char)39 + "," + (char)39 + item.Smartpart + (char)39 + "," + (char)39 + item.Item + (char)39 + "," + (char)39 + validateDescription + (char)39 + "," + (char)39 + item.ItemCost + (char)39 + "," + (char)39 + item.ItemPrice + (char)39 + "," + (char)39 + item.ItemSellPrice + (char)39 + "," + (char)39 + item.ItemWeight + (char)39 + "," + (char)39 + item.UnitOfMeasure + (char)39 + ")";
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
        }
        public static void WriteRecordCfg(ref zCfgParmVal cfg)
        {
            string cfName = cfg.CName;
            string cfValue = cfg.CValue;
            string cfLabel = cfg.CLabel;
            //uncomment this section if decision is made to revisit truncating data
            //switch(cfg.CName.Length > 255)
            //{
            //    case true:
            //        cfName = cfg.CName.Substring(0, 255);
            //        break;
            //    default:
            //        cfName = cfg.CName;
            //        break;
            //}
            //switch(cfg.CValue.Length > 255)
            //{
            //    case true:
            //        cfValue = cfg.CValue.Substring(0, 255);
            //        break;
            //    default:
            //        cfValue = cfg.CValue;
            //        break;
            //}
            //switch(cfg.CLabel.Length > 255)
            //{
            //    case true:
            //        cfLabel = cfg.CLabel.Substring(0, 255);
            //        break;
            //    default:
            //        cfLabel = cfg.CLabel;
            //        break;
            //}

            SQLCommand = "Insert Into GR_CfgParmVal (order_num,order_line_num,name,value,type,label) values (" + (char)39 + cfg.CO_Num + (char)39 + "," + (char)39 + cfg.CO_Line + (char)39 + "," + (char)39 + cfName + (char)39 + "," + (char)39 + cfValue + (char)39 + "," + (char)39 + cfg.CType + (char)39 + "," + (char)39 + cfLabel + (char)39 + ")";
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
        }
        public static void WriteRecordCOItem(ref zCfgCOitem coitem)
        {
            string validateDescription = coitem.Desc;
            if (validateDescription.Length > 40)
            {
                validateDescription = validateDescription.Substring(0, 40);
            }

            SQLCommand = "Insert Into GR_CfgCOItem (order_num,order_line_num,ser_num,item_num,smartpart_num,description,unit_price,unit_cost,discount_amt,quantity) values (" + (char)39 + coitem.CO_Num + (char)39 + "," + (char)39 + coitem.CO_Line + (char)39 + "," + (char)39 + coitem.Serial + (char)39 + "," + (char)39 + coitem.Item + (char)39 + "," + (char)39 + coitem.Smartpart + (char)39 + "," + (char)39 + validateDescription + (char)39 + "," + (char)39 + coitem.UnitPrice + (char)39 + "," + (char)39 + coitem.UnitCost + (char)39 + "," + (char)39 + coitem.Discount + (char)39 + "," + (char)39 + coitem.QTY + (char)39 + ")";
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
        }
        public static void WriteRecordCO(ref zCfgCO co)
        {
            SQLCommand = "Insert Into GR_CfgCO (id,order_num,order_ref_num,cust_name,cust_ref_num,account_num,erp_reference_num,project,payment_terms,ship_via,shipping_terms,bill_to_contact_name,bill_address_line_1,bill_address_line_2,bill_address_line_3,bill_to_city,bill_to_state,bill_to_country,bill_to_postal_code,bill_to_phone_number,bill_to_fax_number,bill_to_email_address,bill_to_ref_num,ship_to_contact_name,ship_address_line_1,ship_address_line_2,ship_address_line_3,ship_to_city,ship_to_state,ship_to_country,ship_to_postal_code,ship_to_phone_number,ship_to_fax_number,ship_to_email_address,ship_to_ref_num) values (" + (char)39 + co.Identifier + (char)39 + "," + (char)39 + co.CO_Num + (char)39 + "," + (char)39 + co.CORefNum + (char)39 + "," + (char)39 + co.CustName + (char)39 + "," + (char)39 + co.CustRefNum + (char)39 + "," + (char)39 + co.AccountNum + (char)39 + "," + (char)39 + co.ErpReferenceNum + (char)39 + "," + (char)39 + co.Project + (char)39 + "," + (char)39 + co.PaymentTerms + (char)39 + "," + (char)39 + co.ShipVia + (char)39 + "," + (char)39 + co.ShippingTerms + (char)39 + "," + (char)39 + co.BillToContactName + (char)39 + "," + (char)39 + co.BillToAddressLine1 + (char)39 + "," + (char)39 + co.BillToAddressLine2 + (char)39 + "," + (char)39 + co.BillToAddressLine3 + (char)39 + "," + (char)39 + co.BillToCity + (char)39 + "," + (char)39 + co.BillToState + (char)39 + "," + (char)39 + co.BillToCountry + (char)39 + "," + (char)39 + co.BillToPostalCode + (char)39 + "," + (char)39 + co.BillToPhoneNumber + (char)39 + "," + (char)39 + co.BillToFaxNumber + (char)39 + "," + (char)39 + co.BillToEmailAddress + (char)39 + "," + (char)39 + co.BillToRefNum + (char)39 + "," + (char)39 + co.ShipToContactName + (char)39 + "," + (char)39 + co.ShipToAddressLine1 + (char)39 + "," + (char)39 + co.ShipToAddressLine2 + (char)39 + "," + (char)39 + co.ShipToAddressLine3 + (char)39 + "," + (char)39 + co.ShipToCity + (char)39 + "," + (char)39 + co.ShipToState + (char)39 + "," + (char)39 + co.ShipToCountry + (char)39 + "," + (char)39 + co.ShipToPostalCode + (char)39 + "," + (char)39 + co.ShipToPhoneNumber + (char)39 + "," + (char)39 + co.ShipToFaxNumber + (char)39 + "," + (char)39 + co.ShipToEmailAddress + (char)39 + "," + (char)39 + co.ShipToRefNum + (char)39 + ")";
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
        }
        public static void ResequenceBOM(string orderNumber, int orderLine)
        {
            //call SP to resequence BOM records based on parentIDs
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("GR_Cfg_ResequenceBOMSp", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@OrderNumber", orderNumber);
            command.Parameters.AddWithValue("@OrderLine", orderLine);
            connection.Open();
            command.CommandTimeout = 600;
            command.ExecuteNonQuery();
            connection.Close();
        }
        public static void CleanupOrder(string orderNum)
        {
            SQLCommand = "DELETE From GR_CfgCO where order_num = " + (char)39 + orderNum + (char)39;
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
            SQLCommand = "DELETE From GR_CfgCOItem where order_num = " + (char)39 + orderNum + (char)39;
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
            SQLCommand = "DELETE From GR_CfgBOM where order_num = " + (char)39 + orderNum + (char)39;
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
            SQLCommand = "DELETE From GR_CfgItem where order_num = " + (char)39 + orderNum + (char)39;
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
            SQLCommand = "DELETE From GR_CfgParmVal where order_num = " + (char)39 + orderNum + (char)39;
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
        }
    }
}

