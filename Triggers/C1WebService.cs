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

            try
            {
                logEvent = "Retrieving/Saving Document Files For Order: " + Triggers.pubOrderNumber;
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

                string useMethod = "http://nationaldev.conceptconfigurator.com/webservices/services/ConceptAccess?method=getDocument";
                key = "getDocument";
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
                documentSerialNumber = configSerial;
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
                            File.WriteAllBytes(@"C:\JH\" + orderNumber + "_" + docs[arrayindex], pdfByteArray);
                            string SharepointLocation = DatabaseFactory.splocation;
                            string fileToCopy = @"C:\JH\" + orderNumber + "_" + docs[arrayindex];
                            NetworkShare.DisconnectFromShare(SharepointLocation, true);
                            NetworkShare.ConnectToShare(SharepointLocation, "hinesj", "$1Spring2017$");
                            File.Copy(fileToCopy, SharepointLocation + orderNumber + "_" + docs[arrayindex], true);
                        }
                    }
                    arrayindex += 1;
                }
                string documentFilesSaved = "";
                arrayindex = 0;
                while (arrayindex < upperBound)
                {
                    documentFilesSaved = documentFilesSaved + docs[arrayindex] + Environment.NewLine;
                    arrayindex += 1;
                }

                logEvent = "Retrieved/Saved Following Document Files: " + Environment.NewLine + Environment.NewLine + documentFilesSaved;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                
                return;
            }
            catch (Exception exd)
            {
                logEvent = "ERROR: " + exd.Message + " -> " + exd.Source;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
            }
        }
    }
}

