using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using ZMQ;
using ZMQ.ZMQExt;

namespace ConfigureOneFlag
{
    class RetrieveDrawings
    {
        public static string result;
        public static void CopyDrawings(XmlDocument xmlResult, string url)
        {
            //PLAN-B: For now, copy XML and order to GR_Cfg_DocumentQueue and send process-command through message queue to remote Windows service
            try
            {
                DatabaseFactory.InsertDocumentRecord(xmlResult, Triggers.pubOrderNumber, Triggers.dbEnvironment, StagingUtilities.dbSite, C1WebService.SPOrderNumber);
            }
            catch (System.Exception dbf1)
            {
                Triggers.logEvent = "ERROR QR: " + dbf1.Message;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            }
                        
            Triggers.logEvent = "Signal Message Queue to begin document processing for: " + Triggers.pubOrderNumber + " -> " + C1WebService.SPOrderNumber;
            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            
            //Send command to ZMQ listener to process newly created record from queue
            //setup
            using (ZMQ.Context context = new ZMQ.Context())
            using (ZMQ.Socket requester = context.Socket(ZMQ.SocketType.REQ))
            {
                //connect to remote endpoint, <server>  
                if (Triggers.dbEnvironment == "PROD")
                {
                    requester.Connect("tcp://192.168.23.19:5555");  //grc000dmzbus
                }
                else
                {
                    requester.Connect("tcp://172.16.1.60:5555");    //grctslsql0 (dev)
                }
                
                //send on the REQ pattern
                string requestText = "PROCESS:" + Triggers.pubOrderNumber;
                requester.Send(Encoding.ASCII.GetBytes(requestText.ToCharArray()));

                //receive Response on the REP pattern
                string ackMsg = requester.Recv(Encoding.ASCII);
                Triggers.logEvent = "Received from ZMQ Processing: " + ackMsg;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            }

            return;
            
            //code below is for when we do not need to split document processing

            string key;
            string payload;
            string logEvent;
            string xmlPayload;
            string sURL = url;
            StringBuilder data = new StringBuilder();
            
            //namespace manager
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlResult.NameTable);
            nsmgr.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");
            nsmgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");

            //dynamically elucidate namespace directly from XML 
            XmlNodeList nodeInfo = xmlResult.GetElementsByTagName("getOrderResponse");
            string xmlNamespace = "";
            xmlNamespace = Convert.ToString(nodeInfo[0].Attributes["xmlns"].Value);
            nsmgr.AddNamespace("c1", xmlNamespace);
            nsmgr.AddNamespace("c2", "http://mod.ws.configure.com");

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
                if (Triggers.dbEnvironment == "PROD" && DatabaseFactory.dbprotect != "YES") { key = "getDocumentPROD"; }

                useMethod = C1Dictionaries.webmethods[key];
                key = "getDocument";
                string documentSerialNumber = configSerial;
                arrayindex = 0;
                                
                XmlNodeList xnl = xmlResult.SelectNodes("//c1:files", nsmgr);
                foreach (XmlNode node in xnl)
                {
                    XmlNode nodeFile = node.SelectSingleNode("c1:fileName", nsmgr);
                    docs[arrayindex] = node.ChildNodes[0].InnerText;
                    arrayindex += 1;
                }

                logEvent = "DEBUG: Finished building document filename array";
                if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }

                //We now have an array of document filenames; process by retrieving and saving each file
                int upperBound = arrayindex;
                arrayindex = 0;
                string documentFilesSaved = "";
                documentSerialNumber = configSerial;

                string SharepointLocation = DatabaseFactory.splocation;

                if (Triggers.dbEnvironment == "PROD" && DatabaseFactory.dbprotect != "YES")
                {
                    SharepointLocation = @"\\ecm.gormanrupp.com\ecm\NPC\Oper\ConfOrd\" + StagingUtilities.dbSite + "\\";
                }

                NetworkShare.DisconnectFromShare(SharepointLocation, true);
                NetworkShare.ConnectToShare(SharepointLocation, DatabaseFactory.spuname, DatabaseFactory.sppassword);

