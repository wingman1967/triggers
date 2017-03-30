using System;
using System.Xml;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Utilities used for staging of data from C1 to SQL databases
    /// </summary>
    class StagingUtilities
    {
        public static string globalOrderNum;
        public static int globalOrderLineNum;
                
        public static void MapXMLToSQL(XmlDocument xmldoc)
        {
            zCfgCO co = new zCfgCO();
            zCfgCOitem coitem = new zCfgCOitem();
            zCfgItem citem = new zCfgItem();
            zCfgParmVal cfg = new zCfgParmVal();
            zCfgBOM bom = new zCfgBOM();

            Audit.resetmE = true;       //reset the mE array in case we have any mapping errors to report for this cycle

            //*** Determine what site we are working with and re-set the connection string accordingly, else default to NOVB
            XmlNodeList xnlsite = xmldoc.GetElementsByTagName("Input");
            foreach (XmlNode node in xnlsite)
            {
                switch (node.ChildNodes[2].InnerText == "ORDER SITE")
                {
                    case true:
                        string rplConnectionString = DatabaseFactory.connectionString;
                        int csPos = rplConnectionString.IndexOf("NOVB");
                        DatabaseFactory.connectionString = rplConnectionString.Substring(0, csPos) + node.ChildNodes[0].Attributes["name"].Value + rplConnectionString.Substring(csPos + 4, rplConnectionString.Length - (csPos + 4));
                        Triggers.logEvent = "Connection String: " + DatabaseFactory.connectionString;
                        System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                        break;
                    default:
                        //continue with our NOVB default
                        break;
                }
            }

            //Retrieve due date
            XmlNodeList xnlduedate = xmldoc.GetElementsByTagName("Input");
            foreach (XmlNode node in xnlduedate)
            {
                switch (node.ChildNodes[2].InnerText == "DUE DATE")
                {
                    case true:
                        co.DueDate = Convert.ToDateTime(node.ChildNodes[0].Attributes["name"].Value);
                        coitem.DueDate = Convert.ToDateTime(node.ChildNodes[0].Attributes["name"].Value);
                        break;
                    default:
                        co.DueDate = DateTime.Now;
                        coitem.DueDate = DateTime.Now;
                        break;
                }
            }
            
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
                co.Project = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("PAYMENT_TERMS");
            foreach (XmlNode node in xnl)
            {
                co.PaymentTerms = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_VIA");
            foreach (XmlNode node in xnl)
            {
                co.ShipVia = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIPPING_TERMS");
            foreach (XmlNode node in xnl)
            {
                co.ShippingTerms = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_CONTACT_NAME");
            foreach (XmlNode node in xnl)
            {
                co.BillToContactName = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_ADDRESS_LINE_1");
            foreach (XmlNode node in xnl)
            {
                co.BillToAddressLine1 = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_ADDRESS_LINE_2");
            foreach (XmlNode node in xnl)
            {
                co.BillToAddressLine2 = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_ADDRESS_LINE_3");
            foreach (XmlNode node in xnl)
            {
                co.BillToAddressLine3 = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_CITY");
            foreach (XmlNode node in xnl)
            {
                co.BillToCity = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_STATE");
            foreach (XmlNode node in xnl)
            {
                co.BillToState = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_COUNTRY");
            foreach (XmlNode node in xnl)
            {
                co.BillToCountry = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_POSTAL_CODE");
            foreach (XmlNode node in xnl)
            {
                co.BillToPostalCode = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_PHONE_NUMBER");
            foreach (XmlNode node in xnl)
            {
                co.BillToPhoneNumber = node.InnerText;
                if (co.BillToPhoneNumber.Length == 0) { co.BillToPhoneNumber = "NA"; };
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_FAX_NUMBER");
            foreach (XmlNode node in xnl)
            {
                co.BillToFaxNumber = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_EMAIL_ADDRESS");
            foreach (XmlNode node in xnl)
            {
                co.BillToEmailAddress = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("BILL_TO_REF_NUM");
            foreach (XmlNode node in xnl)
            {
                co.BillToRefNum = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_CONTACT_NAME");
            foreach (XmlNode node in xnl)
            {
                co.ShipToContactName = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_ADDRESS_LINE_1");
            foreach (XmlNode node in xnl)
            {
                co.ShipToAddressLine1 = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_ADDRESS_LINE_2");
            foreach (XmlNode node in xnl)
            {
                co.ShipToAddressLine2 = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_ADDRESS_LINE_3");
            foreach (XmlNode node in xnl)
            {
                co.ShipToAddressLine3 = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_CITY");
            foreach (XmlNode node in xnl)
            {
                co.ShipToCity = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_STATE");
            foreach (XmlNode node in xnl)
            {
                co.ShipToState = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_COUNTRY");
            foreach (XmlNode node in xnl)
            {
                co.ShipToCountry = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_POSTAL_CODE");
            foreach (XmlNode node in xnl)
            {
                co.ShipToPostalCode = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_PHONE_NUMBER");
            foreach (XmlNode node in xnl)
            {
                co.ShipToPhoneNumber = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_FAX_NUMBER");
            foreach (XmlNode node in xnl)
            {
                co.ShipToFaxNumber = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_EMAIL_ADDRESS");
            foreach (XmlNode node in xnl)
            {
                co.ShipToEmailAddress = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("SHIP_TO_REF_NUM");
            foreach (XmlNode node in xnl)
            {
                co.ShipToRefNum = node.InnerText;
            }
            xnl = xmldoc.GetElementsByTagName("PRIORITY_LEVEL");
            foreach (XmlNode node in xnl)
            {
                co.PriorityLevel = Convert.ToInt16(node.InnerText);
            }
            //output CO header to SQL before proceeding to the coitem
            DatabaseFactory.WriteRecordCO(ref co);
            
            //build COITEM records, per line
            xnl = xmldoc.GetElementsByTagName("Detail");
            foreach (XmlNode node in xnl)
            {
                coitem.CO_Line = Convert.ToInt16(node.ChildNodes[0].InnerText);
                coitem.Serial = node.ChildNodes[2].InnerText;
                coitem.Item = node.ChildNodes[3].InnerText;
                coitem.Smartpart = node.ChildNodes[4].InnerText;
                coitem.Desc = node.ChildNodes[5].InnerText;
                coitem.ConfigType = node.ChildNodes[6].InnerText;
                coitem.UnitPrice = Convert.ToDecimal(node.ChildNodes[7].InnerText);
                coitem.UnitCost = Convert.ToDecimal(node.ChildNodes[8].InnerText);
                coitem.Discount = Convert.ToDecimal(node.ChildNodes[9].InnerText);
                coitem.QTY = Convert.ToDecimal(node.ChildNodes[10].InnerText);
                coitem.PriorityLevel = co.PriorityLevel;
                globalOrderLineNum = coitem.CO_Line;
                //output coitem record
                DatabaseFactory.WriteRecordCOItem(ref coitem);

                //*** Everything else here builds on the COITEM ***
                var detailParent = node.SelectSingleNode(".");     //needed to ensure we traverse ONLY the children of this (Detail) as the new parent
                XmlDocument detailDoc = new XmlDocument();
                detailDoc.LoadXml(detailParent.OuterXml);

                //iterate through Inputs for the line
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
                //item-master for the line we are working with
                int recordSequence = 1;
                XmlNodeList xnlim = detailDoc.GetElementsByTagName("ItemMaster");
                citem.CO_Line = coitem.CO_Line;
                foreach (XmlNode nodeim in xnlim)
                {
                    citem.Sequence = recordSequence;
                    citem.Smartpart = nodeim.ChildNodes[0].InnerText;
                    citem.Item = nodeim.ChildNodes[1].InnerText;
                    citem.Desc = nodeim.ChildNodes[3].InnerText;
                    string cost = nodeim.ChildNodes[4].InnerText;
                    string price = nodeim.ChildNodes[5].InnerText;
                    string sell = nodeim.ChildNodes[6].InnerText;
                    string weight = nodeim.ChildNodes[9].InnerText;
                    citem.ItemCost = Convert.ToDecimal(string.IsNullOrEmpty(cost) ? "0" : cost);
                    citem.ItemPrice = Convert.ToDecimal(string.IsNullOrEmpty(price) ? "0" : price);
                    citem.ItemSellPrice = Convert.ToDecimal(string.IsNullOrEmpty(sell) ? "0" : sell);
                    citem.ItemWeight = Convert.ToDecimal(string.IsNullOrEmpty(weight) ? "0" : weight);
                    citem.UnitOfMeasure = nodeim.ChildNodes[26].InnerText;
                    citem.PriorityLevel = Convert.ToInt16(nodeim.ChildNodes[38].InnerText);
                    recordSequence += 1;
                    //output item-master record
                    DatabaseFactory.WriteRecordCItem(ref citem);
                }
                //BOM records
                recordSequence = 0;
                XmlNodeList xnlb = detailDoc.GetElementsByTagName("Bom");
                bom.CO_Line = coitem.CO_Line;
                foreach (XmlNode nodeib in xnlb)
                {
                    recordSequence = 1;
                    bom.Sequence = recordSequence;
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
                    bom.PriorityLevel = co.PriorityLevel;
                    //output BOM record
                    DatabaseFactory.WriteRecordBOM(ref bom);
                }
                Triggers.logEvent = "RESEQUENCING BOM RECORDS FOR ORDER: " + bom.CO_Num + " LINE: " + bom.CO_Line;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Information, 234);
                DatabaseFactory.ResequenceBOM(bom.CO_Num, bom.CO_Line);
            }
        }
    }
}

