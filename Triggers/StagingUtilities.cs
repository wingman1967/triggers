using System;
using System.Xml;
using System.Web;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Utilities used for staging of data from C1 to SQL databases
    /// </summary>
    class StagingUtilities
    {
        public static string globalOrderNum;
        public static int globalOrderLineNum;
        public static string dbSite = "";
        public static bool foundSite = false;
                
        public static void MapXMLToSQL(XmlDocument xmldoc)
        {
            zCfgCO co = new zCfgCO();
            zCfgCOitem coitem = new zCfgCOitem();
            zCfgItem citem = new zCfgItem();
            zCfgParmVal cfg = new zCfgParmVal();
            zCfgBOM bom = new zCfgBOM();

            Audit.resetmE = true;       //reset the mE array in case we have any mapping errors to report for this cycle

            var nsmgr = new XmlNamespaceManager(xmldoc.NameTable);
            nsmgr.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");
            nsmgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("c1", "http://ws.configureone.com");

            //*** Determine what site we are working with and re-set the connection string accordingly, else default to NOVB and abort
            foundSite = true;
            XmlNodeList xnlsite = xmldoc.GetElementsByTagName("Input");
            foreach (XmlNode node in xnlsite)
            {
                XmlNode nodeSite = node.SelectSingleNode("//c1:Input[@name='ORDER_SITE']", nsmgr);
                dbSite = nodeSite.ChildNodes[0].Attributes["name"].InnerXml;
                if (dbSite == null) { foundSite = false; }
                
                switch (foundSite == true)
                {
                    case true:
                        string rplConnectionString = DatabaseFactory.connectionString;
                        int csPos = rplConnectionString.IndexOf("NOVB");
                        DatabaseFactory.connectionString = rplConnectionString.Substring(0, csPos) + dbSite + rplConnectionString.Substring(csPos + 4, rplConnectionString.Length - (csPos + 4));
                        Triggers.logEvent = "Connection String: " + DatabaseFactory.connectionString;
                        //System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                        break;
                    default:
                        //prepare to log the no-site and abort
                        break;
                }
            }

            if (!foundSite)
            {
                Triggers.logEvent = "ORDER_SITE was not found in XML for order# " + Triggers.pubOrderNumber + ".  Processing aborted.";
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
                SendMail.MailMessage(Triggers.logEvent, "MISSING ORDER_SITE For Order: " + Triggers.pubOrderNumber);
                return;
            }

            Triggers.logEvent = "Execute SP Pre-cache on site: " + dbSite;
            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            DatabaseFactory.ExecutePreCache();

            //Pre-staging activities
            XmlNodeList xnl = xmldoc.GetElementsByTagName("ORDER_NUM");
            foreach (XmlNode node in xnl)
            {
                co.CO_Num = node.InnerText;
                coitem.CO_Num = node.InnerText;
                cfg.CO_Num = node.InnerText;
                citem.CO_Num = node.InnerText;
                bom.CO_Num = node.InnerText;
                globalOrderNum = co.CO_Num;
                globalOrderLineNum = 0;
            }
            
            switch (co.CO_Num == null || co.CO_Num == "")
            {
                case true:
                    Triggers.logEvent = "ORDER NUMBER NOT FOUND: " + co.CO_Num;
                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                    return;
                default:
                    //Remove any pre-existing records in SQL for this order
                    DatabaseFactory.CleanupOrder(co.CO_Num);
                    break;
            }
            Triggers.logEvent = "MAPPING XML TO STAGING TABLES";
            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
            
            //build CO header
            xnl = xmldoc.GetElementsByTagName("ID");
            foreach (XmlNode node in xnl)
            {
                co.Identifier = node.InnerText;
                break;          //we want first occurrence, ONLY
            }
            xnl = xmldoc.GetElementsByTagName("ORDER_REF_NUM");
            foreach (XmlNode node in xnl)
            {
                co.CORefNum = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("CUST_NAME");
            foreach (XmlNode node in xnl)
            {
                co.CustName = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("CUST_REF_NUM");
            foreach (XmlNode node in xnl)
            {
                co.CustRefNum = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("ACCOUNT_NUM");
            foreach (XmlNode node in xnl)
            {
                co.AccountNum = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("ERP_REFERENCE_NUM");
            foreach (XmlNode node in xnl)
            {
                co.ErpReferenceNum = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("PROJECT");
            foreach (XmlNode node in xnl)
            {
                co.Project = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("PAYMENT_TERMS");
            foreach (XmlNode node in xnl)
            {
                co.PaymentTerms = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_VIA");
            foreach (XmlNode node in xnl)
            {
                co.ShipVia = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIPPING_TERMS");
            foreach (XmlNode node in xnl)
            {
                co.ShippingTerms = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_CONTACT_NAME");
            foreach (XmlNode node in xnl)
            {
                co.BillToContactName = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_ADDRESS_LINE_1");
            foreach (XmlNode node in xnl)
            {
                co.BillToAddressLine1 = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_ADDRESS_LINE_2");
            foreach (XmlNode node in xnl)
            {
                co.BillToAddressLine2 = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_ADDRESS_LINE_3");
            foreach (XmlNode node in xnl)
            {
                co.BillToAddressLine3 = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_CITY");
            foreach (XmlNode node in xnl)
            {
                co.BillToCity = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_STATE");
            foreach (XmlNode node in xnl)
            {
                co.BillToState = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_COUNTRY");
            foreach (XmlNode node in xnl)
            {
                co.BillToCountry = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_POSTAL_CODE");
            foreach (XmlNode node in xnl)
            {
                co.BillToPostalCode = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_PHONE_NUMBER");
            foreach (XmlNode node in xnl)
            {
                co.BillToPhoneNumber = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_FAX_NUMBER");
            foreach (XmlNode node in xnl)
            {
                co.BillToFaxNumber = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_EMAIL_ADDRESS");
            foreach (XmlNode node in xnl)
            {
                co.BillToEmailAddress = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            //xnl = xmldoc.GetElementsByTagName("BILL_TO_REF_NUM");
            xnl = xmldoc.GetElementsByTagName("BILL_TO_ERP_CONTACT_REF_NUM");
            foreach (XmlNode node in xnl)
            {
                co.BillToRefNum = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_CONTACT_NAME");
            foreach (XmlNode node in xnl)
            {
                co.ShipToContactName = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_ADDRESS_LINE_1");
            foreach (XmlNode node in xnl)
            {
                co.ShipToAddressLine1 = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_ADDRESS_LINE_2");
            foreach (XmlNode node in xnl)
            {
                co.ShipToAddressLine2 = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_ADDRESS_LINE_3");
            foreach (XmlNode node in xnl)
            {
                co.ShipToAddressLine3 = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_CITY");
            foreach (XmlNode node in xnl)
            {
                co.ShipToCity = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_STATE");
            foreach (XmlNode node in xnl)
            {
                co.ShipToState = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_COUNTRY");
            foreach (XmlNode node in xnl)
            {
                co.ShipToCountry = node.InnerText.Length == 0 ? " " : DatabaseFactory.RetrieveISOCountry(node.InnerText);
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_POSTAL_CODE");
            foreach (XmlNode node in xnl)
            {
                co.ShipToPostalCode = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_PHONE_NUMBER");
            foreach (XmlNode node in xnl)
            {
                co.ShipToPhoneNumber = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_FAX_NUMBER");
            foreach (XmlNode node in xnl)
            {
                co.ShipToFaxNumber = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_EMAIL_ADDRESS");
            foreach (XmlNode node in xnl)
            {
                co.ShipToEmailAddress = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            //xnl = xmldoc.GetElementsByTagName("SHIP_TO_REF_NUM");
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_ERP_CONTACT_REF_NUM");
            foreach (XmlNode node in xnl)
            {
                co.ShipToRefNum = node.InnerText;
            }

            //See if customer is on hold and if so, log and abort
            string custSeq = "";
            int sPos = co.ShipToRefNum.IndexOf("-");
            custSeq = co.ShipToRefNum.Substring(sPos + 1, (co.ShipToRefNum.Length - (sPos + 1)));
            string customerHoldReason = "";

            Triggers.logEvent = "Checking customer ON-HOLD status...";
            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

            try
            {
                customerHoldReason = DatabaseFactory.CustomerOnHold(co.CustRefNum, custSeq);
            }
            catch (Exception ex9)
            {
                Triggers.logEvent = "ERROR: " + ex9.Message + ".  Processing Aborted";
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                SendMail.MailMessage(Triggers.logEvent, "C1 Processing Aborted");
                Triggers.forceStop = 1;
                return;
            }
            
            if (customerHoldReason != "")
            {
                Triggers.forceStop = 1;
                Triggers.logEvent = "Processing Aborted.  Customer " + co.ShipToRefNum + " Is On Hold: " + customerHoldReason;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                SendMail.MailMessage(Triggers.logEvent, "C1 Processing Aborted");
                return;
            }

            xnl = xmldoc.GetElementsByTagName("PRIORITY_LEVEL");
            foreach (XmlNode node in xnl)
            {
                co.PriorityLevel = Convert.ToInt16(node.InnerText);
            }
            xnl = xmldoc.GetElementsByTagName("SERIAL_NUMBER");
            foreach (XmlNode node in xnl)
            {
                co.QuoteNbr = node.InnerText.Length == 0 ? " " : node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("CREATED_BY_USER_ID");
            foreach (XmlNode node in xnl)
            {
                //co.WebUserName = DatabaseFactory.UserName(node.InnerText);    //per Grant, 10/2017 pass-through the ID as-is
                co.WebUserName = node.InnerText;
            }
            
            co.WebOrderDate = System.DateTime.Now;

            //Look for PURCHASE ORDER in INPUTS, load into CO and COITEM
            XmlNode nodePO = xmldoc.SelectSingleNode("//c1:Input[@name='PURCHASE_ORDER']", nsmgr);
            co.CustPO = string.IsNullOrEmpty(nodePO.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodePO.ChildNodes[0].Attributes["name"].InnerXml;
            coitem.CustPO = nodePO.ChildNodes[0].Attributes["name"].InnerXml;

            //Look for FREIGHT ACCOUNT#, load into CO
            XmlNode nodeFA = xmldoc.SelectSingleNode("//c1:Input[@name='FREIGHT_ACCT']", nsmgr);
            co.FreightAcct = nodeFA.ChildNodes[0].Attributes["name"].InnerXml.Length == 0 ? " " : nodeFA.ChildNodes[0].Attributes["name"].InnerXml;
                        
            //Look for FREIGHT TERMS in INPUTS, load into CO
            XmlNode nodeFT = xmldoc.SelectSingleNode("//c1:Input[@name='FREIGHT_TERMS']", nsmgr);
            co.FreightTerms = nodeFT.ChildNodes[0].Attributes["name"].InnerXml.Length == 0 ? " " : nodeFT.ChildNodes[0].Attributes["name"].InnerXml;

            //Look for Order Header Notes, load into CO
            XmlNode nodeOHN = xmldoc.SelectSingleNode("//c1:Input[@name='ORDER_HEADER_NOTES']", nsmgr);
            co.OrderHeaderNotes = nodeOHN.ChildNodes[0].Attributes["name"].InnerXml.Length == 0 ? " " : nodeOHN.ChildNodes[0].Attributes["name"].InnerXml;

            //Retrieve due date
            co.DueDate = DateTime.Now;
            XmlNode nodedd = xmldoc.SelectSingleNode("//c1:Input[@name='DUE_DATE']", nsmgr);
            co.DueDate = nodedd.ChildNodes[0].Attributes["name"].InnerXml.Length == 0 ? DateTime.Now : Convert.ToDateTime(nodedd.ChildNodes[0].Attributes["name"].InnerXml);
            coitem.DueDate = co.DueDate;
                        
            //Look for REQUEST DATE in INPUTS, load into CO
            co.RequestDate = DateTime.Now;
            XmlNode noderd = xmldoc.SelectSingleNode("//c1:Input[@name='REQUEST_DATE']", nsmgr);
            co.RequestDate = noderd.ChildNodes[0].Attributes["name"].InnerXml.Length == 0 ? DateTime.Now : Convert.ToDateTime(noderd.ChildNodes[0].Attributes["name"].InnerXml);

            //Look for Destination_country in INPUTS, load into CO
            co.DestinationCountry = " ";
            XmlNode nodeDC = xmldoc.SelectSingleNode("//c1:Input[@name='DESTINATION_COUNTRY']", nsmgr);
            co.DestinationCountry = nodeDC.ChildNodes[0].Attributes["name"].InnerXml.Length == 0 ? " " : DatabaseFactory.RetrieveISOCountry(nodeDC.ChildNodes[0].Attributes["name"].InnerXml.Substring(0, 2));

            //Dropship data
            co.DropShipName = " ";
            co.DropShipAddress1 = " ";
            co.DropShipAddress2 = " ";
            co.DropShipAddress3 = " ";
            co.DropShipAddress4 = " ";
            co.DropShipCity = " ";
            co.DropShipState = " ";
            co.DropShipZip = " ";
            co.DropShipCountry = " ";
            co.DropShipContact = " ";
            co.DropShipPhone = " ";
            co.DropShipEmail = " ";
            XmlNodeList xnlds = xmldoc.GetElementsByTagName("Input");
            foreach (XmlNode nodeds in xnlds)
            {
                //UPDATE 6/16/2017:  Preserve the selected ship-to from C1 and if dropship has value, load up the dropship fields
                XmlNode nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_NAME']", nsmgr);
                co.DropShipName = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_ADDRESS_1']", nsmgr);
                co.DropShipAddress1 = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_ADDRESS_2']", nsmgr);
                co.DropShipAddress2 = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_ADDRESS_3']", nsmgr);
                co.DropShipAddress3 = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_ADDRESS_4']", nsmgr);
                co.DropShipAddress4 = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                
                //evaluate CITY and then CIty if the former fails, as some XML is loaded improperly with mixed-case for City
                try
                {
                    nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_CITY']", nsmgr);
                    co.DropShipCity = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                }
                catch (Exception dd1)
                {
                    try
                    {
                        nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_CIty']", nsmgr);
                        co.DropShipCity = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                    }
                    catch (Exception dd2)
                    {
                        co.DropShipCity = " ";
                    }
                }
                
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_STATE']", nsmgr);
                co.DropShipState = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_ZIP_CODE']", nsmgr);
                co.DropShipZip = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_COUNTRY']", nsmgr);
                co.DropShipCountry = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : DatabaseFactory.RetrieveISOCountry(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml);
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_CONTACT']", nsmgr);
                co.DropShipContact = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_PHONE']", nsmgr);
                co.DropShipPhone = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='DROP_SHIP_EMAIL']", nsmgr);
                co.DropShipEmail = string.IsNullOrEmpty(nodeDRS.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                break;
            }

            //Replace &amp; in any dropship fields with &
            co.DropShipName.Replace("&amp;", "&");
            co.DropShipAddress1.Replace("&amp;", "&");
            co.DropShipAddress2.Replace("&amp;", "&");
            co.DropShipAddress3.Replace("&amp;", "&");
            co.DropShipAddress4.Replace("&amp;", "&");
            co.DropShipContact.Replace("&amp;", "&");

            //build COITEM records, per line
            xnl = xmldoc.GetElementsByTagName("Detail");
            foreach (XmlNode node in xnl)
            {
                XmlNode nodertv = node.SelectSingleNode("c1:ORDER_LINE_NUM", nsmgr);
                coitem.CO_Line = Convert.ToInt16(nodertv.ChildNodes[0].InnerText);
                nodertv = node.SelectSingleNode("c1:SERIAL_NUM", nsmgr); 
                coitem.Serial = string.IsNullOrEmpty(nodertv.ChildNodes[0].InnerText) ? " " : nodertv.ChildNodes[0].InnerText;

                try
                {
                    nodertv = node.SelectSingleNode("c1:ITEM_NUM", nsmgr);
                    coitem.Item = string.IsNullOrEmpty(nodertv.ChildNodes[0].InnerText) ? " " : nodertv.ChildNodes[0].InnerText;
                    nodertv = node.SelectSingleNode("c1:SMARTPART_NUM", nsmgr);
                    coitem.Smartpart = string.IsNullOrEmpty(nodertv.ChildNodes[0].InnerText) ? " " : nodertv.ChildNodes[0].InnerText;
                }
                catch (Exception c11)
                {
                    coitem.Item = " ";
                    coitem.Smartpart = " ";
                }
                
                if (coitem.Item == "") { coitem.Item = " "; }
                if (coitem.Smartpart == "") { coitem.Smartpart = " "; }
                
                nodertv = node.SelectSingleNode("c1:DESCRIPTION", nsmgr);
                coitem.Desc = string.IsNullOrEmpty(nodertv.ChildNodes[0].InnerText) ? " " : nodertv.ChildNodes[0].InnerText;
                nodertv = node.SelectSingleNode("c1:TYPE", nsmgr);
                coitem.ConfigType = string.IsNullOrEmpty(nodertv.ChildNodes[0].InnerText) ? " " : nodertv.ChildNodes[0].Value;
                nodertv = node.SelectSingleNode("c1:UNIT_PRICE", nsmgr);
                coitem.UnitPrice = Convert.ToDecimal(nodertv.ChildNodes[0].InnerText);
                nodertv = node.SelectSingleNode("c1:UNIT_COST", nsmgr);
                coitem.UnitCost = Convert.ToDecimal(nodertv.ChildNodes[0].InnerText);
                nodertv = node.SelectSingleNode("c1:DISCOUNT_AMT", nsmgr);
                coitem.Discount = Convert.ToDecimal(nodertv.ChildNodes[0].InnerText);
                nodertv = node.SelectSingleNode("c1:QUANTITY", nsmgr);
                coitem.QTY = Convert.ToDecimal(nodertv.ChildNodes[0].InnerText);
                coitem.PriorityLevel = co.PriorityLevel;
                globalOrderLineNum = coitem.CO_Line;

                //Look for Line Notes, load into COItem
                XmlNode nodeLN = xmldoc.SelectSingleNode("//c1:Input[@name='LINE_NOTES']", nsmgr);
                coitem.OrderLineNotes = nodeLN.ChildNodes[0].Attributes["name"].InnerXml.Length == 0 ? " " : nodeLN.ChildNodes[0].Attributes["name"].InnerXml;

                if (coitem.ConfigType == "K")
                {
                    Triggers.logEvent = "WARNING: Config Type Is: " + coitem.ConfigType + " On C1 Order#: " + co.CO_Num + ". Ignoring line# " + coitem.CO_Line;
                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
                    SendMail.MailMessage(Triggers.logEvent, "Config-Type Warning");
                    continue;           //we cannot process a type K; notify, ignore this line and continue with the next detail node if any exist
                }
                //output coitem record
                DatabaseFactory.WriteRecordCOItem(ref coitem);

                //*** Everything else here builds on the COITEM ***
                var detailParent = node.SelectSingleNode(".");     //ensure we traverse ONLY children of this node (Detail) as the new parent (root) element
                XmlDocument detailDoc = new XmlDocument();
                detailDoc.LoadXml(detailParent.OuterXml);

                //iterate through Inputs for the line (none of these have static variable names or attributes)
                XmlNodeList xnli = detailDoc.GetElementsByTagName("Input");
                foreach (XmlNode nodei in xnli)
                {
                    cfg.CO_Line = coitem.CO_Line;
                    cfg.CName = nodei.ChildNodes[2].InnerText.Replace(" ", "_");
                    cfg.CValue = nodei.ChildNodes[0].InnerText;
                    cfg.CType = nodei.ChildNodes[1].InnerText;
                    cfg.CLabel = nodei.ChildNodes[2].InnerText;
                    cfg.PriorityLevel = co.PriorityLevel;
                    //output cfg (parmval) record
                    DatabaseFactory.WriteRecordCfg(ref cfg);
                }

                //Look for SHIP_VIA in INPUTS 
                co.ShipVia = " ";
                XmlNodeList xnlisv = xmldoc.GetElementsByTagName("Input");
                foreach (XmlNode nodeisv in xnlisv)
                {
                    if (nodeisv.ChildNodes[2].InnerText == "SHIP_VIA") { co.ShipVia = nodeisv.ChildNodes[0].Attributes["name"].Value; }
                }
                
                //item-master for the line we are working with
                int recordSequence = 1;
                XmlNodeList xnlim = detailDoc.GetElementsByTagName("ItemMaster");
                citem.CO_Line = coitem.CO_Line;
                string cost = "";
                string price = "";
                string sell = "";
                string weight = "";
                citem.IM_VAR1 = "";
                citem.IM_VAR2 = "";
                citem.IM_VAR3 = "";
                citem.IM_VAR4 = "";
                citem.IM_VAR5 = "";
                //Build nodelist of ItemMaster nodes and a second nodelist within of its childnodes so we can reference those elements by name instead of index
                foreach (XmlNode nodeim in xnlim)
                {
                    foreach (XmlNode childIM in nodeim.ChildNodes)
                    {
                        switch (childIM.Name)
                        {
                            case "SMARTPART_NUM":
                                citem.Smartpart = childIM.InnerText;
                                break;
                            case "ITEM_NUM":
                                citem.Item = childIM.InnerText;
                                break;
                            case "DESCRIPTION":
                                citem.Desc = childIM.InnerText;
                                break;
                            case "COST":
                                cost = childIM.InnerText;
                                break;
                            case "PRICE":
                                price = childIM.InnerText;
                                break;
                            case "SELL_PRICE":
                                sell = childIM.InnerText;
                                break;
                            case "WEIGHT":
                                weight = childIM.InnerText;
                                break;
                            case "UOM":
                                citem.UnitOfMeasure = childIM.InnerText;
                                break;
                            case "PRIORITY_LEVEL":
                                citem.PriorityLevel = Convert.ToInt16(childIM.InnerText);
                                break;
                            case "VAR_1":
                                citem.IM_VAR1 = string.IsNullOrEmpty(childIM.InnerText) ? " " : childIM.InnerText;
                                break;
                            case "VAR_2":
                                citem.IM_VAR2 = string.IsNullOrEmpty(childIM.InnerText) ? " " : childIM.InnerText;
                                break;
                            case "VAR_3":
                                citem.IM_VAR3 = string.IsNullOrEmpty(childIM.InnerText) ? " " : childIM.InnerText;
                                break;
                            case "VAR_4":
                                citem.IM_VAR4 = string.IsNullOrEmpty(childIM.InnerText) ? " " : childIM.InnerText;
                                break;
                            case "VAR_5":
                                citem.IM_VAR5 = string.IsNullOrEmpty(childIM.InnerText) ? " " : childIM.InnerText;
                                break;
                            default:
                                //do nothing
                                break;
                        }
                        citem.ItemCost = Convert.ToDecimal(string.IsNullOrEmpty(cost) ? "0" : cost);
                        citem.ItemPrice = Convert.ToDecimal(string.IsNullOrEmpty(price) ? "0" : price);
                        citem.ItemSellPrice = Convert.ToDecimal(string.IsNullOrEmpty(sell) ? "0" : sell);
                        citem.ItemWeight = Convert.ToDecimal(string.IsNullOrEmpty(weight) ? "0" : weight);
                        citem.Sequence = recordSequence;
                    }
                    recordSequence += 1;
                    DatabaseFactory.WriteRecordCItem(ref citem);
                    cost = "";
                    price = "";
                    sell = "";
                    weight = "";
                    citem.IM_VAR1 = "";
                    citem.IM_VAR2 = "";
                    citem.IM_VAR3 = "";
                    citem.IM_VAR4 = "";
                    citem.IM_VAR5 = "";
                }

                //BOM records (Must load in this manner, as none of these have static variable names or attributes)
                recordSequence = 0;
                XmlNodeList xnlb = detailDoc.GetElementsByTagName("Bom");
                bom.CO_Line = coitem.CO_Line;
                foreach (XmlNode nodeib in xnlb)
                {
                    recordSequence += 1;
                    bom.Sequence = recordSequence;
                    bom.RecordSequence = recordSequence;
                    var parent = nodeib.SelectSingleNode("..");
                    string parentID = parent.ChildNodes[0].InnerText;
                    bom.Parent = parentID == "1" ? null : parentID;
                    bom.Identifier = nodeib.ChildNodes[0].InnerText;
                    bom.Item = nodeib.ChildNodes[1].InnerText;
                    bom.Smartpart = nodeib.ChildNodes[2].InnerText;
                    bom.UnitPrice = Convert.ToDecimal(string.IsNullOrEmpty(nodeib.ChildNodes[3].InnerText) ? "0" : nodeib.ChildNodes[3].InnerText);
                    bom.UnitCost = Convert.ToDecimal(string.IsNullOrEmpty(nodeib.ChildNodes[4].InnerText) ? "0" : nodeib.ChildNodes[4].InnerText);
                    bom.Discount = Convert.ToDecimal(string.IsNullOrEmpty(nodeib.ChildNodes[5].InnerText) ? "0" : nodeib.ChildNodes[5].InnerText);
                    bom.QTY = Convert.ToDecimal(string.IsNullOrEmpty(nodeib.ChildNodes[6].InnerText) ? "0" : nodeib.ChildNodes[6].InnerText);
                    
                    //Search the ItemMaster XML records for the matching smartpart_num and retrieve its priority level for this BOM record
                    bool foundchild = false;
                    bool imdone = false;
                    foreach (XmlNode nodeIMPL in xnlim)
                    {
                        if (imdone == true)
                        {
                            break;
                        }
                        foreach (XmlNode childIMPL in nodeIMPL.ChildNodes)
                        {
                            if (imdone == true)
                            {
                                break;
                            }
                            if (childIMPL.InnerText == bom.Smartpart)
                            {
                                foundchild = true;
                            }
                            if (foundchild == true)
                            {
                                if (childIMPL.Name == "PRIORITY_LEVEL")
                                {
                                    bom.PriorityLevel = Convert.ToInt16(childIMPL.InnerText);
                                    imdone = true;
                                }
                            }
                        }
                    }
                    //output BOM record
                    DatabaseFactory.WriteRecordBOM(ref bom);
                }
                
                Triggers.logEvent = "RESEQUENCING BOM RECORDS FOR ORDER: " + bom.CO_Num + " LINE: " + bom.CO_Line;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                DatabaseFactory.ResequenceBOM(bom.CO_Num, bom.CO_Line);
            }
            DatabaseFactory.WriteRecordCO(ref co);              //deferred write
        }
    }
}

