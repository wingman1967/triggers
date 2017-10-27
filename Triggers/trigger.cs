using Microsoft.SqlServer.Server;
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
        [SqlTrigger(Name = "C1Order", Target = "GR_Cfg_Queue", Event = "AFTER INSERT")]
        public static void C1Order()
        {
            if (!System.Diagnostics.EventLog.SourceExists("C1ORDER"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "C1ORDER", "Application");
            }
                                    
            logEvent = "TRIGGER FIRING ON INSERT EVENT - GR_CFG_QUEUE";
            System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

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
            string orderValue = "";
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
                        logEvent = "ORDER NUMBER: " + orderValue;
                        System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                    }
                    break;
                default:
                    return;
            }
            
            //Determine whether incoming order is production or dev/test
            string incomingOrder = orderValue.Substring(0, 1);
            dbEnvironment = "DEV";
            if (incomingOrder != "D")
            {
                dbEnvironment = "PROD";
            }

            string useMethod = "";
            string key = "getOrder";
            if (Triggers.dbEnvironment == "PROD" && DatabaseFactory.dbprotect != "YES") { key = "getOrderPROD"; }

            //force prod
            key = "getOrder";

            if (C1Dictionaries.webmethods.ContainsKey(key))
            {
                useMethod = C1Dictionaries.webmethods[key];
                C1URL = useMethod;
            }
            
            DatabaseFactory dbf = new DatabaseFactory();
            dbf.SetConnectionString();
            caller = "ORDER";
            orderNum = orderValue;
            pubOrderNumber = orderNum;
                        
            logEvent = "Connection String For Session: " + DatabaseFactory.connectionString;
            System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            
            //Ensure the mE array is initialized, in case it is never addressed again before an array.clear is attempted
            Audit.mE = new string[50];
            Audit.mEIndex = 0;
            string xmlPayload = "<soap:Envelope xmlns:xsi=" + (char)34 + "http://www.w3.org/2001/XMLSchema-instance" + (char)34 + " xmlns:xsd=" + (char)34 + "http://www.w3.org/2001/XMLSchema" + (char)34 + " xmlns:soap=" + (char)34 + "http://schemas.xmlsoap.org/soap/envelope/" + (char)34 + ">" + "<soap:Body><" + key + " xmlns=" + (char)34 + "http://ws.configureone.com" + (char)34 + "><orderNum>" + orderNum + "</orderNum></" + key + "></soap:Body></soap:Envelope>";
            C1WebService.CallConfigureOne(key, xmlPayload, C1URL);
            logEvent = "PROCESSING COMPLETE FOR ORDER: " + orderValue;
            System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            Audit.ProcessingCompleted(logEvent);
        }
    }
}
