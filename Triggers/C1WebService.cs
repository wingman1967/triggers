using System;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;

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
            string result = "";

            try
            {
                //return response from AXIS (if any)
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
                return;
            }

            DatabaseFactory.CfgImport(Triggers.pubOrderNumber);                 //map staging-table data into Syteline
            
            //Retrieve the SL order# (if not found, default to using the C1 order#):
            string SPOrderNumber = string.IsNullOrEmpty(DatabaseFactory.RetrieveSLCO(Triggers.pubOrderNumber)) ? Triggers.pubOrderNumber : DatabaseFactory.RetrieveSLCO(Triggers.pubOrderNumber);
            SPPUBOrderNumber = SPOrderNumber;
            OutputXMLToFile(Triggers.wsReturn);

            try
            {
                logEvent = "Retrieving/Saving Document Files For Order: " + Triggers.pubOrderNumber + " -> " + SPOrderNumber;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                //Retrieve documents for order, reusing xmlResult above
                string documentType = "Design";
                string documentName = "";
                string orderNumber = Triggers.pubOrderNumber;
                string[] docs;
                docs = new string[50];
                int arrayindex = 0;
                
                //Retrieve the serialNumber from the XML 
                string configSerial = "";
                XmlNodeList xnlSN = xmlResult.GetElementsByTagName("serialNumber");
                foreach (XmlNode node in xnlSN)
                {
                    configSerial = node.InnerText;
                }

                string useMethod = "";
                key = "getDocument";
                useMethod = C1Dictionaries.webmethods[key];
                string documentSerialNumber = configSerial;
                arrayindex = 0;

                XmlNodeList xnl = xmlResult.GetElementsByTagName("fileName");
                foreach (XmlNode node in xnl)
                {
                    docs[arrayindex] = node.InnerText;
                    arrayindex += 1;
                }
                
                //We now have an array of document filenames; process by retrieving and saving each file
                int upperBound = arrayindex;
                arrayindex = 0;
                string documentFilesSaved = "";
                documentSerialNumber = configSerial;
                string SharepointLocation = DatabaseFactory.splocation;
                NetworkShare.DisconnectFromShare(SharepointLocation, true);
                NetworkShare.ConnectToShare(SharepointLocation, DatabaseFactory.spuname, DatabaseFactory.sppassword);

                //Create Sharepoint folder for order#
                string SharepointCopyLocation = SharepointLocation + SPOrderNumber + "\\";
                try { DirectoryInfo di = Directory.CreateDirectory(SharepointCopyLocation); }
                catch (Exception edi)
                {
                    //directory already exists, nothing further needs done
                }
                
                while (arrayindex < upperBound)
                {
                    documentName = docs[arrayindex];
                    xmlPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soapenv:Envelope xmlns:xsi=" + (char)34 + "http://www.w3.org/2001/XMLSchema-instance" + (char)34 + " xmlns:xsd=" + (char)34 + "http://www.w3.org/2001/XMLSchema" + (char)34 + " xmlns:ws=" + (char)34 + "http://ws.configureone.com" + (char)34 + " xmlns:mod=" + (char)34 + "http://model.ws.configureone.com" + (char)34 + " xmlns:soapenv=" + (char)34 + "http://schemas.xmlsoap.org/soap/envelope/" + (char)34 + ">" + "<soapenv:Body><ws:" + key + " xmlns=" + (char)34 + "http://ws.configureone.com" + (char)34 + "><ws:document><mod:type>" + documentType + "</mod:type><mod:serialNumber>" + documentSerialNumber + "</mod:serialNumber><mod:files><mod:fileName>" + documentName + "</mod:fileName><mod:content>" + documentName + "</mod:content></mod:files></ws:document></ws:" + key + "></soapenv:Body></soapenv:Envelope>";
                    sURL = useMethod;
                    objRequest = (HttpWebRequest)WebRequest.Create(sURL.ToString());
                    objRequest.Method = "POST";
                    objRequest.ContentType = "text/xml";
                    objRequest.Headers.Add("SOAPAction", key);

                    data = new StringBuilder();
                    data.Append(xmlPayload);
                    byte[] byteDataDoc = Encoding.UTF8.GetBytes(data.ToString());          // Sending our request to Apache AXIS in a byte array
                    objRequest.ContentLength = byteDataDoc.Length;

                    using (Stream postStream = objRequest.GetRequestStream())
                    {
                        postStream.Write(byteDataDoc, 0, byteDataDoc.Length);
                    }
                    xmlResult = new XmlDocument();
                    result = "";
                    using (HttpWebResponse response = objRequest.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        result = reader.ReadToEnd();
                        reader.Close();

                        try
                        {
                            xmlResult.LoadXml(result);          
                        }
                        catch (Exception ex1)
                        {
                            return;
                        }
                        
                        XmlNodeList xnldoc = xmlResult.GetElementsByTagName("content");
                        foreach (XmlNode node in xnldoc)
                        {
                            byte[] pdfByteArray = Convert.FromBase64String(node.InnerText);
                            string fileToCopy = @"C:\C1TEMP\" + SPOrderNumber + "_" + docs[arrayindex];
                            File.WriteAllBytes(fileToCopy, pdfByteArray);
                            File.Copy(fileToCopy, SharepointCopyLocation + SPOrderNumber + "_" + docs[arrayindex], true);
                            documentFilesSaved = documentFilesSaved + docs[arrayindex] + Environment.NewLine;
                        }
                    }
                    arrayindex += 1;
                }

                //Save XML output to SharePoint as well
                File.Copy("XMLOutput_" + SPPUBOrderNumber + ".txt", SharepointCopyLocation + "XMLOutput_" + SPPUBOrderNumber + ".txt");

                logEvent = "Retrieved/Saved Following Document Files: " + Environment.NewLine + Environment.NewLine + documentFilesSaved;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                                
                //Update order-status and ref number in C1 to Ordered and SL order#, respectively
                key = "updateOrder";
                useMethod = C1Dictionaries.webmethods[key];
                string C1orderNumber = orderNumber;
                string C1orderReference = SPOrderNumber;
                string C1orderStatus = "OR";              //Ordered
                xmlPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=" + (char)34 + "http://www.w3.org/2001/XMLSchema-instance" + (char)34 + " xmlns:xsd=" + (char)34 + "http://www.w3.org/2001/XMLSchema" + (char)34 + " xmlns:soap=" + (char)34 + "http://schemas.xmlsoap.org/soap/envelope/" + (char)34 + ">" + "<soap:Body><" + key + " xmlns=" + (char)34 + "http://ws.configureone.com" + (char)34 + "><param1>" + orderNumber + "</param1><param2>" + C1orderReference + "</param2><param3>" + C1orderStatus + "</param3></" + key + "></soap:Body></soap:Envelope>";
                sURL = useMethod;
                pubURL = sURL;
                pubPayload = xmlPayload;
                logEvent = "XML Payload To AXIS: " + xmlPayload;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                objRequest = (HttpWebRequest)WebRequest.Create(sURL.ToString());
                objRequest.Method = "POST";
                objRequest.ContentType = "text/xml";
                objRequest.Headers.Add("SOAPAction", key);

                data = new StringBuilder();
                data.Append(xmlPayload);
                byte[] byteDataStatus = Encoding.UTF8.GetBytes(data.ToString());          // Sending our request to Apache AXIS in a byte array
                objRequest.ContentLength = byteDataStatus.Length;
                using (Stream postStream = objRequest.GetRequestStream())
                {
                    postStream.Write(byteDataStatus, 0, byteDataStatus.Length);
                }
                
                //return response from AXIS (if any)
                using (HttpWebResponse response = objRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    result = reader.ReadToEnd();
                    reader.Close();
                }
                
                try
                {
                    xmlResult.LoadXml(result);
                    xnl = xmlResult.GetElementsByTagName("success");
                    foreach (XmlNode node in xnl)
                    {
                        switch (node.InnerText)
                        {
                            case "true":
                                logEvent = "Status-Set Result: Success";
                                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                                break;
                            default:
                                logEvent = "Status-Set Result: Failure";
                                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
                                break;
                        }
                    }
                }
                catch (Exception ex2)
                {
                    logEvent = "ERROR LOADING XML FROM WEB SERVICE ON C1 ORDER STATUS SET: " + ex2.Message;
                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
                    return;
                }
                
                logEvent = "C1 Order Status And Reference Completed For C1 Order: " + orderNumber + " - Syteline Order: " + SPOrderNumber;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                //return;
            }
            catch (Exception exd)
            {
                logEvent = "ERROR: " + exd.Message + " -> " + exd.Source;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
            }
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
    }
}

