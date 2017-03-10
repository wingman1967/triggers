using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.IO;

namespace ConfigureOneFlag
{
    class C1WebService
    {
        public static void CallConfigureOne(string key, string payload, string url)
        {
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
            // Return response from the ws and display in our textbox
            XmlDocument xmlResult = new XmlDocument();

            try
            {
                //return response from web service (if any)
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
                    //log in event viewer
                    return;
                }
                if (Triggers.caller == "ORDER")
                {
                    StagingUtilities.MapXMLToSQL(xmlResult);
                }

                Triggers.caller = "";

                //display well-formed and formatted XML in textbox and output same as flat-file
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
                //Triggers.wsReturn = "ERROR RETURNED BY APACHE AXIS WEB SERVICE: " + wex1.Response.ToString();
                //log in event viewer
            }
        }
    }
}

