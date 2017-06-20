using System.Data.SqlClient;
using System.Data;
using Microsoft.Win32;
using System.IO;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Repository for all database functions and calls on behalf of the assembly
    /// </summary>
    class DatabaseFactory
    {
        static string SQLCommand;
        public static string connectionString = "";
        public static string emailaddr = "";
        public static string emailServer = "";
        public static string emailFrom = "";
        public static string splocation = "";
        public static string spuname = "";
        public static string sppassword = "";
        public void SetConnectionString()
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\ConfigureOneAssembly\\1.0", true);
            if (reg == null)
            {
                //create keys if they don't already exist and set values
                reg = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\ConfigureOneAssembly\\1.0");
                string dbConnectionData = File.ReadAllText(@"C:\C1\dbconnection.dat");
                string dbkey = File.ReadAllText(@"C:\C1\dbkey.dat");
                emailaddr = File.ReadAllText(@"C:\C1\emailaddr.dat");
                emailServer = File.ReadAllText(@"C:\C1\emailserver.dat");
                emailFrom = File.ReadAllText(@"C:\C1\emailFromAddress.dat");
                splocation = File.ReadAllText(@"C:\C1\splocation.dat");
                spuname = File.ReadAllText(@"C:\C1\spuname.dat");
                sppassword = File.ReadAllText(@"C:\C1\sppassword.dat");
                reg.SetValue("DB", dbConnectionData);
                reg.SetValue("ENCKEY", dbkey);
                reg.SetValue("EMAILADDR", emailaddr);
                reg.SetValue("EMAILSERVER", emailServer);
                reg.SetValue("EMAILFROM", emailFrom);
                reg.SetValue("SPLOCATION", splocation);
                reg.SetValue("SPUNAME", spuname);
                reg.SetValue("SPPASSWORD", sppassword);
            }
            
            //decrypt cs
            string sKey = reg.GetValue("ENCKEY").ToString();
            connectionString = reg.GetValue("DB").ToString();
            emailaddr = reg.GetValue("EMAILADDR").ToString();
            emailServer = reg.GetValue("EMAILSERVER").ToString();
            emailFrom = reg.GetValue("EMAILFROM").ToString();
            splocation = reg.GetValue("SPLOCATION").ToString();
            spuname = reg.GetValue("SPUNAME").ToString();
            sppassword = reg.GetValue("SPPASSWORD").ToString();
            crypto csec = new crypto();
            csec.DecryptCS(connectionString, sKey);
            
            //deboog
            //connectionString = "server = grcdslsql0.dom.grc; database = SL_man_App; enlist=false; User ID = sa_config; Password = options23";
            //end deboog
        }        
        public static void WriteRecordBOM(ref zCfgBOM bom)
        {
            SQLCommand = "Insert Into GR_CfgBOM (order_num,order_line_num,seq,parent_id,id,item_num,smartpart_num,unit_price,unit_cost,discount_amt,quantity,priority_level) values (" + (char)39 + bom.CO_Num + (char)39 + "," + (char)39 + bom.CO_Line + (char)39 + "," + (char)39 + bom.Sequence + (char)39 + "," + (char)39 + bom.Parent + (char)39 + "," + (char)39 + bom.Identifier.Substring(0, C1Dictionaries.DBFieldLenBOM("BOM:id", ref bom)) + (char)39 + "," + (char)39 + bom.Item.Substring(0, C1Dictionaries.DBFieldLenBOM("BOM:item_num", ref bom)).Replace("'","''") + (char)39 + "," + (char)39 + bom.Smartpart.Substring(0, C1Dictionaries.DBFieldLenBOM("BOM:smartpart_num", ref bom)).Replace("'","''") + (char)39 + "," + (char)39 + bom.UnitPrice + (char)39 + "," + (char)39 + bom.UnitCost + (char)39 + "," + (char)39 + bom.Discount + (char)39 + "," + (char)39 + bom.QTY + (char)39 + "," + (char)39 + bom.PriorityLevel + (char)39 + ")";
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
        }
        public static void WriteRecordCItem(ref zCfgItem item)
        {
            SQLCommand = "Insert Into GR_CfgItem (order_num,order_line_num,seq,smartpart_num,item_num,description,cost,price,sell_price,weight,uom,priority_level) values (" + (char)39 + item.CO_Num + (char)39 + "," + (char)39 + item.CO_Line + (char)39 + "," + (char)39 + item.Sequence + (char)39 + "," + (char)39 + item.Smartpart.Substring(0, C1Dictionaries.DBFieldLenCfgItem("CfgItem:smartpart_num", ref item)).Replace("'","''") + (char)39 + "," + (char)39 + item.Item.Substring(0, C1Dictionaries.DBFieldLenCfgItem("CfgItem:item_num", ref item)).Replace("'","''") + (char)39 + "," + (char)39 + item.Desc.Substring(0, C1Dictionaries.DBFieldLenCfgItem("CfgItem:description", ref item)).Replace("'","''") + (char)39 + "," + (char)39 + item.ItemCost + (char)39 + "," + (char)39 + item.ItemPrice + (char)39 + "," + (char)39 + item.ItemSellPrice + (char)39 + "," + (char)39 + item.ItemWeight + (char)39 + "," + (char)39 + item.UnitOfMeasure.Substring(0, C1Dictionaries.DBFieldLenCfgItem("CfgItem:uom", ref item)).Replace("'","''") + (char)39 + "," + (char)39 + item.PriorityLevel + (char)39 + ")";
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
            SQLCommand = "Insert Into GR_CfgParmVal (order_num,order_line_num,name,value,type,label,priority_level) values (" + (char)39 + cfg.CO_Num + (char)39 + "," + (char)39 + cfg.CO_Line + (char)39 + "," + (char)39 + cfName.Substring(0, C1Dictionaries.DBFieldLenParmVal("CfgParmVal:name", ref cfg)).Replace("'","''") + (char)39 + "," + (char)39 + cfValue.Substring(0, C1Dictionaries.DBFieldLenParmVal("CfgParmVal:value", ref cfg)).Replace("'","''") + (char)39 + "," + (char)39 + cfg.CType.Substring(0, C1Dictionaries.DBFieldLenParmVal("CfgParmVal:type", ref cfg)).Replace("'","''") + (char)39 + "," + (char)39 + cfLabel.Substring(0, C1Dictionaries.DBFieldLenParmVal("CfgParmVal:label", ref cfg)).Replace("'","''") + (char)39 + "," + (char)39 + cfg.PriorityLevel + (char)39 + ")";
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
        }
        public static void WriteRecordCOItem(ref zCfgCOitem coitem)
        {
            SQLCommand = "Insert Into GR_CfgCOItem (order_num,order_line_num,ser_num,item_num,smartpart_num,description,unit_price,unit_cost,discount_amt,quantity,priority_level,due_date,config_type,CustPO) values (" + (char)39 + coitem.CO_Num + (char)39 + "," + (char)39 + coitem.CO_Line + (char)39 + "," + (char)39 + coitem.Serial.Substring(0, C1Dictionaries.DBFieldLenCOItem("COItem:ser_num", ref coitem)) + (char)39 + "," + (char)39 + coitem.Item.Substring(0, C1Dictionaries.DBFieldLenCOItem("COItem:item_num", ref coitem)) + (char)39 + "," + (char)39 + coitem.Smartpart.Substring(0, C1Dictionaries.DBFieldLenCOItem("COItem:smartpart_num", ref coitem)) + (char)39 + "," + (char)39 + coitem.Desc.Substring(0, C1Dictionaries.DBFieldLenCOItem("COItem:description", ref coitem)).Replace("'","''") + (char)39 + "," + (char)39 + coitem.UnitPrice + (char)39 + "," + (char)39 + coitem.UnitCost + (char)39 + "," + (char)39 + coitem.Discount + (char)39 + "," + (char)39 + coitem.QTY + (char)39 + "," + (char)39 + coitem.PriorityLevel + (char)39 + "," + (char)39 + coitem.DueDate + (char)39 + "," + (char)39 + coitem.ConfigType + (char)39 + "," + (char)39 + coitem.CustPO.Replace("'","''") + (char)39 + ")";
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
        }
        public static void WriteRecordCO(ref zCfgCO co)
        {
            SQLCommand = "Insert Into GR_CfgCO (id,order_num,order_ref_num,cust_name,cust_ref_num,account_num,erp_reference_num,project,payment_terms,ship_via,shipping_terms,bill_to_contact_name,bill_address_line_1,bill_address_line_2,bill_address_line_3,bill_to_city,bill_to_state,bill_to_country,bill_to_postal_code,bill_to_phone_number,bill_to_fax_number,bill_to_email_address,bill_to_ref_num,ship_to_contact_name,ship_address_line_1,ship_address_line_2,ship_address_line_3,ship_to_city,ship_to_state,ship_to_country,ship_to_postal_code,ship_to_phone_number,ship_to_fax_number,ship_to_email_address,ship_to_ref_num,priority_level,due_date,CustPO,ship_address_line_4,freight_terms,freight_acct,quote_nbr,web_user_name,web_order_date,dropship_address1, dropship_address2,dropship_address3,dropship_address4,dropship_city,dropship_state,dropship_zip,dropship_name,dropship_contact,dropship_country,dropship_phone,dropship_email) values (" + (char)39 + co.Identifier + (char)39 + "," + (char)39 + co.CO_Num + (char)39 + "," + (char)39 + co.CORefNum + (char)39 + "," + (char)39 + co.CustName.Replace("'","''") + (char)39 + "," + (char)39 + co.CustRefNum + (char)39 + "," + (char)39 + co.AccountNum + (char)39 + "," + (char)39 + co.ErpReferenceNum + (char)39 + "," + (char)39 + co.Project.Replace("'", "''") + (char)39 + "," + (char)39 + co.PaymentTerms.Replace("'", "''") + (char)39 + "," + (char)39 + co.ShipVia.Substring(0, C1Dictionaries.DBFieldLenCO("CO:ship_via", ref co)).Replace("'", "''") + (char)39 + "," + (char)39 + co.ShippingTerms.Substring(0, C1Dictionaries.DBFieldLenCO("CO:shipping_terms", ref co)).Replace("'", "''") + (char)39 + "," + (char)39 + co.BillToContactName.Replace("'", "''") + (char)39 + "," + (char)39 + co.BillToAddressLine1.Replace("'", "''") + (char)39 + "," + (char)39 + co.BillToAddressLine2 + (char)39 + "," + (char)39 + co.BillToAddressLine3 + (char)39 + "," + (char)39 + co.BillToCity + (char)39 + "," + (char)39 + co.BillToState + (char)39 + "," + (char)39 + co.BillToCountry + (char)39 + "," + (char)39 + co.BillToPostalCode + (char)39 + "," + (char)39 + co.BillToPhoneNumber.Substring(0, C1Dictionaries.DBFieldLenCO("CO:bill_to_phone_number", ref co)) + (char)39 + "," + (char)39 + co.BillToFaxNumber.Substring(0, C1Dictionaries.DBFieldLenCO("CO:bill_to_fax_number", ref co)) + (char)39 + "," + (char)39 + co.BillToEmailAddress + (char)39 + "," + (char)39 + co.BillToRefNum.Replace("'", "''") + (char)39 + "," + (char)39 + co.ShipToContactName.Replace("'", "''") + (char)39 + "," + (char)39 + co.ShipToAddressLine1 + (char)39 + "," + (char)39 + co.ShipToAddressLine2 + (char)39 + "," + (char)39 + co.ShipToAddressLine3 + (char)39 + "," + (char)39 + co.ShipToCity + (char)39 + "," + (char)39 + co.ShipToState + (char)39 + "," + (char)39 + co.ShipToCountry + (char)39 + "," + (char)39 + co.ShipToPostalCode + (char)39 + "," + (char)39 + co.ShipToPhoneNumber + (char)39 + "," + (char)39 + co.ShipToFaxNumber + (char)39 + "," + (char)39 + co.ShipToEmailAddress + (char)39 + "," + (char)39 + co.ShipToRefNum + (char)39 + "," + (char)39 + co.PriorityLevel + (char)39 + "," + (char)39 + co.DueDate + (char)39 + "," + (char)39 + co.CustPO.Substring(0, C1Dictionaries.DBFieldLenCO("CO:CustPO", ref co)) + (char)39 + "," + (char)39 + co.ShipToAddressLine4 + (char)39 + "," + (char)39 + co.FreightTerms + (char)39 + "," + (char)39 + co.FreightAcct + (char)39 + "," + (char)39 + co.QuoteNbr + (char)39 + "," + (char)39 + co.WebUserName.Replace("'", "''") + (char)39 + "," + (char)39 + co.WebOrderDate + (char)39 + "," + (char)39 + co.DropShipAddress1 + (char)39 + "," + (char)39 + co.DropShipAddress2 + (char)39 + "," + (char)39 + co.DropShipAddress3 + (char)39 + "," + (char)39 + co.DropShipAddress4 + (char)39 + "," + (char)39 + co.DropShipCity + (char)39 + "," + (char)39 + co.DropShipState + (char)39 + "," + (char)39 + co.DropShipZip + (char)39 + "," + (char)39 + co.DropShipName + (char)39 + "," + (char)39 + co.DropShipContact + (char)39 + "," + (char)39 + co.DropShipCountry + (char)39 + "," + (char)39 + co.DropShipPhone + (char)39 + "," + (char)39 + co.DropShipEmail + (char)39 + ")";
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
        }
        public static string UserName(string id)
        {
            string name = "N/A";
            SQLCommand = "Select cfg_user_name as UNAME From GR_CfgUserID where cfg_user_id like " + (char)39 + "%" + id + "%" + (char)39;
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.CommandTimeout = 600;
                using (SqlDataReader reader = myCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            name = reader["UNAME"].ToString();
                        }
                    }
                    reader.Close();
                }
                myConnection.Close();
            }
            return name;
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
        public static void CfgImport(string orderNumber)
        {
            //call SP to import data into Syteline
            int CreateOrder = 1;
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("GR_CfgImportSp", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@pStartingOrderNum", orderNumber);
            command.Parameters.AddWithValue("@pEndingOrderNum", orderNumber);
            command.Parameters.AddWithValue("@pCreateOrder", CreateOrder);
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
        public static void WriteAuditRecord(string auditMessageL, string orderNum, int orderLine, string auditEvent)
        {
            SQLCommand = "Insert Into GR_Cfg_Audit (order_num,order_line_num,auditEvent,auditMessageL) values (" + (char)39 + orderNum + (char)39 + "," + (char)39 + orderLine + (char)39 + "," + (char)39 + auditEvent + (char)39 + "," + (char)39 + auditMessageL + (char)39 + ")";
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
        }
        public static string RetrieveSLCO (string orderNum)
        {
            string SLOrderNumber = "";
            SQLCommand = "Select TOP 1 co_num as SLCO From CO c Where c.uf_weborder = " + (char)39 + orderNum + (char)39 + " Order By c.order_date desc, c.co_num desc";
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(SQLCommand, myConnection);
                myConnection.Open();
                using (SqlDataReader reader = myCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            SLOrderNumber = reader["SLCO"].ToString();
                        }
                    }
                    reader.Close();
                }
                myConnection.Close();
            }
            return SLOrderNumber;
        }
    }
}

