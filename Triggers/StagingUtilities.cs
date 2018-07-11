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
            zCfgRoute route = new zCfgRoute();

            Audit.resetmE = true;       //reset the mE array in case we have any mapping errors to report for this cycle

            var nsmgr = new XmlNamespaceManager(xmldoc.NameTable);
            nsmgr.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");
            nsmgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("c1", "http://ws.configureone.com");
            
            //*** Determine what site we are working with and re-set the connection string accordingly, else default and abort
            foundSite = true;
            XmlNodeList xnlsite = xmldoc.GetElementsByTagName("Input");
            foreach (XmlNode node in xnlsite)
            {
                XmlNode nodeSite = node.SelectSingleNode("//c1:Input[@name='ORDER_SITE']", nsmgr);
                try
                {
                    dbSite = nodeSite.ChildNodes[0].Attributes["name"].InnerXml;
                }
                catch (Exception exSite)
                {
                    Triggers.logEvent = "ERROR occurred: " + exSite.Message;
                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
                }
                dbSite = nodeSite.ChildNodes[0].Attributes["name"].InnerXml;
                if (dbSite == null) { foundSite = false; }
                //foundSite &= dbSite != null;

                switch (foundSite == true)
                {
                    case true:
                        string rplConnectionString = DatabaseFactory.connectionString;
                        int csPos = rplConnectionString.IndexOf("SL_", StringComparison.CurrentCulture);
                        csPos += 3;
                        DatabaseFactory.connectionString = rplConnectionString.Substring(0, csPos) + dbSite + rplConnectionString.Substring(csPos + 4, rplConnectionString.Length - (csPos + 4));
                        break;
                    default:
                        //prepare to log the no-site and abort
                        break;
                }
            }

            Triggers.logEvent = "Site: " + dbSite;
            System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

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
            XmlNode xnode = xmldoc.SelectSingleNode("//c1:ORDER_NUM", nsmgr);
            co.CO_Num = xnode.InnerText;
            coitem.CO_Num = xnode.InnerText;
            cfg.CO_Num = xnode.InnerText;
            citem.CO_Num = xnode.InnerText;
            bom.CO_Num = xnode.InnerText;
            globalOrderNum = co.CO_Num;
            globalOrderLineNum = 0;
            co.WebUserName = "";
            co.WebOrderDate = System.DateTime.Now;
            
            switch (string.IsNullOrEmpty(co.CO_Num))
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
            co.Identifier = LoadFromXML(xmldoc, "//c1:ID", nsmgr);
            co.CORefNum = LoadFromXML(xmldoc, "//c1:ORDER_REF_NUM", nsmgr);
            co.CustName = LoadFromXML(xmldoc, "//c1:CUST_NAME", nsmgr);
            co.CustRefNum = LoadFromXML(xmldoc, "//c1:CUST_REF_NUM", nsmgr);
            co.AccountNum = LoadFromXML(xmldoc, "//c1:ACCOUNT_NUM", nsmgr);
            co.ErpReferenceNum = LoadFromXML(xmldoc, "//c1:ERP_REFERENCE_NUM", nsmgr);
            co.PaymentTerms = LoadFromXML(xmldoc, "//c1:PAYMENT_TERMS", nsmgr);
            co.ShipVia = LoadFromXML(xmldoc, "//c1:SHIP_VIA", nsmgr);
            co.ShippingTerms = LoadFromXML(xmldoc, "//c1:SHIPPING_TERMS", nsmgr);
            co.BillToContactName = LoadFromXML(xmldoc, "//c1:BILL_TO_CONTACT_NAME", nsmgr);
            co.BillToAddressLine1 = LoadFromXML(xmldoc, "//c1:BILL_TO_ADDRESS_LINE_1", nsmgr);
            co.BillToAddressLine2 = LoadFromXML(xmldoc, "//c1:BILL_TO_ADDRESS_LINE_2", nsmgr);
            co.BillToAddressLine3 = LoadFromXML(xmldoc, "//c1:BILL_TO_ADDRESS_LINE_3", nsmgr);
            co.BillToCity = LoadFromXML(xmldoc, "//c1:BILL_TO_CITY", nsmgr);
            co.BillToState = LoadFromXML(xmldoc, "//c1:BILL_TO_STATE", nsmgr);
            co.BillToCountry = LoadFromXML(xmldoc, "//c1:BILL_TO_COUNTRY", nsmgr);
            co.BillToPostalCode = LoadFromXML(xmldoc, "//c1:BILL_TO_POSTAL_CODE", nsmgr);
            co.BillToPhoneNumber = LoadFromXML(xmldoc, "//c1:BILL_TO_PHONE_NUMBER", nsmgr);
            co.BillToFaxNumber = LoadFromXML(xmldoc, "//c1:BILL_TO_FAX_NUMBER", nsmgr);
            co.BillToEmailAddress = LoadFromXML(xmldoc, "//c1:BILL_TO_EMAIL_ADDRESS", nsmgr);
            co.BillToRefNum = LoadFromXML(xmldoc, "//c1:BILL_TO_ERP_CONTACT_REF_NUM", nsmgr);
            co.ShipToContactName = LoadFromXML(xmldoc, "//c1:SHIP_TO_CONTACT_NAME", nsmgr);
            co.ShipToAddressLine1 = LoadFromXML(xmldoc, "//c1:SHIP_TO_ADDRESS_LINE_1", nsmgr);
            co.ShipToAddressLine2 = LoadFromXML(xmldoc, "//c1:SHIP_TO_ADDRESS_LINE_2", nsmgr);
            co.ShipToAddressLine3 = LoadFromXML(xmldoc, "//c1:SHIP_TO_ADDRESS_LINE_3", nsmgr);
            co.ShipToCity = LoadFromXML(xmldoc, "//c1:SHIP_TO_CITY", nsmgr);
            co.ShipToState = LoadFromXML(xmldoc, "//c1:SHIP_TO_STATE", nsmgr);
            co.ShipToCountry = DatabaseFactory.RetrieveISOCountry(LoadFromXML(xmldoc, "//c1:SHIP_TO_COUNTRY", nsmgr));
            co.ShipToPostalCode = LoadFromXML(xmldoc, "//c1:SHIP_TO_POSTAL_CODE", nsmgr);
            co.ShipToPhoneNumber = LoadFromXML(xmldoc, "//c1:SHIP_TO_PHONE_NUMBER", nsmgr);
            co.ShipToFaxNumber = LoadFromXML(xmldoc, "//c1:SHIP_TO_FAX_NUMBER", nsmgr);
            co.ShipToEmailAddress = LoadFromXML(xmldoc, "//c1:SHIP_TO_EMAIL_ADDRESS", nsmgr);
            co.ShipToRefNum = LoadFromXML(xmldoc, "//c1:SHIP_TO_ERP_CONTACT_REF_NUM", nsmgr);

            //See if customer is on hold and if so, log and abort
            string custSeq = "";
            int sPos = co.ShipToRefNum.IndexOf("-", StringComparison.CurrentCulture);
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
                Triggers.forceStop = 1;
                return;
            }

            try
            {
                co.PriorityLevel = Convert.ToInt16(LoadFromXML(xmldoc, "//c1:PRIORITY_LEVEL", nsmgr));
            }
            catch (Exception exPL)
            {
                co.PriorityLevel = 0;                //there is no priority_level in the XML
                Triggers.logEvent = "There is no PRIORITY_LEVEL in the XML.  Defaulting PRIORITY_LEVEL to 0";
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
            }

            co.QuoteNbr = LoadFromXML(xmldoc, "//c1:SERIAL_NUMBER", nsmgr);
            co.WebUserName = LoadFromXML(xmldoc, "//c1:CREATED_BY_USER_ID", nsmgr);
            co.WebOrderDate = System.DateTime.Now;

            //Look for PURCHASE ORDER in INPUTS, load into CO and COITEM
            XmlNode nodePO = xmldoc.SelectSingleNode("//c1:Input[@name='PURCHASE_ORDER']", nsmgr);
            co.CustPO = string.IsNullOrEmpty(nodePO.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodePO.ChildNodes[0].Attributes["name"].InnerXml;
            coitem.CustPO = string.IsNullOrEmpty(nodePO.ChildNodes[0].Attributes["name"].InnerXml) ? " " : nodePO.ChildNodes[0].Attributes["name"].InnerXml;

            //Look for FREIGHT ACCOUNT#, load into CO
            XmlNode nodeFA = xmldoc.SelectSingleNode("//c1:Input[@name='FREIGHT_ACCT']", nsmgr);
            co.FreightAcct = nodeFA.ChildNodes[0].Attributes["name"].InnerXml.Length == 0 ? " " : nodeFA.ChildNodes[0].Attributes["name"].InnerXml;
                        
            //Look for FREIGHT TERMS in INPUTS, load into CO
            XmlNode nodeFT = xmldoc.SelectSingleNode("//c1:Input[@name='FREIGHT_TERMS']", nsmgr);
            co.FreightTerms = nodeFT.ChildNodes[0].Attributes["name"].InnerXml.Length == 0 ? " " : nodeFT.ChildNodes[0].Attributes["name"].InnerXml;
            co.OrderHeaderNotes = " ";

            //Look for Order Header Notes, load into CO
            try
            {
                XmlNode nodeOHN = xmldoc.SelectSingleNode("//c1:Input[@name='ORDER_HEADER_NOTES']", nsmgr);
                co.OrderHeaderNotes = nodeOHN.ChildNodes[0].Attributes["name"].InnerXml.Length == 0 ? " " : nodeOHN.ChildNodes[0].Attributes["name"].InnerXml;
                co.OrderHeaderNotes = co.OrderHeaderNotes.Replace("&amp;", "&");
            }
            catch (Exception lnex)
            {
                Triggers.logEvent = "Order_Note error: " + lnex.Message;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
            }

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
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='PROJECT']", nsmgr);
                co.Project = nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='END_USER']", nsmgr);
                co.EndUser = nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;
                nodeDRS = nodeds.SelectSingleNode("//c1:Input[@name='ENGINEER']", nsmgr);
                co.Engineer = nodeDRS.ChildNodes[0].Attributes["name"].InnerXml;

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
            co.DropShipName = co.DropShipName.Replace("&amp;", "&");
            co.DropShipAddress1 = co.DropShipAddress1.Replace("&amp;", "&");
            co.DropShipAddress2 = co.DropShipAddress2.Replace("&amp;", "&");
            co.DropShipAddress3 = co.DropShipAddress3.Replace("&amp;", "&");
            co.DropShipAddress4 = co.DropShipAddress4.Replace("&amp;", "&");
            co.DropShipContact = co.DropShipContact.Replace("&amp;", "&");

            //build COITEM records, per line
            XmlNodeList xnl = xmldoc.GetElementsByTagName("Detail");
            foreach (XmlNode node in xnl)
            {
                XmlNode nodertv = node.SelectSingleNode("c1:ORDER_LINE_NUM", nsmgr);
                coitem.CO_Line = Convert.ToInt16(nodertv.ChildNodes[0].InnerText);
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);

                nodertv = node.SelectSingleNode("c1:SERIAL_NUM", nsmgr); 
                coitem.Serial = string.IsNullOrEmpty(nodertv.ChildNodes[0].InnerText) ? " " : nodertv.ChildNodes[0].InnerText;

                Triggers.logEvent = "Processing line# " + coitem.CO_Line.ToString();
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);

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


                try
                {
                    nodertv = node.SelectSingleNode("c1:DESCRIPTION", nsmgr);

                    string chkPercent = string.IsNullOrEmpty(nodertv.ChildNodes[0].InnerText) ? " " : nodertv.ChildNodes[0].InnerText;
                    coitem.Desc = chkPercent.Replace("%", "[%]");

                    coitem.Desc = string.IsNullOrEmpty(nodertv.ChildNodes[0].InnerText) ? " " : nodertv.ChildNodes[0].InnerText;
                    nodertv = node.SelectSingleNode("c1:TYPE", nsmgr);
                    coitem.ConfigType = string.IsNullOrEmpty(nodertv.ChildNodes[0].InnerText) ? " " : nodertv.ChildNodes[0].Value;
                    nodertv = node.SelectSingleNode("c1:UNIT_PRICE", nsmgr);
                    coitem.UnitPrice = Convert.ToDecimal(nodertv.ChildNodes[0].InnerText);
                    nodertv = node.SelectSingleNode("c1:UNIT_COST", nsmgr);

                    try
                    {
                        coitem.UnitCost = Convert.ToDecimal(nodertv.ChildNodes[0].InnerText);
                    }
                    catch (Exception costex)
                    {
                        coitem.UnitCost = 0;
                        Triggers.logEvent = "Unit Cost invalid: " + costex.Message + " (Incoming value was: " + nodertv.ChildNodes[0].InnerText + "  Defaulting to 0)";
                        System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
                    }
                    
                    nodertv = node.SelectSingleNode("c1:DISCOUNT_AMT", nsmgr);
                    coitem.Discount = Convert.ToDecimal(nodertv.ChildNodes[0].InnerText);
                    nodertv = node.SelectSingleNode("c1:QUANTITY", nsmgr);
                    coitem.QTY = Convert.ToDecimal(nodertv.ChildNodes[0].InnerText);
                    coitem.PriorityLevel = co.PriorityLevel;
                    globalOrderLineNum = coitem.CO_Line;
                    coitem.OrderLineNotes = " ";
                }
                catch (Exception ciex)
                {
                    Triggers.logEvent = "COITEM Error: " + ciex.Message;
                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
                    return;
                }
                

                //Look for Line Notes, load into COItem
                try
                {
                    nodertv = node.SelectSingleNode("c1:Input[@name='LINE_NOTES']", nsmgr);
                    coitem.OrderLineNotes = nodertv.ChildNodes[0].Attributes["name"].InnerXml.Length == 0 ? " " : nodertv.ChildNodes[0].Attributes["name"].InnerXml;
                    coitem.OrderLineNotes = coitem.OrderLineNotes.Replace("&amp;", "&");
                }
                catch (Exception exrtv)
                {
                    Triggers.logEvent = "Line_Note error: " + exrtv.Message;
                    System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
                }

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
                //Routing - Output only if the Routing tag contains a nodelist count greater than 0
                Triggers.logEvent = "STARTING ROUTING LOGIC";
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                int routebomSeq = 0;
                bool UseRouteBOM = false;
                route.CO_Num = coitem.CO_Num;
                route.CO_Line = coitem.CO_Line;
                XmlNodeList xnlr = detailDoc.GetElementsByTagName("Routing");
                if (xnlr.Count > 0)
                {
                    UseRouteBOM = true;
                    foreach (XmlNode nodecr in xnlr)
                    {
                        XmlNode nodecrtg = nodecr.SelectSingleNode("c1:SMARTPART_NUM", nsmgr);
                        route.SmartpartNum = nodecrtg.ChildNodes[0].InnerText;
                        nodecrtg = nodecr.SelectSingleNode("c1:ITEM_NUM", nsmgr);
                        route.ItemNum = nodecrtg.ChildNodes[0].InnerText;
                        nodecrtg = nodecr.SelectSingleNode("c1:BOM_ID", nsmgr);
                        route.BOM_ID = nodecrtg.ChildNodes[0].InnerText;

                        //isolate OPERATION elements from Routing and process
                        XmlDocument routeOP = new XmlDocument();
                        routeOP.LoadXml(nodecr.OuterXml);
                        XmlNodeList xnlOperation = routeOP.GetElementsByTagName("Operation");
                        foreach (XmlNode nodeol in xnlOperation)
                        {
                            XmlNode operationParentTL = nodeol.SelectSingleNode(".");             //current operation becomes our new parent (.. to .)
                            XmlDocument operationDocumentTL = new XmlDocument();
                            operationDocumentTL.LoadXml(operationParentTL.OuterXml);
                            XmlNodeList xnlOP = operationDocumentTL.GetElementsByTagName("OperationParam");
                            foreach (XmlNode nodeParamChild in xnlOP)
                            {
                                if (nodeParamChild.ChildNodes[0].InnerText == "LABOR_HRS") { route.Labor_Hours = Convert.ToDouble(nodeParamChild.ChildNodes[2].InnerText); }
                                if (nodeParamChild.ChildNodes[0].InnerText == "SETUP_HRS") { route.Setup_Hours = Convert.ToDouble(nodeParamChild.ChildNodes[2].InnerText); }
                                if (nodeParamChild.ChildNodes[0].InnerText == "WC") { route.WC = nodeParamChild.ChildNodes[2].InnerText; }
                                if (nodeParamChild.ChildNodes[0].InnerText == "NOTES") { route.Notes = nodeParamChild.ChildNodes[2].InnerText; }
                                if (nodeParamChild.ChildNodes[0].InnerText == "MACH_NAME") { route.Machine_Name = nodeParamChild.ChildNodes[2].InnerText; }
                            }

                            XmlNode nodeOpItem = nodeol.SelectSingleNode("c1:OPER_NUM", nsmgr);
                            route.OPERATION = Convert.ToInt16(nodeOpItem.ChildNodes[0].InnerText);
                            nodeOpItem = nodeol.SelectSingleNode("c1:DESCRIPTION", nsmgr);
                            route.Description = nodeOpItem.ChildNodes[0].InnerText;

                            //set current operation as new parent and look ONLY for its OperationInput tags
                            var operationParent = nodeol.SelectSingleNode(".");     //ensure we traverse ONLY descendants of this node (Operation) as the new parent 
                            
                            XmlDocument operationDoc = new XmlDocument();
                            operationDoc.LoadXml(operationParent.OuterXml);
                            XmlNodeList xnlOperationInputs = operationDoc.GetElementsByTagName("OperationInput");

                            foreach (XmlNode nodeOpInput in xnlOperationInputs)
                            {
                                XmlNode nodeoin = nodeOpInput.SelectSingleNode("c1:SMARTPART_NUM", nsmgr);
                                route.MatlSmartpartNum = nodeOpInput.ChildNodes[0].InnerText;
                                route.MatlItemNum = nodeOpInput.ChildNodes[1].InnerText;
                                route.MatlQty = Convert.ToDecimal(nodeOpInput.ChildNodes[2].InnerText);
                                //write to gr_cfgroutebom
                                routebomSeq += 1;
                                route.Seq = routebomSeq;
                                DatabaseFactory.WriteRecordCfgRoute(ref route);
                            }
                        }
                    }
                }

                Triggers.logEvent = "RESEQUENCING BOM RECORDS FOR ORDER: " + bom.CO_Num + " LINE: " + bom.CO_Line;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                DatabaseFactory.ResequenceBOM(bom.CO_Num, bom.CO_Line);

                //If we used routeBOM, dump the old-process BOM records
                //if (UseRouteBOM) { DatabaseFactory.DeleteBOM(co.CORefNum); }          //this may be reinstated in the future
            }
            DatabaseFactory.WriteRecordCO(ref co);              //deferred write
        }
        public static string LoadFromXML(XmlDocument xmldoc, string element, XmlNamespaceManager nsmgr)
        {
            string returnValue = "";

            try
            {
                XmlNode xnode = xmldoc.SelectSingleNode(element, nsmgr);
                returnValue = string.IsNullOrEmpty(xnode.InnerText) ? " " : xnode.InnerText;
            }
            catch (Exception exLoad)
            {
                Triggers.logEvent = "Attempt to access XML tag <" + element + "> failed:  " + exLoad.Message;
                string auditMessage = "Attempt to access XML tag (" + element + ") failed:  " + exLoad.Message;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
                returnValue = " ";

                Audit.SetTruncate("MAPPING", 2, 1, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, auditMessage);

            }
            
            return returnValue;
        }
        public static void initCOFields()
        {
            
        }
    }
}

