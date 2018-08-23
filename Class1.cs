using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using System.Data.SqlClient;

namespace ConfigureOneFlag
{
    public class Triggers
    {
        static Dictionary<string, string> webmethods = new Dictionary<string, string>();
        public static string wsReturn;
        public static string caller = "";
        public static string C1URL;
        [SqlTrigger(Name = "C1Order", Target = "GR_Cfg_Queue", Event = "FOR INSERT")]
        public static void C1Order()
        {
           try
            {
                webmethods.Add("getOrder", "http://nationaldev.conceptconfigurator.com/webservices/services/ConceptAccess?method=getOrder");
            }
            catch (Exception ex1)
            {
                //fallthru
            }
            string connectionString = "server = grcdslsql0.dom.grc; enlist=false; database = SL_MAN_App; User ID = sa_config; Password = options23";
            //using (SqlConnection conn = new SqlConnection("context connection=true"))

            string orderNum;
            string orderValue = "";
            SqlTriggerContext triggContext = SqlContext.TriggerContext;
            SqlParameter orderNumber = new SqlParameter("@order_num", System.Data.SqlDbType.NVarChar);



            if (triggContext.TriggerAction == TriggerAction.Insert)
            {
                using (SqlConnection conn = new SqlConnection("context connection=true"))
                {
                    conn.Open();
                    SqlCommand sqlComm = new SqlCommand();
                    SqlPipe sqlP = SqlContext.Pipe;

                    sqlComm.Connection = conn;
                    sqlComm.CommandText = "SELECT order_num from INSERTED";
                    orderNumber.Value = sqlComm.ExecuteScalar().ToString();
                    orderValue = orderNumber.Value.ToString();
                }
            }


            string useMethod = "";
            string key = "getOrder";

            if (webmethods.ContainsKey(key))
            {
                useMethod = webmethods[key];
                C1URL = useMethod;
            }

            caller = "ORDER";
            orderNum = orderValue;
            string xmlPayload = "<soap:Envelope xmlns:xsi=" + (char)34 + "http://www.w3.org/2001/XMLSchema-instance" + (char)34 + " xmlns:xsd=" + (char)34 + "http://www.w3.org/2001/XMLSchema" + (char)34 + " xmlns:soap=" + (char)34 + "http://schemas.xmlsoap.org/soap/envelope/" + (char)34 + ">" + "<soap:Body><" + key + " xmlns=" + (char)34 + "http://ws.configureone.com" + (char)34 + "><orderNum>" + orderNum + "</orderNum></" + key + "></soap:Body></soap:Envelope>";
            C1WebService.CallConfigureOne(key, xmlPayload, C1URL);
        }
    }
}