                logEvent = "DEBUG: SP Share credential fixed.  UNAME: " + DatabaseFactory.spuname + " -> PWD: " + DatabaseFactory.sppassword;
                if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }

                string documentList = "";
                while (arrayindex < upperBound)
                {
                    logEvent = "File #" + (arrayindex + 1) + " : " + docs[arrayindex];
                    documentList += logEvent + Environment.NewLine;
                    
                    arrayindex += 1;
                }
                logEvent = "Files stored are: " + Environment.NewLine + Environment.NewLine + documentList;
                if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }

                arrayindex = 0;

                //Create Sharepoint BASE folder for order# (each line will have its own folder by item#)
                string SharepointCopyLocation = SharepointLocation + C1WebService.SPOrderNumber + "\\";
                try { DirectoryInfo di = Directory.CreateDirectory(SharepointCopyLocation); }
                catch (System.Exception edi)
                {
                    //directory already exists, nothing further needs done
                }
                
                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(sURL.ToString());
                objRequest.Method = "POST";
                objRequest.ContentType = "text/xml";
                objRequest.Headers.Add("SOAPAction", key);
                objRequest.Timeout = 120000;        
                objRequest.ReadWriteTimeout = 120000;
                objRequest.Credentials = new NetworkCredential(DatabaseFactory.ws_uname, DatabaseFactory.ws_password);

                while (arrayindex < upperBound)
                {
                    documentName = docs[arrayindex];
                    int strPos = documentName.IndexOf("_");
                    documentSerialNumber = documentName.Substring(0, strPos);

                    xmlPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soapenv:Envelope xmlns:xsi=" + (char)34 + "http://www.w3.org/2001/XMLSchema-instance" + (char)34 + " xmlns:xsd=" + (char)34 + "http://www.w3.org/2001/XMLSchema" + (char)34 + " xmlns:ws=" + (char)34 + "http://ws.configureone.com" + (char)34 + " xmlns:mod=" + (char)34 + "http://model.ws.configureone.com" + (char)34 + " xmlns:soapenv=" + (char)34 + "http://schemas.xmlsoap.org/soap/envelope/" + (char)34 + ">" + "<soapenv:Body><ws:" + key + " xmlns=" + (char)34 + "http://ws.configureone.com" + (char)34 + "><ws:document><mod:type>" + documentType + "</mod:type><mod:serialNumber>" + documentSerialNumber + "</mod:serialNumber><mod:files><mod:fileName>" + documentName + "</mod:fileName><mod:content>" + documentName + "</mod:content></mod:files></ws:document></ws:" + key + "></soapenv:Body></soapenv:Envelope>";
                    sURL = useMethod;
                    objRequest = (HttpWebRequest)WebRequest.Create(sURL.ToString());
                    objRequest.Method = "POST";
                    objRequest.ContentType = "text/xml";
                    objRequest.Headers.Add("SOAPAction", key);
                    objRequest.Timeout = 120000;
                    objRequest.ReadWriteTimeout = 120000;
                    objRequest.Credentials = new NetworkCredential(DatabaseFactory.ws_uname, DatabaseFactory.ws_password);

                    logEvent = "DEBUG: Requested document from C1: " + documentName + " Serial#: " + documentSerialNumber;
                    if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }

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
                    if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }

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
                            catch (System.Exception ex1)
                            {
                                return;
                            }

                            logEvent = "DEBUG: Response received from C1: " + result;
                            if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }
                            
                            // *** LOG TIME
                            endTime = DateTime.Now;
                            ts = endTime.Subtract(startTime);
                            elapsedTimeMS = ts.Milliseconds;
                            elapsedTimeSeconds = ts.Seconds;
                            logEvent = "DEBUG: Document response was received from ConfigureOne in: " + Convert.ToString(elapsedTimeMS) + "ms / " + Convert.ToString(elapsedTimeSeconds) + " s";
                            if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }
                            string copyToLocation = "";
                            bool alreadySent = false;
                            XmlNodeList xnldoc = xmlResult.SelectNodes("//c1:files", nsmgr);
                            foreach (XmlNode node in xnldoc)
                            {
                                logEvent = "DEBUG: Copying file to Sharepoint: " + C1WebService.SPOrderNumber + "_" + docs[arrayindex];
                                if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }

                                //Format Sharepoint directory, create if not exist, and copy document to that location (20 retries)
                                for (int r = 0; r < 20; r += 1)
                                {
                                    copyToLocation = DatabaseFactory.COItemitem(C1WebService.SPOrderNumber, documentSerialNumber);
                                    if (copyToLocation != "") { break; }
                                    System.Threading.Thread.Sleep(1000);
                                }
                                
                                //One last retry if we still did not retrieve the coitem item:
                                if (copyToLocation == "")
                                {
                                    copyToLocation = DatabaseFactory.COItemitem(C1WebService.SPOrderNumber, documentSerialNumber);
                                    if (copyToLocation == "")
                                    {
                                        logEvent = "Syteline Coitem Item Could Not Be Retrieved After 20 seconds.  The GR_ImportSp stored procedure may have timed-out or otherwise failed.  Documents Will Be Copied To The Root Of Order Folder For Order: " + C1WebService.SPOrderNumber;
                                        System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
                                        if (!alreadySent)
                                        {
                                            alreadySent = true;
                                            //possible TODO:  wait a little longer for the coitem to appear, though this can run into CLR timeout
                                        }
                                    }
                                }

                                //Create directory if not exists
                                try { DirectoryInfo di = Directory.CreateDirectory(SharepointCopyLocation + copyToLocation); }
                                catch (System.Exception edi)
                                {
                                    //directory already exists, nothing further needs done
                                }
                                
                                logEvent = "SharepointCopyLocation Value: " + SharepointCopyLocation;
                                if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }

                                logEvent = "CopyToLocation Value: " + copyToLocation;
                                if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }

                                logEvent = "Full-Path Value: " + SharepointCopyLocation + copyToLocation;
                                if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }

                                logEvent = "Full-Path with file Value: " + SharepointCopyLocation + copyToLocation + "\\" + docs[arrayindex];
                                if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }

                                startTime = DateTime.Now;
                                
                                try
                                {
                                    byte[] pdfByteArray = Convert.FromBase64String(node.ChildNodes[1].InnerText);
                                    File.WriteAllBytes(SharepointCopyLocation + copyToLocation + "\\" + docs[arrayindex], pdfByteArray);
                                    documentFilesSaved = documentFilesSaved + docs[arrayindex] + Environment.NewLine;

                                    // *** LOG TIME
                                    endTime = DateTime.Now;
                                    ts = endTime.Subtract(startTime);
                                    elapsedTimeMS = ts.Milliseconds;
                                    elapsedTimeSeconds = ts.Seconds;
                                    logEvent = "DEBUG: Document copied to Sharepoint in: " + Convert.ToString(elapsedTimeMS) + "ms / " + Convert.ToString(elapsedTimeSeconds) + " s";
                                    if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }
                                }
                                catch (System.Exception ts1)
                                {
                                    logEvent = "An error occurred copying file to Sharepoint: " + C1WebService.SPOrderNumber + "_" + docs[arrayindex] + "  >  " + "TS1 Exception ( " + ts1.Message + ").  A maximum of 5 retries will be attempted.";
                                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                                    bool successfulDocument = false;
                                    for (int i = 0; i < 5; i += 1)
                                    {
                                        if (successfulDocument) { break; }
                                        try
                                        {
                                            byte[] pdfByteArray = Convert.FromBase64String(node.ChildNodes[1].InnerText);
                                            File.WriteAllBytes(SharepointCopyLocation + C1WebService.SPOrderNumber + "_" + docs[arrayindex], pdfByteArray);
                                            documentFilesSaved = documentFilesSaved + docs[arrayindex] + Environment.NewLine;

                                            // *** LOG TIME
                                            endTime = DateTime.Now;
                                            ts = endTime.Subtract(startTime);
                                            elapsedTimeMS = ts.Milliseconds;
                                            elapsedTimeSeconds = ts.Seconds;
                                            logEvent = "DEBUG: Document copied to Sharepoint in: " + Convert.ToString(elapsedTimeMS) + "ms / " + Convert.ToString(elapsedTimeSeconds) + " s";
                                            if (DatabaseFactory.debugLogging) { System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234); }
                                            successfulDocument = true;
                                        }
                                        catch (System.Exception rt1)
                                        {
                                            logEvent = "An error occurred copying file to Sharepoint: " + C1WebService.SPOrderNumber + "_" + docs[arrayindex] + "  >  " + "rt1 Exception (" + rt1.Message + ") on Retry#: " + (i + 1).ToString();
                                            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                                            System.Threading.Thread.Sleep(500);
                                        }
                                    }

                                    if (!successfulDocument)
                                    {
                                        // *** LOG TIME
                                        endTime = DateTime.Now;
                                        ts = endTime.Subtract(startTime);
                                        elapsedTimeMS = ts.Milliseconds;
                                        elapsedTimeSeconds = ts.Seconds;
                                        logEvent = "DEBUG: Time that elapsed before timeout occurred for current document: " + Convert.ToString(elapsedTimeMS) + "ms, " + Convert.ToString(elapsedTimeSeconds) + " s";
                                        System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception ts2)
                    {
                        logEvent = "An error occurred copying file to Sharepoint: " + C1WebService.SPOrderNumber + "_" + docs[arrayindex] + "  >  " + "TS2 Exception (" + ts2.Message + ")";
                        System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                        SendMail.MailMessage(logEvent, "ERROR");
                    }
                    arrayindex += 1;
                }
                
                //Save XML output to SharePoint as well
                File.Copy(@"C:\C1\XMLOUTPUTS\" + C1WebService.SPPUBOrderNumber + "_XMLOutput.txt", SharepointCopyLocation + C1WebService.SPPUBOrderNumber + "_XMLOutput.txt", true);

                //Housekeeping - delete the temporary XML output file
                File.Delete(@"C:\C1\XMLOUTPUTS\" + C1WebService.SPPUBOrderNumber + "_XMLOutput.txt");

                logEvent = "Retrieved/Saved Following Document Files: " + Environment.NewLine + Environment.NewLine + documentFilesSaved;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                
                //Update order-status and ref number in C1 to Ordered and SL order#, respectively
                key = "updateOrder";
                if (Triggers.dbEnvironment == "PROD" && DatabaseFactory.dbprotect != "YES") { key = "updateOrderPROD"; }

                useMethod = C1Dictionaries.webmethods[key];
                key = "updateOrder";
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
                objRequest.Credentials = new NetworkCredential(DatabaseFactory.ws_uname, DatabaseFactory.ws_password);
                
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
                catch (System.Exception ex2)
                {
                    logEvent = "ERROR LOADING XML FROM WEB SERVICE ON C1 ORDER STATUS SET: " + ex2.Message;
                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
                    return;
                }

                logEvent = "C1 Order Status And Reference Completed For C1 Order: " + orderNumber + " - Syteline Order: " + C1WebService.SPOrderNumber;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            }
            catch (System.Exception exd)
            {
                logEvent = "ERROR: " + exd.Message + " -> " + exd.Source;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
            }
        }
    }
}
