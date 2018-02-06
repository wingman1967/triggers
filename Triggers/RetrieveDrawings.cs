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
            //Copy XML and order to GR_Cfg_DocumentQueue and send process-command through message queue to remote Windows service
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
                Triggers.logEvent = "Message received from ZMQ: " + ackMsg;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            }
        }
    }
}
