using System;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Processing for call to ConfigureOne web service and return for further handling
    /// </summary>
    class C1WebService
    {
        public static void CallConfigureOne(string key, string payload, string url)
        {
            string logEvent = "CALLING C1 WEBSERVICE";
            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            string sURL = url;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(sURL.ToString());
            objRequest.Method = "POST";
            objRequest.ContentType = "text/xml";
            objRequest.Headers.Add("SOAPAction", key);
            string xmlPayload = payload;
            StringBuilder data = new StringBuilder();
            data.Append(xmlPayload);
            byte[] byteData = Encoding.UTF8.GetBytes(data.ToString());          // Sending our request to Apache AXIS in a byte array
            objRequest.ContentLength = byteData.Length;
            
            using (Stream postStream = objRequest.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }
            XmlDocument xmlResult = new XmlDocument();
            try
            {
                //return response from AXIS (if any)
                string result = "";
                using (HttpWebResponse response = objRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    result = reader.ReadToEnd();
                    reader.Close();
                }
                try
                {
                    xmlResult.LoadXml(result);
                }
                catch (Exception ex2)
                {
                    logEvent = "ERROR LOADING XML FROM WEB SERVICE: " + ex2.Message;
                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
                    return;
                }
                if (Triggers.caller == "ORDER") {StagingUtilities.MapXMLToSQL(xmlResult);}

                Triggers.caller = "";
                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    xmlResult.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    string xmlOut = stringWriter.GetStringBuilder().ToString();
                    Triggers.wsReturn = System.Xml.Linq.XDocument.Parse(xmlOut).ToString();
                }
            }
            catch (WebException wex1)
            {
                logEvent = "ERROR RETURNED FROM C1 WEBSERVICE: " + wex1.Message + " : " + wex1.Response.ToString();
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
            }
        }
    }
}

