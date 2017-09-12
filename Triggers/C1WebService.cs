using System;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Threading;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Processing for calls to ConfigureOne web service and return for further handling
    /// </summary>
    class C1WebService
    {
        public static string SPPUBOrderNumber;
        public static string pubPayload;
        public static string pubURL;
        public static string SPOrderNumber;
        public static XmlDocument xmlResultParm;
        public static string urlParm;
        public static void CallConfigureOne(string key, string payload, string url)
        {
            //Timing vars
            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now;
            TimeSpan ts = endTime.Subtract(startTime);
            decimal elapsedTimeMS = ts.Milliseconds;
            decimal elapsedTimeSeconds = ts.Seconds;
            DateTime totalTimeStart = DateTime.Now;
            DateTime totalTimeStop = DateTime.Now;

            string logEvent = "CALLING C1 WEBSERVICE";
            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            string sURL = url;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(sURL.ToString());
            objRequest.Method = "POST";
            objRequest.ContentType = "text/xml";
            objRequest.Headers.Add("SOAPAction", key);
            objRequest.Timeout = 120000;        //increased
            objRequest.ReadWriteTimeout = 120000;   //increased
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
            string result = "";

            try
            {
                startTime = DateTime.Now;

                //return response from AXIS (if any)
                using (HttpWebResponse response = objRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    result = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
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
                
                // *** LOG TIME
                endTime = DateTime.Now;
                ts = endTime.Subtract(startTime);
                elapsedTimeMS = ts.Milliseconds;
                elapsedTimeSeconds = ts.Seconds;
                logEvent = "DEBUG: XML Order data returned from ConfigureOne in: " + Convert.ToString(elapsedTimeMS) + "ms / " + Convert.ToString(elapsedTimeSeconds) + " s";
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                startTime = DateTime.Now;

                //Retrieve the SL order# (if not found, default to using the C1 order#):
                SPOrderNumber = string.IsNullOrEmpty(DatabaseFactory.RetrieveSLCO(Triggers.pubOrderNumber)) ? Triggers.pubOrderNumber : DatabaseFactory.RetrieveSLCO(Triggers.pubOrderNumber);
                SPPUBOrderNumber = SPOrderNumber;

                //Save XML output to object for further handling
                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    xmlResult.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    string xmlOut = stringWriter.GetStringBuilder().ToString();
                    Triggers.wsReturn = System.Xml.Linq.XDocument.Parse(xmlOut).ToString();
                }

                OutputXMLToFile(Triggers.wsReturn);             //so file will be there for the worker-thread

                if (Triggers.caller == "ORDER") {StagingUtilities.MapXMLToSQL(xmlResult);}

                // *** LOG TIME
                endTime = DateTime.Now;
                ts = endTime.Subtract(startTime);
                elapsedTimeMS = ts.Milliseconds;
                elapsedTimeSeconds = ts.Seconds;
                logEvent = "DEBUG: XML data mapped to staging tables in: " + Convert.ToString(elapsedTimeMS) + "ms / " + Convert.ToString(elapsedTimeSeconds) + " s";
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                Triggers.caller = "";
            }
            catch (WebException wex1)
            {
                logEvent = "ERROR RETURNED FROM C1 WEBSERVICE: " + wex1.Message + " : " + wex1.Response.ToString();
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
                return;
            }

            startTime = DateTime.Now;
            DatabaseFactory.CfgImport(Triggers.pubOrderNumber);                 //map staging-table data into Syteline

            //start downloading and copying drawing files on separate thread while mapping, import, and job-creation take place
            xmlResultParm = xmlResult;
            urlParm = url;
            Action DrawingsAsync = new Action(StartCopy);
            DrawingsAsync.BeginInvoke(new AsyncCallback(MTresult =>
            {
                (MTresult.AsyncState as Action).EndInvoke(MTresult);
            }), DrawingsAsync);

            // *** LOG TIME
            endTime = DateTime.Now;
            ts = endTime.Subtract(startTime);
            elapsedTimeMS = ts.Milliseconds;
            elapsedTimeSeconds = ts.Seconds;
            logEvent = "DEBUG: Staging tables mapped to Syteline in: " + Convert.ToString(elapsedTimeMS) + "ms / " + Convert.ToString(elapsedTimeSeconds) + " s";
            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

            // *** LOG TOTAL TIME
            totalTimeStop = DateTime.Now;
            ts = totalTimeStop.Subtract(totalTimeStart);
            elapsedTimeMS = ts.Milliseconds;
            elapsedTimeSeconds = ts.Seconds;
            logEvent = "DEBUG: Total execution: " + Convert.ToString(elapsedTimeMS) + "ms / " + Convert.ToString(elapsedTimeSeconds) + " s";
            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
        }
        private static void OutputXMLToFile(string XML)
        {
            string formattedXML = "";
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                string xmlOut = XML;
                formattedXML = System.Xml.Linq.XDocument.Parse(xmlOut).ToString();
            }

            if (File.Exists("XMLOutput_" + SPPUBOrderNumber + ".txt"))
            {
                File.Delete("XMLOutput_" + SPPUBOrderNumber + ".txt");
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"XMLOutput_" + SPPUBOrderNumber + ".txt", true))
            {
                file.WriteLine(formattedXML);
            }
        }
        private static void StartCopy()
        {
            RetrieveDrawings.CopyDrawings(xmlResultParm, urlParm);
        }
    }
}

