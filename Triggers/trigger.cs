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
        [SqlTrigger(Name = "C1Order", Target = "GR_Cfg_Queue", Event = "FOR INSERT")]
        public static void C1Order()
        {
            if (!System.Diagnostics.EventLog.SourceExists("C1ORDER"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "C1ORDER", "Application");
            }
                                    
            logEvent = "TRIGGER FIRING ON INSERT";
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
            SqlTriggerContext triggContext = SqlContext.TriggerContext;
            SqlParameter orderNumber = new SqlParameter("@order_num", System.Data.SqlDbType.NVarChar);
            
            switch (triggContext.TriggerAction == TriggerAction.Insert)
            {
                case true:
                    using (SqlConnection conn = new SqlConnection("context connection=true"))
                    {
                        conn.Open();
                        SqlCommand sqlComm = new SqlCommand();
                        SqlPipe sqlP = SqlContext.Pipe;
                        sqlComm.Connection = conn;
                        sqlComm.CommandText = "SELECT order_num from INSERTED";
                        orderNumber.Value = sqlComm.ExecuteScalar().ToString();
                        orderValue = orderNumber.Value.ToString();
                        logEvent = "ORDER NUMBER: " + orderValue;
                        System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                    }
                    break;
                default:
                    return;
            }

            string useMethod = "";
            string key = "getOrder";

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
            string xmlPayload = "<soap:Envelope xmlns:xsi=" + (char)34 + "http://www.w3.org/2001/XMLSchema-instance" + (char)34 + " xmlns:xsd=" + (char)34 + "http://www.w3.org/2001/XMLSchema" + (char)34 + " xmlns:soap=" + (char)34 + "http://schemas.xmlsoap.org/soap/envelope/" + (char)34 + ">" + "<soap:Body><" + key + " xmlns=" + (char)34 + "http://ws.configureone.com" + (char)34 + "><orderNum>" + orderNum + "</orderNum></" + key + "></soap:Body></soap:Envelope>";
            C1WebService.CallConfigureOne(key, xmlPayload, C1URL);
            DatabaseFactory.CfgImport(orderNum);
            logEvent = "PROCESSING COMPLETE FOR ORDER: " + orderValue;
            System.Diagnostics.EventLog.WriteEntry(logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            Audit.ProcessingCompleted(logEvent);
        }
    }
}
