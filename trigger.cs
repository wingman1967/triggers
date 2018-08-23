using Microsoft.SqlServer.Server;
using System;
using System.Data.SqlClient;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Primary entrypoint called from C1ORDER trigger for accessing C1 web service and returning/mapping data to SQL where appropriate
    /// </summary>
    public class Triggers
    {
        public static string wsReturn;
        public static string caller = "";
        public static string C1URL;
        public static string logSource = "C1ORDER";
        public static string logEvent;
        public static string logName = "Application";
        public static string pubOrderNumber = "";
        public static string dbEnvironment = "";
        public static string key = "getOrder";
        public static string xmlPayload = "";
        public static string orderValue = "";
        public static DateTime AsOf = DateTime.Now;
        public static int itr = 0;
        public static int forceStop = 0;
        [SqlTrigger(Name = "C1Order", Target = "GR_Cfg_Queue", Event = "AFTER INSERT")]
        public static void C1Order()
        {
            if (!System.Diagnostics.EventLog.SourceExists("C1ORDER"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "C1ORDER", "Application");
            }
                                    
            logEvent = "TRIGGER - GR_CFG_QUEUE : INCOMING ORDER";
            if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }

            switch (C1Dictionaries.webmethods.ContainsKey("getOrder"))
            {
                case false:
                    C1Dictionaries.LoadWMDictionary();
                    C1Dictionaries.LoadDBFLengthDictionary();
                    break;
                default:
                    //dictionary already loaded
                    break;
            }
           
            string orderNum;
            orderValue = "";
            string qRowPointerValue = "";
            SqlTriggerContext triggContext = SqlContext.TriggerContext;
            SqlParameter orderNumber = new SqlParameter("@order_num", System.Data.SqlDbType.NVarChar);
            SqlParameter qRowPointer = new SqlParameter("@RowPointer", System.Data.SqlDbType.NVarChar);

            switch (triggContext.TriggerAction == TriggerAction.Insert)
            {
                case true:
                    using (SqlConnection conn = new SqlConnection("context connection=true"))
                    {
                        conn.Open();
                        SqlCommand sqlComm = new SqlCommand();
                        sqlComm.CommandTimeout = 1800;
                        SqlPipe sqlP = SqlContext.Pipe;
                        sqlComm.Connection = conn;
                        sqlComm.CommandText = "SELECT order_num, RowPointer from INSERTED";
                        orderNumber.Value = sqlComm.ExecuteScalar().ToString();
                        orderValue = orderNumber.Value.ToString();

                        sqlComm.CommandText = "SELECT RowPointer from INSERTED";
                        qRowPointer.Value = sqlComm.ExecuteScalar().ToString();
                        qRowPointerValue = qRowPointer.Value.ToString();
                         
                        conn.Close();
                        logEvent = "INCOMING C1 ORDER NUMBER: " + orderValue;
                        System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                    }
                    break;
                default:
                    return;
            }
            
            //Determine whether incoming order is production or dev/test
            string incomingOrder = orderValue.Substring(0, 1);
            dbEnvironment = "DEV";
            if (incomingOrder != "D") { dbEnvironment = "PROD"; }

            string useMethod = "";
            if (Triggers.dbEnvironment == "PROD" && DatabaseFactory.dbprotect != "YES") { key = "getOrderPROD"; }
            if (C1Dictionaries.webmethods.ContainsKey(key))
            {
                useMethod = C1Dictionaries.webmethods[key];
                key = "getOrder";
                C1URL = useMethod;
            }
            
            DatabaseFactory dbf = new DatabaseFactory();
            dbf.SetConnectionString();
            caller = "ORDER";
            orderNum = orderValue;
            pubOrderNumber = orderNum;
                        
            logEvent = "Connection String (Initial): " + DatabaseFactory.connectionString;
            if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }
            
            //Check if this order has already been completely processed:
            if (DatabaseFactory.OrderCompleted(orderNum))
            {
                logEvent = "Order# " + orderNum + " has already been completed.  Further processing aborted";
                System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
                SendMail.MailMessage(logEvent, "Aborted Status-C Order");
                return;     //abort
            }
            
            //Ensure the mE array is initialized in case it is never addressed again before an array.clear is attempted
            Audit.mE = new string[50];
            Audit.mEIndex = 0;
            xmlPayload = "<soap:Envelope xmlns:xsi=" + (char)34 + "http://www.w3.org/2001/XMLSchema-instance" + (char)34 + " xmlns:xsd=" + (char)34 + "http://www.w3.org/2001/XMLSchema" + (char)34 + " xmlns:soap=" + (char)34 + "http://schemas.xmlsoap.org/soap/envelope/" + (char)34 + ">" + "<soap:Body><" + key + " xmlns=" + (char)34 + "http://ws.configureone.com" + (char)34 + "><orderNum>" + orderNum + "</orderNum></" + key + "></soap:Body></soap:Envelope>";

            AsOf = DateTime.Now;
            forceStop = 0;

            //Begin order-processing as an async task and allow trigger to reset
            Action ProcessXMLAsync = new Action(BeginProcessing);
            ProcessXMLAsync.BeginInvoke(new AsyncCallback(MTresult =>
            {
                (MTresult.AsyncState as Action).EndInvoke(MTresult);
            }), ProcessXMLAsync);

            //Hold trigger context open for up to 60 seconds waiting for order# so portal can display to user
            for (int r = 0; r < 60; r += 1)
            {
                if (DatabaseFactory.OrderExists(orderNum))
                {
                    logEvent = "SL Order found within " + itr.ToString() + " seconds";
                    System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                    break;
                }

                System.Threading.Thread.Sleep(1000);
                itr = r + 1;
            }
            
            if (forceStop == 1) { return; }
            
            logEvent = "Trigger context decoupled/resetting... (" + orderNum + ")";
            System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

            try
            {
                System.Threading.Thread.Sleep(10000);
            }
            catch (Exception tt1)
            {
                //disregard, as any error would likely be a thread-abort by the trigger
            }
        }
        public static void BeginProcessing()
        {
            C1WebService.CallConfigureOne(key, xmlPayload, C1URL);
            logEvent = "PROCESSING COMPLETE FOR ORDER: " + orderValue;
            System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            //Audit.ProcessingCompleted(logEvent);
        }
    }
}
