using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;
using Microsoft.Win32;
using System.IO;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Data.SqlTypes;
using Triggers;
using Triggers.GRCfg3TableAdapters;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Repository for all database functions and calls on behalf of the assembly
    /// </summary>
    class DatabaseFactory : abDatabase
    {
        //static abDatabase database;
        static string SQLCommand;
        public static string connectionString = "";
        public static string backupCS = "";
        public static string emailaddr = "";
        public static string emailServer = "";
        public static string emailFrom = "";
        public static string splocation = "";
        public static string spuname = "";
        public static string sppassword = "";
        public static string decryptedValue = "";
        public static bool debugLogging = false;
        public static string dbprotect = "";
        public static string ws_uname = "";
        public static string ws_password = "";
        public static string queueControlConnectionString = "";
        public static string queueControlConnectionStringValueDEV = "";
        public static string queueControlConnectionStringValuePROD = "";
        public void SetConnectionString()
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\ConfigureOneAssembly\\1.0", true);
            if (reg == null)
            {
                //create keys if they don't already exist and set values
                reg = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\ConfigureOneAssembly\\1.0");
                string dbConnectionData = File.ReadAllText(@"C:\C1\dbconnection.dat");
                string dbkey = File.ReadAllText(@"C:\C1\dbkey.dat");
                string proddbConnectionData = File.ReadAllText(@"C:\C1\proddbconnection.dat");
                emailaddr = File.ReadAllText(@"C:\C1\emailaddr.dat");
                emailServer = File.ReadAllText(@"C:\C1\emailserver.dat");
                emailFrom = File.ReadAllText(@"C:\C1\emailFromAddress.dat");
                splocation = File.ReadAllText(@"C:\C1\splocation.dat");
                spuname = File.ReadAllText(@"C:\C1\spuname.dat");
                sppassword = File.ReadAllText(@"C:\C1\sppassword.dat");
                dbprotect = File.ReadAllText(@"C:\C1\protect.dat");
                ws_uname = File.ReadAllText(@"C:\C1\ws_uname.dat");
                ws_password = File.ReadAllText(@"C:\C1\ws_password.dat");
                queueControlConnectionStringValueDEV = File.ReadAllText(@"C:\C1\queuecontroldev.dat");
                queueControlConnectionStringValuePROD = File.ReadAllText(@"C:\C1\queuecontrolprod.dat");
                //set registry values with defaults
                reg.SetValue("PROD_DB", proddbConnectionData);
                reg.SetValue("DB", dbConnectionData);
                reg.SetValue("ENCKEY", dbkey);
                reg.SetValue("EMAILADDR", emailaddr);
                reg.SetValue("EMAILSERVER", emailServer);
                reg.SetValue("EMAILFROM", emailFrom);
                reg.SetValue("SPLOCATION", splocation);
                reg.SetValue("SPUNAME", spuname);
                reg.SetValue("SPPASSWORD", sppassword);
                reg.SetValue("DEBUGLOGGING", "NO");
                reg.SetValue("PROTECT", dbprotect);
                reg.SetValue("WS_UNAME", ws_uname);
                reg.SetValue("WS_PASSWORD", ws_password);
                reg.SetValue("QueueControlConnectionDEV", queueControlConnectionStringValueDEV);
                reg.SetValue("QueueControlConnectionPROD", queueControlConnectionStringValuePROD);
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
            connectionString = crypto.DecryptCS(connectionString, sKey);
            spuname = crypto.DecryptCS(spuname, sKey);
            sppassword = crypto.DecryptCS(sppassword, sKey);
            dbprotect = reg.GetValue("PROTECT").ToString();
            ws_uname = reg.GetValue("WS_UNAME").ToString();
            ws_password = reg.GetValue("WS_PASSWORD").ToString();
            queueControlConnectionStringValueDEV = reg.GetValue("QueueControlConnectionDEV").ToString();
            queueControlConnectionStringValuePROD = reg.GetValue("QueueControlConnectionPROD").ToString();

            debugLogging = false;
            if (reg.GetValue("DEBUGLOGGING").ToString() == "YES")
            {
                debugLogging = true;
            }

            queueControlConnectionString = queueControlConnectionStringValueDEV;
            if (reg.GetValue("ENVIRONMENT").ToString() == "PROD") { queueControlConnectionString = queueControlConnectionStringValuePROD; }

            //if requested environment is not dev/test, change connectionString to prod; override to DEV if we sensed PROD but PROTECT in registry is YES
            if (Triggers.dbEnvironment == "PROD" && reg.GetValue("PROTECT").ToString() != "YES")
            {
                connectionString = reg.GetValue("PROD_DB").ToString();
                queueControlConnectionString = queueControlConnectionStringValuePROD;   //override ENVIRONMENT if incoming order is production
            }
            backupCS = connectionString;
        }        
        public static void WriteRecordBOM(ref zCfgBOM bom)
        {
            try
            {
                GR_CfgBOMTableAdapter tbl = new GR_CfgBOMTableAdapter();
                tbl.Connection.ConnectionString = connectionString;

                tbl.Insert(
                    bom.CO_Num,
                    bom.CO_Line,
                    bom.Sequence,
                    bom.Parent,
                    bom.Identifier,
                    bom.Item,
                    bom.Smartpart,
                    bom.UnitPrice,
                    bom.UnitCost,
                    bom.Discount,
                    bom.QTY,
                    bom.PriorityLevel,
                    bom.RecordSequence
                    );
            }
            catch (Exception eBOM)
            {
                Triggers.logEvent = "ERROR WRITING GR_CFGBOM: " + eBOM.Message;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
            }
        }
        public static void WriteRecordCItem(ref zCfgItem item)
        {
            try
            {
                GR_CfgItemTableAdapter tbl = new GR_CfgItemTableAdapter();
                tbl.Connection.ConnectionString = connectionString;

                tbl.Insert(
                    item.CO_Num,
                    item.CO_Line,
                    item.Sequence,
                    item.Smartpart,
                    item.Item,
                    item.Desc,
                    item.ItemCost,
                    item.ItemPrice,
                    item.ItemSellPrice,
                    item.ItemWeight,
                    item.UnitOfMeasure,
                    item.PriorityLevel,
                    item.IM_VAR1,
                    item.IM_VAR2,
                    item.IM_VAR3,
                    item.IM_VAR4,
                    item.IM_VAR5
                    );
            }
            catch (Exception eCI)
            {
                Triggers.logEvent = "ERROR WRITING GR_CFGITEM: " + eCI.Message;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
            }
        }
        public static void WriteRecordCfg(ref zCfgParmVal cfg)
        {
            try
            {
                GR_CfgParmValTableAdapter tbl = new GR_CfgParmValTableAdapter();
                tbl.Connection.ConnectionString = connectionString;

                tbl.Insert(
                    cfg.CO_Num,
                    cfg.CO_Line,
                    cfg.CName,
                    cfg.CValue,
                    cfg.CType,
                    cfg.CLabel,
                    cfg.PriorityLevel
                    );
            }
            catch (Exception eCFG)
            {
                Triggers.logEvent = "ERROR WRITING GR_CFGPARMVAL: " + eCFG.Message;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
            }
        }
        public static void WriteRecordCOItem(ref zCfgCOitem coitem)
        {
            try
            {
                GR_CfgCoitemTableAdapter tbl = new GR_CfgCoitemTableAdapter();
                tbl.Connection.ConnectionString = connectionString;

                tbl.Insert(
                    coitem.CO_Num,
                    coitem.CO_Line,
                    coitem.Serial,
                    coitem.Item,
                    coitem.Smartpart,
                    coitem.Desc,
                    coitem.UnitPrice,
                    coitem.UnitCost,
                    coitem.Discount,
                    coitem.QTY,
                    coitem.PriorityLevel,
                    coitem.DueDate,
                    coitem.ConfigType,
                    coitem.CustPO,
                    coitem.OrderLineNotes
                    );
            }
            catch (Exception coitemx)
            {
                Triggers.logEvent = "ERROR WRITING GR_CFGCOItem: " + coitemx.Message;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
            }
        }
        public static void WriteRecordCO(ref zCfgCO co)
        {
            try
            {
                GR_CfgCOTableAdapter tbl = new GR_CfgCOTableAdapter();
                tbl.Connection.ConnectionString = connectionString;

                tbl.Insert(
                    co.Identifier,
                    co.CO_Num,
                    co.CORefNum,
                    co.CustName,
                    co.CustRefNum,
                    co.AccountNum,
                    co.ErpReferenceNum,
                    co.Project,
                    co.PaymentTerms,
                    co.ShipVia,
                    co.ShippingTerms,
                    co.BillToContactName,
                    co.BillToAddressLine1,
                    co.BillToAddressLine2,
                    co.BillToAddressLine3,
                    co.BillToCity,
                    co.BillToState,
                    co.BillToCountry,
                    co.BillToPostalCode,
                    co.BillToPhoneNumber,
                    co.BillToFaxNumber,
                    co.BillToEmailAddress,
                    co.BillToRefNum,
                    co.ShipToContactName,
                    co.ShipToAddressLine1,
                    co.ShipToAddressLine2,
                    co.ShipToAddressLine3,
                    co.ShipToCity,
                    co.ShipToState,
                    co.ShipToCountry,
                    co.ShipToPostalCode,
                    co.ShipToPhoneNumber,
                    co.ShipToFaxNumber,
                    co.ShipToEmailAddress,
                    co.ShipToRefNum,
                    co.PriorityLevel,
                    null,
                    null,
                    co.DueDate,
                    co.CustPO,
                    co.DropShipAddress4,
                    co.FreightTerms,
                    co.FreightAcct,
                    co.QuoteNbr,
                    co.WebUserName,
                    co.WebOrderDate,
                    co.DropShipAddress1,
                    co.DropShipAddress2,
                    co.DropShipAddress3,
                    co.DropShipAddress4,
                    co.DropShipCity,
                    co.DropShipState,
                    co.DropShipZip,
                    co.DropShipName,
                    co.DropShipContact,
                    co.DropShipCountry,
                    co.DropShipPhone,
                    co.DropShipEmail,
                    co.RequestDate,
                    co.DestinationCountry,
                    co.OrderHeaderNotes,
                    co.EndUser,
                    co.Engineer);
            }
            catch (Exception coex)
            {
                Triggers.logEvent = "ERROR WRITING GR_CFGCO: " + coex.Message;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
            }
        }
        public static void WriteRecordCfgRoute(ref zCfgRoute route)
        {
            try
            {
                GR_CfgRouteBOMTableAdapter tbl = new GR_CfgRouteBOMTableAdapter();
                tbl.Connection.ConnectionString = connectionString;

                tbl.Insert(
                    route.CO_Num,
                    route.CO_Line,
                    route.Seq,
                    route.Type,
                    route.BOM_ID,
                    route.ItemNum,
                    route.SmartpartNum,
                    route.OPERATION,
                    route.WC,
                    route.Description,
                    (decimal?)route.Labor_Hours,
                    (decimal?)route.Setup_Hours,
                    (decimal?)route.Labor_Rate,
                    route.Notes,
                    route.Machine_Name,
                    (decimal?)route.Run_Time,
                    route.MatlItemNum,
                    route.MatlSmartpartNum,
                    route.MatlQty
                    );
            }
            catch (Exception RBError)
            {
                Triggers.logEvent = "ERROR WRITING GR_CfgRouteBOM: " + RBError.Message;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
            }
        }
        public static void ResequenceBOM(string orderNumber, int orderLine)
        {
            try
            {
                using (DbConnection database = new DbConnection())
                {
                    database.Command = new SqlCommand("GR_Cfg_ResequenceBOMSp", database.Connection);
                    database.Command.CommandType = CommandType.StoredProcedure;
                    database.Command.Parameters.AddWithValue("@OrderNumber", orderNumber);
                    database.Command.Parameters.AddWithValue("@OrderLine", orderLine);
                    database.Command.CommandTimeout = 120000;

                    database.Connection.Open();
                    database.Command.ExecuteNonQuery();
                    database.Connection.Close();
                }
            }
            catch (Exception bomreseq)
            {
                string logEvent = "An error resequence-BOM occurred: " + bomreseq.Message;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
            }
        }
        public static void CfgImport(string orderNumber)
        {
            //call SP to import data into Syteline
            int CreateOrder = 1;
            using (DbConnection database = new DbConnection())
            {
                database.Command = new SqlCommand("GR_CfgImportSp", database.Connection);
                database.Command.CommandType = CommandType.StoredProcedure;
                database.Command.Parameters.AddWithValue("@pStartingOrderNum", orderNumber);
                database.Command.Parameters.AddWithValue("@pEndingOrderNum", orderNumber);
                database.Command.Parameters.AddWithValue("@pCreateOrder", CreateOrder);
                database.Command.CommandTimeout = 120000;
                database.Connection.Open();
                try
                {
                    database.Command.ExecuteNonQuery();
                }
                catch (Exception spI)
                {
                    //A timeout has likely occurred; try again
                    string logEvent = "An error executing GR_CfgImportSp has occurred: " + spI.Message + " -> Retrying...";
                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);

                    database.Connection.Close();
                    database.Connection.Open();
                    database.Command.ExecuteNonQuery();
                }
            }
        }
        public static void CleanupOrder(string orderNum)
        {
            using (DbConnection database = new DbConnection())
            {
                database.Command = new SqlCommand("DELETE from GR_CfgCO where order_num = @ordernum", database.Connection);
                database.Command.Parameters.AddWithValue("@ordernum", orderNum);
                database.Command.CommandTimeout = 120000;
                database.Connection.Open();
                database.Command.ExecuteNonQuery();

                database.Command = new SqlCommand("DELETE from GR_CfgCOItem where order_num = @ordernum", database.Connection);
                database.Command.Parameters.AddWithValue("@ordernum", orderNum);
                database.Command.ExecuteNonQuery();

                database.Command = new SqlCommand("DELETE from GR_CfgBOM where order_num = @ordernum", database.Connection);
                database.Command.Parameters.AddWithValue("@ordernum", orderNum);
                database.Command.ExecuteNonQuery();

                database.Command = new SqlCommand("DELETE from GR_CfgItem where order_num = @ordernum", database.Connection);
                database.Command.Parameters.AddWithValue("@ordernum", orderNum);
                database.Command.ExecuteNonQuery();

                database.Command = new SqlCommand("DELETE from GR_CfgParmVal where order_num = @ordernum", database.Connection);
                database.Command.Parameters.AddWithValue("@ordernum", orderNum);
                database.Command.ExecuteNonQuery();

                database.Command = new SqlCommand("DELETE from GR_CfgRouteBOM where order_num = @ordernum", database.Connection);
                database.Command.Parameters.AddWithValue("@ordernum", orderNum);
                database.Command.ExecuteNonQuery();
                database.Connection.Close();
            }
        }
        public static void WriteAuditRecord(string auditMessageL, string orderNum, int orderLine, string auditEvent)
        {
            try
            {
                using (DbConnection database = new DbConnection())
                {
                    SQLCommand = "Insert Into GR_Cfg_Audit (order_num,order_line_num,auditEvent,auditMessageL) values (@orderNum, @orderLine, @auditEvent, @auditMessageL)";
                    database.Command = new SqlCommand(SQLCommand, database.Connection);
                    database.Command.CommandTimeout = 120000;
                    SqlParameter order_num = database.Command.Parameters.AddWithValue("@orderNum", orderNum);
                    SqlParameter order_line = database.Command.Parameters.AddWithValue("@orderLine", orderLine);
                    SqlParameter audit_event = database.Command.Parameters.AddWithValue("@auditEvent", auditEvent);
                    SqlParameter audit_message = database.Command.Parameters.AddWithValue("@auditMessageL", auditMessageL);

                    database.Connection.Open();
                    database.Command.ExecuteNonQuery();
                }
            }
            catch (Exception auditex)
            {
                string logEvent = "Audit message error: " + auditex.Message;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
            }
        }
        public static string RetrieveSLCO(string orderNum)
        {
            string SLOrderNumber = "";

            using (DbConnection database = new DbConnection())
            {
                try
                {
                    SQLCommand = "Select TOP 1 co_num as SLCO From GR_CfgCO c with(nolock) Where c.order_num = @order_num";
                    database.Command = new SqlCommand(SQLCommand, database.Connection);
                    database.Command.CommandTimeout = 120000;
                    SqlParameter order_num = database.Command.Parameters.Add("@order_num", SqlDbType.NVarChar, 15);
                    order_num.Value = orderNum;
                    database.Connection.Open();

                    using (SqlDataReader reader = database.Command.ExecuteReader())
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
                }
                catch (Exception rCOex)
                {
                    Triggers.logEvent = "ERROR RETRIEVING SLCO: " + rCOex.Message;
                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
                }

                database.Connection.Close();
                return SLOrderNumber;
            }
        }
        public static string RetrieveISOCountry(string ISOCode)
        {
            string ISOCountry = "";

            SQLCommand = "Select IsNULL(country, ' ') as country from country with (nolock) Where iso_country_code = @isoCode";
            using (DbConnection database = new DbConnection())
            {
                database.Command = new SqlCommand(SQLCommand, database.Connection);
                database.Command.CommandTimeout = 120000;
                SqlParameter iso_code = database.Command.Parameters.AddWithValue("@isoCode", ISOCode);
                database.Connection.Open();

                using (SqlDataReader reader = database.Command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            ISOCountry = reader["country"].ToString();
                        }
                    }
                    reader.Close();
                }
                database.Connection.Close();
            }

            return ISOCountry;
        }
        public static void InsertDocumentRecord(XmlDocument xml, string orderNum, string environment, string site, string SLorderNumber)
        {
            string maintConnectionString = connectionString;
            connectionString = queueControlConnectionString;
            
            SQLCommand = "Insert Into GR_Cfg_DocumentQueue(order_num, environment, site, SLOrderNumber, orderXML) values(@orderNum, @environment, @site, @SLorderNumber, @xmldata)";
            using (DbConnection database = new DbConnection())
            {
                database.Command = new SqlCommand(SQLCommand, database.Connection);
                database.Command.Parameters.AddWithValue("@orderNum", orderNum);
                database.Command.Parameters.AddWithValue("@environment", environment);
                database.Command.Parameters.AddWithValue("@site", site);
                database.Command.Parameters.AddWithValue("@SlorderNumber", SLorderNumber);
                database.Command.Parameters.Add(new SqlParameter("@xmldata", System.Data.SqlDbType.Xml) { Value = new SqlXml(new XmlTextReader(xml.InnerXml, XmlNodeType.Document, null)) });
                database.Command.CommandTimeout = 120000;
                database.Connection.Open();

                database.Command.ExecuteNonQuery();
                database.Connection.Close();
            }

            //to force the abstract class to reload our primary connnection string
            connectionString = maintConnectionString;
            using (DbConnection database = new DbConnection())
            {
                database.Connection.Open();
                database.Connection.Close();
            }
        }
        public static bool OrderCompleted(string order_num)
        {
            bool rowExists = false;

            SQLCommand = "Select order_num, co_num, stat from GR_CfgCO with(nolock) where order_num = @order_num and stat = 'C' and co_num is not null";
            using (DbConnection database = new DbConnection())
            {
                database.Command = new SqlCommand(SQLCommand, database.Connection);
                database.Command.CommandTimeout = 120000;
                SqlParameter ordernum = database.Command.Parameters.AddWithValue("@order_num", order_num);
                database.Connection.Open();
                using (SqlDataReader reader = database.Command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        rowExists = true;
                    }
                    reader.Close();
                }
                database.Connection.Close();
            }

            return rowExists;
        }
        public static int CoLines(string SLOrderNumber)
        {
            int coitems = 0;

            SQLCommand = "Select ISNULL(COUNT(co_line), 0) as lines From COITEM with(nolock) where co_num = @orderNumber";
            using (DbConnection database = new DbConnection())
            {
                database.Command = new SqlCommand(SQLCommand, database.Connection);
                database.Command.CommandTimeout = 120000;
                SqlParameter ordernum = database.Command.Parameters.AddWithValue("@orderNumber", SLOrderNumber);
                database.Connection.Open();
                using (SqlDataReader reader = database.Command.ExecuteReader())
                {
                    reader.Read();
                    coitems = Convert.ToInt16(reader["lines"].ToString());
                    reader.Close();
                }
                database.Connection.Close();
            }
            return coitems;
        }
        public static void ExecutePreCache()
        {
            //Execute Pre-caching of the SP's to force an on-demand recompile while we are preparing
            int CreateOrder = 1;

            using (DbConnection database = new DbConnection())
            {
                database.Command = new SqlCommand("GR_CfgImportSp", database.Connection);
                database.Command.CommandType = CommandType.StoredProcedure;
                database.Command.Parameters.AddWithValue("@pStartingOrderNum", "PRECACHE");
                database.Command.Parameters.AddWithValue("@pEndingOrderNum", "PRECACHE");
                database.Command.Parameters.AddWithValue("@pCreateOrder", CreateOrder);
                database.Command.CommandTimeout = 120000;

                database.Connection.Open();
                database.Command.ExecuteNonQuery();
                database.Connection.Close();
            }
        }
        public static bool OrderExists(string C1orderNum)
        {
            bool orderExists = false;

            SQLCommand = "Select TOP 1 co_num as SLCO From CO c with (nolock) Where c.uf_weborder = @C1OrderNum Order By c.order_date desc, c.co_num desc";
            using (DbConnection database = new DbConnection())
            {
                database.Command = new SqlCommand(SQLCommand, database.Connection);
                database.Command.CommandTimeout = 120000;
                SqlParameter c1ordernum = database.Command.Parameters.AddWithValue("@C1OrderNum", C1orderNum);
                database.Connection.Open();
                using (SqlDataReader reader = database.Command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            orderExists = true;
                        }
                    }
                    reader.Close();
                }
                database.Connection.Close();
            }
            return orderExists;
        }
        public static string CustomerOnHold(string custnum, string seq)
        {
            string holdReason = "";

            SQLCommand = "Select uf_coholddescription From customer cust with (nolock) Where ltrim(rtrim(cust.cust_num)) = @custnum and cust_seq = @seq and uf_cohold = 1";
            using (DbConnection database = new DbConnection())
            {
                database.Command = new SqlCommand(SQLCommand, database.Connection);
                database.Command.CommandTimeout = 120000;
                SqlParameter CustNumber = database.Command.Parameters.AddWithValue("@custnum", custnum);
                SqlParameter CustSeq = database.Command.Parameters.AddWithValue("@seq", seq);
                database.Connection.Open();
                using (SqlDataReader reader = database.Command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            holdReason = reader["uf_coholddescription"].ToString();
                            Triggers.logEvent = "Found an on-hold record match";
                            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                        }
                    }
                    reader.Close();
                }
                database.Connection.Close();
            }
            return holdReason;
        }
        public static void MoveQueueRecord(string orderNum, string dbSite)
        {
            try
            {
                //Disable trigger on target database/table, copy queue record, delete from NOVB
                SQLCommand = "DISABLE TRIGGER C1Order ON SL_" + dbSite + "_App.dbo.GR_Cfg_Queue";
                using (DbConnection database = new DbConnection())
                {
                    database.Command = new SqlCommand(SQLCommand, database.Connection);
                    database.Command.CommandTimeout = 120000;
                    database.Connection.Open();
                    database.Command.ExecuteNonQuery();
                    database.Connection.Close();
                }

                string recordRowPointer = "";
                SQLCommand = "SELECT TOP 1 rowpointer from SL_NOVB_App.dbo.GR_Cfg_Queue where order_num = @orderNum order by recorddate desc";
                using (DbConnection database = new DbConnection())
                {
                    database.Command = new SqlCommand(SQLCommand, database.Connection);
                    database.Command.CommandTimeout = 120000;
                    SqlParameter CustNumber = database.Command.Parameters.AddWithValue("@orderNum", orderNum);
                    database.Connection.Open();
                    using (SqlDataReader reader = database.Command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())              //this should never NOT happen (ok that's a little obtuse.  How about: This should ALWAYS happen)
                            {
                                //Copy this record to the target site DB
                                recordRowPointer = reader["rowpointer"].ToString();
                                reader.Close();

                                SQLCommand = "insert into SL_" + dbSite + "_App.dbo.GR_Cfg_Queue Select * from SL_NOVB_App.dbo.GR_Cfg_Queue where rowpointer = @recordrowpointer";
                                using (database.Connection)
                                {
                                    database.Command = new SqlCommand(SQLCommand, database.Connection);
                                    database.Command.CommandTimeout = 120000;
                                    SqlParameter RecRowPointer = database.Command.Parameters.AddWithValue("@recordrowpointer", recordRowPointer);
                                    database.Command.ExecuteNonQuery();

                                    SQLCommand = "DELETE From SL_NOVB_App.dbo.GR_Cfg_Queue where rowpointer = @recordrowpointer";
                                    database.Command = new SqlCommand(SQLCommand, database.Connection);
                                    database.Command.CommandTimeout = 120000;
                                    RecRowPointer = database.Command.Parameters.AddWithValue("@recordrowpointer", recordRowPointer);
                                    database.Command.ExecuteNonQuery();
                                    database.Connection.Close();
                                }
                            }
                        }
                    }
                }
                Triggers.logEvent = "Queue record was moved to SL_" + StagingUtilities.dbSite + "_App.dbo.GR_Cfg_Queue";
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            }
            catch (Exception mqEx)
            {
                //Log and ignore the error and move on with re-enabling the trigger
                string logEvent = "An error occurred while attempting to move the queue record to " + StagingUtilities.dbSite + ": " + mqEx.Message;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
            }

            //Re-enable trigger on db site queue table
            SQLCommand = "ENABLE TRIGGER C1Order ON SL_" + dbSite + "_App.dbo.GR_Cfg_Queue";
            using (DbConnection database = new DbConnection())
            {
                database.Command = new SqlCommand(SQLCommand, database.Connection);
                database.Command.CommandTimeout = 120000;
                database.Connection.Open();
                database.Command.ExecuteNonQuery();
                database.Connection.Close();
            }
        }
    }
}
