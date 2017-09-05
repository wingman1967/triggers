using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Xml;
using System.IO;

namespace ConfigureOneFlag
{
    class RetrieveDrawings
    {
        public static string result;
        public static void CopyDrawings(XmlDocument xmlResult, string url)
        {
            string key;
            string payload;
            
            string logEvent;
            string xmlPayload;
            string sURL = url;

            StringBuilder data = new StringBuilder();

            //Timing vars
            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now;
            TimeSpan ts = endTime.Subtract(startTime);
            decimal elapsedTimeMS = ts.Milliseconds;
            decimal elapsedTimeSeconds = ts.Seconds;
            DateTime totalTimeStart = DateTime.Now;
            DateTime totalTimeStop = DateTime.Now;

            try
            {
                logEvent = "Retrieving/Saving Document Files For Order: " + Triggers.pubOrderNumber + " -> " + C1WebService.SPOrderNumber;
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

                logEvent = "DEBUG: Finished building document filename array";
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                //We now have an array of document filenames; process by retrieving and saving each file
                int upperBound = arrayindex;
                arrayindex = 0;
                string documentFilesSaved = "";
                documentSerialNumber = configSerial;
                string SharepointLocation = DatabaseFactory.splocation;
                NetworkShare.DisconnectFromShare(SharepointLocation, true);
                NetworkShare.ConnectToShare(SharepointLocation, DatabaseFactory.spuname, DatabaseFactory.sppassword);

                logEvent = "DEBUG: SP Share credential fixed";
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                //Create Sharepoint folder for order#
                string SharepointCopyLocation = SharepointLocation + C1WebService.SPOrderNumber + "\\";
                try { DirectoryInfo di = Directory.CreateDirectory(SharepointCopyLocation); }
                catch (Exception edi)
                {
                    //directory already exists, nothing further needs done
                }
                
                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(sURL.ToString());
                objRequest.Method = "POST";
                objRequest.ContentType = "text/xml";
                objRequest.Headers.Add("SOAPAction", key);
                objRequest.Timeout = 120000;        //increased
                objRequest.ReadWriteTimeout = 120000;   //increased

                while (arrayindex < upperBound)
                {
                    documentName = docs[arrayindex];
                    xmlPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soapenv:Envelope xmlns:xsi=" + (char)34 + "http://www.w3.org/2001/XMLSchema-instance" + (char)34 + " xmlns:xsd=" + (char)34 + "http://www.w3.org/2001/XMLSchema" + (char)34 + " xmlns:ws=" + (char)34 + "http://ws.configureone.com" + (char)34 + " xmlns:mod=" + (char)34 + "http://model.ws.configureone.com" + (char)34 + " xmlns:soapenv=" + (char)34 + "http://schemas.xmlsoap.org/soap/envelope/" + (char)34 + ">" + "<soapenv:Body><ws:" + key + " xmlns=" + (char)34 + "http://ws.configureone.com" + (char)34 + "><ws:document><mod:type>" + documentType + "</mod:type><mod:serialNumber>" + documentSerialNumber + "</mod:serialNumber><mod:files><mod:fileName>" + documentName + "</mod:fileName><mod:content>" + documentName + "</mod:content></mod:files></ws:document></ws:" + key + "></soapenv:Body></soapenv:Envelope>";
                    sURL = useMethod;
                    objRequest = (HttpWebRequest)WebRequest.Create(sURL.ToString());
                    objRequest.Method = "POST";
                    objRequest.ContentType = "text/xml";
                    objRequest.Headers.Add("SOAPAction", key);
                    objRequest.Timeout = 120000;
                    objRequest.ReadWriteTimeout = 120000;
                    //objRequest.KeepAlive = true;

                    logEvent = "DEBUG: Requested document from C1";
                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                    startTime = DateTime.Now;
                    data = new StringBuilder();

                    data = new StringBuilder();
                    data.Append(xmlPayload);
                    byte[] byteDataDoc = Encoding.UTF8.GetBytes(data.ToString());          // Sending our request to Apache AXIS in a byte array
                    objRequest.ContentLength = byteDataDoc.Length;

                    using (Stream postStream = objRequest.GetRequestStream())
                    {
                        postStream.ReadTimeout = 120000;
                        postStream.WriteTimeout = 120000;
                        postStream.Write(byteDataDoc, 0, byteDataDoc.Length);
                    }
                    xmlResult = new XmlDocument();
                    result = "";

                    logEvent = "DEBUG: Waiting for response from C1";
                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                    // using (HttpWebResponse response = objRequest.GetResponse() as HttpWebResponse)
                    try
                    {
                        using (HttpWebResponse response = (HttpWebResponse)objRequest.GetResponse())
                        {
                            StreamReader reader = new StreamReader(response.GetResponseStream());
                            result = reader.ReadToEnd();
                            reader.Close();
                            response.Close();

                            try
                            {
                                xmlResult.LoadXml(result);
                            }
                            catch (Exception ex1)
                            {
                                return;
                            }

                            logEvent = "DEBUG: Response received from C1";
                            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                            // *** LOG TIME
                            endTime = DateTime.Now;
                            ts = endTime.Subtract(startTime);
                            elapsedTimeMS = ts.Milliseconds;
                            elapsedTimeSeconds = ts.Seconds;
                            logEvent = "DEBUG: Document response was received from ConfigureOne in: " + Convert.ToString(elapsedTimeMS) + "ms / " + Convert.ToString(elapsedTimeSeconds) + " s";
                            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                            XmlNodeList xnldoc = xmlResult.GetElementsByTagName("content");
                            foreach (XmlNode node in xnldoc)
                            {
                                logEvent = "DEBUG: Copying file to Sharepoint: " + C1WebService.SPOrderNumber + "_" + docs[arrayindex];
                                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                                startTime = DateTime.Now;

                                try
                                {
                                    byte[] pdfByteArray = Convert.FromBase64String(node.InnerText);
                                    string fileToCopy = @"C:\C1TEMP\" + C1WebService.SPOrderNumber + "_" + docs[arrayindex];

                                    //deboog (comment out following line to NOT copy files to Sharepoint)
                                    File.WriteAllBytes(SharepointCopyLocation + C1WebService.SPOrderNumber + "_" + docs[arrayindex], pdfByteArray);
                                    //end deboog

                                    //File.WriteAllBytes(fileToCopy, pdfByteArray);
                                    //File.Copy(fileToCopy, SharepointCopyLocation + SPOrderNumber + "_" + docs[arrayindex], true);
                                    documentFilesSaved = documentFilesSaved + docs[arrayindex] + Environment.NewLine;

                                    // *** LOG TIME
                                    endTime = DateTime.Now;
                                    ts = endTime.Subtract(startTime);
                                    elapsedTimeMS = ts.Milliseconds;
                                    elapsedTimeSeconds = ts.Seconds;
                                    logEvent = "DEBUG: Document copied to Sharepoint in: " + Convert.ToString(elapsedTimeMS) + "ms / " + Convert.ToString(elapsedTimeSeconds) + " s";
                                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                                }
                                catch (Exception ts1)
                                {
                                    logEvent = "An error occurred copying file to Sharepoint: " + C1WebService.SPOrderNumber + "_" + docs[arrayindex] + "  >  " + "TS1 Exception";
                                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                                    // *** LOG TIME
                                    endTime = DateTime.Now;
                                    ts = endTime.Subtract(startTime);
                                    elapsedTimeMS = ts.Milliseconds;
                                    elapsedTimeSeconds = ts.Seconds;
                                    logEvent = "DEBUG: Time that elapsed before timeout occurred for current document: " + Convert.ToString(elapsedTimeMS) + "ms, " + Convert.ToString(elapsedTimeSeconds) + " s";
                                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                                    SendMail.MailMessage(logEvent, "TIMEOUT");
                                }
                            }
                        }
                    }
                    catch (Exception ts2)
                    {
                        logEvent = "An error occurred copying file to Sharepoint: " + C1WebService.SPOrderNumber + "_" + docs[arrayindex] + "  >  " + "TS2 Exception";
                        System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                        SendMail.MailMessage(logEvent, "TIMEOUT");
                    }
                    arrayindex += 1;
                }

                //Save XML output to SharePoint as well
                File.Copy("XMLOutput_" + C1WebService.SPPUBOrderNumber + ".txt", SharepointCopyLocation + "XMLOutput_" + C1WebService.SPPUBOrderNumber + ".txt");

                logEvent = "Retrieved/Saved Following Document Files: " + Environment.NewLine + Environment.NewLine + documentFilesSaved;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

                //Update order-status and ref number in C1 to Ordered and SL order#, respectively
                key = "updateOrder";
                useMethod = C1Dictionaries.webmethods[key];
                string C1orderNumber = orderNumber;
                string C1orderReference = C1WebService.SPOrderNumber;
                string C1orderStatus = "OR";              //Ordered
                xmlPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=" + (char)34 + "http://www.w3.org/2001/XMLSchema-instance" + (char)34 + " xmlns:xsd=" + (char)34 + "http://www.w3.org/2001/XMLSchema" + (char)34 + " xmlns:soap=" + (char)34 + "http://schemas.xmlsoap.org/soap/envelope/" + (char)34 + ">" + "<soap:Body><" + key + " xmlns=" + (char)34 + "http://ws.configureone.com" + (char)34 + "><param1>" + orderNumber + "</param1><param2>" + C1orderReference + "</param2><param3>" + C1orderStatus + "</param3></" + key + "></soap:Body></soap:Envelope>";
                sURL = useMethod;
                C1WebService.pubURL = sURL;
                C1WebService.pubPayload = xmlPayload;
                logEvent = "XML Payload To AXIS: " + xmlPayload;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                objRequest = (HttpWebRequest)WebRequest.Create(sURL.ToString());
                objRequest.Method = "POST";
                objRequest.ContentType = "text/xml";
                objRequest.Headers.Add("SOAPAction", key);
                objRequest.Timeout = 120000;
                objRequest.ReadWriteTimeout = 120000;

                data = new StringBuilder();
                data.Append(xmlPayload);
                byte[] byteDataStatus = Encoding.UTF8.GetBytes(data.ToString());          // Sending our request to Apache AXIS in a byte array
                objRequest.ContentLength = byteDataStatus.Length;
                using (Stream postStream = objRequest.GetRequestStream())
                {
                    postStream.ReadTimeout = 120000;
                    postStream.WriteTimeout = 120000;
                    postStream.Write(byteDataStatus, 0, byteDataStatus.Length);
                }

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

                logEvent = "C1 Order Status And Reference Completed For C1 Order: " + orderNumber + " - Syteline Order: " + C1WebService.SPOrderNumber;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            }
            catch (Exception exd)
            {
                logEvent = "ERROR: " + exd.Message + " -> " + exd.Source;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
            }
        }
    }
}
