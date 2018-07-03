using System.Collections.Generic;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Management of dictionaries used for C1 data mapping/processing
    /// </summary>
    public static class C1Dictionaries
    {
        public static Dictionary<string, string> webmethods = new Dictionary<string, string>();
        public static Dictionary<string, int> fieldLengths = new Dictionary<string, int>();
        public static int objectLength;
        public static void LoadWMDictionary()
        {
            //DEV and TEST objects
            webmethods.Clear();
            webmethods.Add("getOrder", "http://nationaldev.conceptconfigurator.com/webservices/services/ConceptAccess?method=getOrder");
            webmethods.Add("updateOrder", "http://nationaldev.conceptconfigurator.com/webservices/services/ConceptAccess?method=updateOrder");
            webmethods.Add("getConfiguration", "http://nationaldev.conceptconfigurator.com/webservices/services/ConceptAccess?method=getConfiguration");
            webmethods.Add("getDocument", "http://nationaldev.conceptconfigurator.com/webservices/services/ConceptAccess?method=getDocument");
            //Production objects
            webmethods.Add("getOrderPROD", "http://national.conceptconfigurator.com/webservices/services/ConceptAccess?method=getOrder");
            webmethods.Add("updateOrderPROD", "http://national.conceptconfigurator.com/webservices/services/ConceptAccess?method=updateOrder");
            webmethods.Add("getConfigurationPROD", "http://national.conceptconfigurator.com/webservices/services/ConceptAccess?method=getConfiguration");
            webmethods.Add("getDocumentPROD", "http://national.conceptconfigurator.com/webservices/services/ConceptAccess?method=getDocument");
        }
        public static void LoadDBFLengthDictionary()
        {
            fieldLengths.Clear();
            //BOM Fields
            fieldLengths.Add("BOM:order_num", 15);
            fieldLengths.Add("BOM:parent_id", 20);
            fieldLengths.Add("BOM:id", 20);
            fieldLengths.Add("BOM:item_num", 40);
            fieldLengths.Add("BOM:smartpart_num", 40);
            //CO Fields
            fieldLengths.Add("CO:id", 15);
            fieldLengths.Add("CO:order_num", 15);
            fieldLengths.Add("CO:order_ref_num", 15);
            fieldLengths.Add("CO:cust_name", 40);
            fieldLengths.Add("CO:cust_ref_num", 15);
            fieldLengths.Add("CO:account_num", 15);
            fieldLengths.Add("CO:erp_reference_num", 15);
            fieldLengths.Add("CO:project", 40);
            fieldLengths.Add("CO:payment_terms", 15);
            fieldLengths.Add("CO:ship_via", 15);
            fieldLengths.Add("CO:shipping_terms", 40);
            fieldLengths.Add("CO:bill_to_contact_name", 40);
            fieldLengths.Add("CO:bill_address_line_1", 40);
            fieldLengths.Add("CO:bill_address_line_2", 40);
            fieldLengths.Add("CO:bill_address_line_3", 40);
            fieldLengths.Add("CO:bill_to_city", 40);
            fieldLengths.Add("CO:bill_to_state", 40);
            fieldLengths.Add("CO:bill_to_country", 40);
            fieldLengths.Add("CO:bill_to_postal_code", 15);
            fieldLengths.Add("CO:bill_to_phone_number", 25);
            fieldLengths.Add("CO:bill_to_fax_number", 25);
            fieldLengths.Add("CO:bill_to_email_address", 60);
            fieldLengths.Add("CO:bill_to_ref_num", 15);
            fieldLengths.Add("CO:ship_to_contact_name", 40);
            fieldLengths.Add("CO:ship_address_line_1", 40);
            fieldLengths.Add("CO:ship_address_line_2", 40);
            fieldLengths.Add("CO:ship_address_line_3", 40);
            fieldLengths.Add("CO:ship_to_city", 40);
            fieldLengths.Add("CO:ship_to_state", 40);
            fieldLengths.Add("CO:ship_to_country", 40);
            fieldLengths.Add("CO:ship_to_postal_code", 15);
            fieldLengths.Add("CO:ship_to_phone_number", 25);
            fieldLengths.Add("CO:ship_to_fax_number", 25);
            fieldLengths.Add("CO:ship_to_email_address", 60);
            fieldLengths.Add("CO:ship_to_ref_num", 15);
            fieldLengths.Add("CO:CustPO", 22);
            fieldLengths.Add("CO:ship_address_line_4", 40);
            fieldLengths.Add("CO:freight_terms", 100);
            fieldLengths.Add("CO:freight_acct", 100);
            fieldLengths.Add("CO:quote_nbr", 10);
            fieldLengths.Add("CO:web_user_name", 30);
            fieldLengths.Add("CO:dropship_address1", 50);
            fieldLengths.Add("CO:dropship_address2", 50);
            fieldLengths.Add("CO:dropship_address3", 50);
            fieldLengths.Add("CO:dropship_address4", 50);
            fieldLengths.Add("CO:dropship_city", 30);
            fieldLengths.Add("CO:dropship_state", 5);
            fieldLengths.Add("CO:dropship_zip", 10);
            fieldLengths.Add("CO:dropship_name", 60);
            fieldLengths.Add("CO:dropship_contact", 30);
            fieldLengths.Add("CO:dropship_country", 30);
            fieldLengths.Add("CO:dropship_phone", 25);
            fieldLengths.Add("CO:dropship_email", 60);
            fieldLengths.Add("CO:destination_country", 40);
            fieldLengths.Add("CO:ORDER_HEADER_NOTES", 1000);
            fieldLengths.Add("CO:end_user", 100);
            fieldLengths.Add("CO:engineer", 100);
            //COITEM Fields
            fieldLengths.Add("COItem:order_num", 15);
            fieldLengths.Add("COItem:ser_num", 15);
            fieldLengths.Add("COItem:item_num", 30);
            fieldLengths.Add("COItem:smartpart_num", 30);
            fieldLengths.Add("COItem:description", 1000);
            fieldLengths.Add("COItem:LINE_NOTES", 1000);
            fieldLengths.Add("COItem:CustPO", 22);
            //CfgItem Fields
            fieldLengths.Add("CfgItem:order_num", 15);
            fieldLengths.Add("CfgItem:smartpart_num", 40);
            fieldLengths.Add("CfgItem:item_num", 40);
            fieldLengths.Add("CfgItem:description", 1000);
            fieldLengths.Add("CfgItem:uom", 10);
            fieldLengths.Add("CfgItem:im_VAR1", 200);
            fieldLengths.Add("CfgItem:im_VAR2", 200);
            fieldLengths.Add("CfgItem:im_VAR3", 200);
            fieldLengths.Add("CfgItem:im_VAR4", 200);
            fieldLengths.Add("CfgItem:im_VAR5", 200);
            //CfgParmVal Fields
            fieldLengths.Add("CfgParmVal:order_num", 15);
            fieldLengths.Add("CfgParmVal:name", 255);
            fieldLengths.Add("CfgParmVal:value", 1024);
            fieldLengths.Add("CfgParmVal:type", 15);
            fieldLengths.Add("CfgParmVal:label", 255);
            //CfgRoute Fields
            fieldLengths.Add("CfgRoute:order_num", 15);
            fieldLengths.Add("CfgRoute:bom_id", 30);
            fieldLengths.Add("CfgRoute:description", 50);
            fieldLengths.Add("CfgRoute:notes", 500);
            fieldLengths.Add("CfgRoute:mach_name", 50);
        }
        public static int DBFieldLenCO(string field, ref zCfgCO co)
        {
            int fieldlength = (!fieldLengths.ContainsKey(field)) ? 15 : fieldLengths[field];
            switch (field)
            {
                case "CO:bill_to_phone_number": switch (co.BillToPhoneNumber.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.BillToPhoneNumber.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.BillToPhoneNumber); break; default: fieldlength = co.BillToPhoneNumber.Length; break; } break;
                case "CO:ship_via": switch (co.ShipVia.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipVia.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipVia); break; default: fieldlength = co.ShipVia.Length; break; } break;
                case "CO:shipping_terms": switch (co.ShippingTerms.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShippingTerms.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShippingTerms); break; default: fieldlength = co.ShippingTerms.Length; break; } break;
                case "CO:bill_to_fax_number": switch (co.BillToFaxNumber.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.BillToFaxNumber.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.BillToFaxNumber); break; default: fieldlength = co.BillToFaxNumber.Length; break; } break;
                case "CO:dropship_address1": switch (co.DropShipAddress1.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DropShipAddress1.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DropShipAddress1); break; default: fieldlength = co.DropShipAddress1.Length; break; } break;
                case "CO:dropship_address2": switch (co.DropShipAddress2.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DropShipAddress2.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DropShipAddress2); break; default: fieldlength = co.DropShipAddress2.Length; break; } break;
                case "CO:dropship_address3": switch (co.DropShipAddress3.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DropShipAddress3.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DropShipAddress3); break; default: fieldlength = co.DropShipAddress3.Length; break; } break;
                case "CO:dropship_address4": switch (co.DropShipAddress4.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DropShipAddress4.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DropShipAddress4); break; default: fieldlength = co.DropShipAddress4.Length; break; } break;
                case "CO:dropship_city": switch (co.DropShipCity.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DropShipCity.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DropShipCity); break; default: fieldlength = co.DropShipCity.Length; break; } break;
                case "CO:dropship_state": switch (co.DropShipState.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DropShipState.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DropShipState); break; default: fieldlength = co.DropShipState.Length; break; } break;
                case "CO:dropship_zip": switch (co.DropShipZip.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DropShipZip.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DropShipZip); break; default: fieldlength = co.DropShipZip.Length; break; } break;
                case "CO:dropship_name": switch (co.DropShipName.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DropShipName.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DropShipName); break; default: fieldlength = co.DropShipName.Length; break; } break;
                case "CO:dropship_contact": switch (co.DropShipContact.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DropShipContact.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DropShipContact); break; default: fieldlength = co.DropShipContact.Length; break; } break;
                case "CO:dropship_country": switch (co.DropShipCountry.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DropShipCountry.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DropShipCountry); break; default: fieldlength = co.DropShipCountry.Length; break; } break;
                case "CO:dropship_phone": switch (co.DropShipPhone.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DropShipPhone.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DropShipPhone); break; default: fieldlength = co.DropShipPhone.Length; break; } break;
                case "CO:CustPO": switch (co.CustPO.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.CustPO.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.CustPO); break; default: fieldlength = co.CustPO.Length; break; } break;
                case "CO:ship_address_line_4": switch (co.ShipToAddressLine4.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipToAddressLine4.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipToAddressLine4); break; default: fieldlength = co.ShipToAddressLine4.Length; break; } break;
                case "CO:freight_terms": switch (co.FreightTerms.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.FreightTerms.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.FreightTerms); break; default: fieldlength = co.FreightTerms.Length; break; } break;
                case "CO:freight_acct": switch (co.FreightAcct.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.FreightAcct.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.FreightAcct); break; default: fieldlength = co.FreightAcct.Length; break; } break;
                case "CO:dropship_email": switch (co.DropShipEmail.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DropShipEmail.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DropShipEmail); break; default: fieldlength = co.DropShipEmail.Length; break; } break;
                case "CO:bill_to_contact_name": switch (co.BillToContactName.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.BillToContactName.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.BillToContactName); break; default: fieldlength = co.BillToContactName.Length; break; } break;
                case "CO:bill_address_line_1": switch (co.BillToAddressLine1.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.BillToAddressLine1.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.BillToAddressLine1); break; default: fieldlength = co.BillToAddressLine1.Length; break; } break;
                case "CO:bill_address_line_2": switch (co.BillToAddressLine2.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.BillToAddressLine2.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.BillToAddressLine2); break; default: fieldlength = co.BillToAddressLine2.Length; break; } break;
                case "CO:bill_address_line_3": switch (co.BillToAddressLine3.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.BillToAddressLine3.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.BillToAddressLine3); break; default: fieldlength = co.BillToAddressLine3.Length; break; } break;
                case "CO:bill_to_city": switch (co.BillToCity.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.BillToCity.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.BillToCity); break; default: fieldlength = co.BillToCity.Length; break; } break;
                case "CO:bill_to_state": switch (co.BillToState.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.BillToState.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.BillToState); break; default: fieldlength = co.BillToState.Length; break; } break;
                case "CO:bill_to_country": switch (co.BillToCountry.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.BillToCountry.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.BillToCountry); break; default: fieldlength = co.BillToCountry.Length; break; } break;
                case "CO:bill_to_postal_code": switch (co.BillToPostalCode.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.BillToPostalCode.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.BillToPostalCode); break; default: fieldlength = co.BillToPostalCode.Length; break; } break;
                case "CO:bill_to_email_address": switch (co.BillToEmailAddress.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.BillToEmailAddress.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.BillToEmailAddress); break; default: fieldlength = co.BillToEmailAddress.Length; break; } break;
                case "CO:ship_to_contact_name": switch (co.ShipToContactName.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipToContactName.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipToContactName); break; default: fieldlength = co.ShipToContactName.Length; break; } break;
                case "CO:ship_address_line_1": switch (co.ShipToAddressLine1.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipToAddressLine1.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipToAddressLine1); break; default: fieldlength = co.ShipToAddressLine1.Length; break; } break;
                case "CO:ship_address_line_2": switch (co.ShipToAddressLine2.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipToAddressLine2.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipToAddressLine2); break; default: fieldlength = co.ShipToAddressLine2.Length; break; } break;
                case "CO:ship_address_line_3": switch (co.ShipToAddressLine3.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipToAddressLine3.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipToAddressLine3); break; default: fieldlength = co.ShipToAddressLine3.Length; break; } break;
                case "CO:ship_to_city": switch (co.ShipToCity.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipToCity.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipToCity); break; default: fieldlength = co.ShipToCity.Length; break; } break;
                case "CO:ship_to_state": switch (co.ShipToState.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipToState.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipToState); break; default: fieldlength = co.ShipToState.Length; break; } break;
                case "CO:ship_to_country": switch (co.ShipToCountry.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipToCountry.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipToCountry); break; default: fieldlength = co.ShipToCountry.Length; break; } break;
                case "CO:ship_to_postal_code": switch (co.ShipToPostalCode.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipToPostalCode.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipToPostalCode); break; default: fieldlength = co.ShipToPostalCode.Length; break; } break;
                case "CO:ship_to_email_address": switch (co.ShipToEmailAddress.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipToEmailAddress.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipToEmailAddress); break; default: fieldlength = co.ShipToEmailAddress.Length; break; } break;
                case "CO:ship_to_fax_number": switch (co.ShipToFaxNumber.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipToFaxNumber.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipToFaxNumber); break; default: fieldlength = co.ShipToFaxNumber.Length; break; } break;
                case "CO:ship_to_phone_number": switch (co.ShipToPhoneNumber.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.ShipToPhoneNumber.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.ShipToPhoneNumber); break; default: fieldlength = co.ShipToPhoneNumber.Length; break; } break;
                case "CO:destination_country": switch (co.DestinationCountry.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.DestinationCountry.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.DestinationCountry); break; default: fieldlength = co.DestinationCountry.Length; break; } break;
                case "CO:ORDER_HEADER_NOTES": switch (co.OrderHeaderNotes.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.OrderHeaderNotes.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.OrderHeaderNotes); break; default: fieldlength = co.OrderHeaderNotes.Length; break; } break;
                case "CO:project": switch (co.Project.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.Project.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.Project); break; default: fieldlength = co.Project.Length; break; } break;
                case "CO:end_user": switch (co.EndUser.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.EndUser.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.EndUser); break; default: fieldlength = co.EndUser.Length; break; } break;
                case "CO:engineer": switch (co.Engineer.Length >= fieldlength) { case true: Audit.SetTruncate(field, co.Engineer.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, co.Engineer); break; default: fieldlength = co.Engineer.Length; break; } break;
            }

            return fieldlength;
        }
        public static int DBFieldLenCOItem(string field, ref zCfgCOitem coitem)
        {
            int fieldlength = (!fieldLengths.ContainsKey(field)) ? 15 : fieldLengths[field];
            switch (field)
            {
                case "COItem:ser_num": switch (coitem.Serial.Length >= fieldlength) { case true: Audit.SetTruncate(field, coitem.Serial.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, coitem.Serial); break; default: fieldlength = coitem.Serial.Length; break; } break;
                case "COItem:item_num": switch (coitem.Item.Length >= fieldlength) { case true: Audit.SetTruncate(field, coitem.Item.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, coitem.Item); break; default: fieldlength = coitem.Item.Length; break; } break;
                case "COItem:smartpart_num": switch (coitem.Smartpart.Length >= fieldlength) { case true: Audit.SetTruncate(field, coitem.Smartpart.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, coitem.Smartpart); break; default: fieldlength = coitem.Smartpart.Length; break; } break;
                case "COItem:description": switch (coitem.Desc.Length >= fieldlength) { case true: Audit.SetTruncate(field, coitem.Desc.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, coitem.Desc); break; default: fieldlength = coitem.Desc.Length; break; } break;
                case "COItem:LINE_NOTES": switch (coitem.OrderLineNotes.Length >= fieldlength) { case true: Audit.SetTruncate(field, coitem.OrderLineNotes.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, coitem.OrderLineNotes); break; default: fieldlength = coitem.OrderLineNotes.Length; break; } break;
                case "COItem:CustPO": switch (coitem.CustPO.Length >= fieldlength) { case true: Audit.SetTruncate(field, coitem.CustPO.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, coitem.CustPO); break; default: fieldlength = coitem.CustPO.Length; break; } break;
            }
            return fieldlength;
        }
        public static int DBFieldLenCfgItem(string field, ref zCfgItem citem)
        {
            int fieldlength = (!fieldLengths.ContainsKey(field)) ? 15 : fieldLengths[field];
            switch (field)
            {
                case "CfgItem:smartpart_num": switch (citem.Smartpart.Length >= fieldlength) { case true: Audit.SetTruncate(field, citem.Smartpart.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, citem.Smartpart); break; default: fieldlength = citem.Smartpart.Length; break; } break;
                case "CfgItem:item_num": switch (citem.Item.Length >= fieldlength) { case true: Audit.SetTruncate(field, citem.Item.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, citem.Item); break; default: fieldlength = citem.Item.Length; break; } break;
                case "CfgItem:description": switch (citem.Desc.Length >= fieldlength) { case true: Audit.SetTruncate(field, citem.Desc.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, citem.Desc); break; default: fieldlength = citem.Desc.Length; break; } break;
                case "CfgItem:uom": switch (citem.UnitOfMeasure.Length >= fieldlength) { case true: Audit.SetTruncate(field, citem.UnitOfMeasure.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, citem.UnitOfMeasure); break; default: fieldlength = citem.UnitOfMeasure.Length; break; } break;
                case "CfgItem:im_VAR1": switch (citem.IM_VAR1.Length >= fieldlength) { case true: Audit.SetTruncate(field, citem.IM_VAR1.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, citem.IM_VAR1); break; default: fieldlength = citem.IM_VAR1.Length; break; } break;
                case "CfgItem:im_VAR2": switch (citem.IM_VAR2.Length >= fieldlength) { case true: Audit.SetTruncate(field, citem.IM_VAR2.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, citem.IM_VAR2); break; default: fieldlength = citem.IM_VAR2.Length; break; } break;
                case "CfgItem:im_VAR3": switch (citem.IM_VAR3.Length >= fieldlength) { case true: Audit.SetTruncate(field, citem.IM_VAR3.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, citem.IM_VAR3); break; default: fieldlength = citem.IM_VAR3.Length; break; } break;
                case "CfgItem:im_VAR4": switch (citem.IM_VAR4.Length >= fieldlength) { case true: Audit.SetTruncate(field, citem.IM_VAR4.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, citem.IM_VAR4); break; default: fieldlength = citem.IM_VAR4.Length; break; } break;
                case "CfgItem:im_VAR5": switch (citem.IM_VAR5.Length >= fieldlength) { case true: Audit.SetTruncate(field, citem.IM_VAR5.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, citem.IM_VAR5); break; default: fieldlength = citem.IM_VAR5.Length; break; } break;
            }
            return fieldlength;
        }
        public static int DBFieldLenParmVal(string field, ref zCfgParmVal cfg)
        {
            int fieldlength = (!fieldLengths.ContainsKey(field)) ? 15 : fieldLengths[field];
            switch (field)
            {
                case "CfgParmVal:name": switch (cfg.CName.Length >= fieldlength) { case true: Audit.SetTruncate(field, cfg.CName.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, cfg.CName); break; default: fieldlength = cfg.CName.Length; break; } break;
                case "CfgParmVal:value": switch (cfg.CValue.Length >= fieldlength) { case true: Audit.SetTruncate(field, cfg.CValue.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, cfg.CValue); break; default: fieldlength = cfg.CValue.Length; break; } break;
                case "CfgParmVal:type": switch (cfg.CType.Length >= fieldlength) { case true: Audit.SetTruncate(field, cfg.CType.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, cfg.CType); break; default: fieldlength = cfg.CType.Length; break; } break;
                case "CfgParmVal:label": switch (cfg.CLabel.Length >= fieldlength) { case true: Audit.SetTruncate(field, cfg.CLabel.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, cfg.CLabel); break; default: fieldlength = cfg.CLabel.Length; break; } break;
            }
            return fieldlength;
        }
        public static int DBFieldLenBOM(string field, ref zCfgBOM bom)
        {
            int fieldlength = (!fieldLengths.ContainsKey(field)) ? 15 : fieldLengths[field];
            switch (field)
            {
                case "BOM:id": switch (bom.Identifier.Length >= fieldlength) { case true: Audit.SetTruncate(field, bom.Identifier.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, bom.Identifier); break; default: fieldlength = bom.Identifier.Length; break; } break;
                case "BOM:item_num": switch (bom.Item.Length >= fieldlength) { case true: Audit.SetTruncate(field, bom.Item.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, bom.Item); break; default: fieldlength = bom.Item.Length; break; } break;
                case "BOM:smartpart_num": switch (bom.Smartpart.Length >= fieldlength) { case true: Audit.SetTruncate(field, bom.Smartpart.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, bom.Smartpart); break; default: fieldlength = bom.Smartpart.Length; break; } break;
            }
            return fieldlength;
        }
        public static int DBFieldLenRoute(string field, ref zCfgRoute route)
        {
            int fieldlength = (!fieldLengths.ContainsKey(field)) ? 15 : fieldLengths[field];
            switch (field)
            {
                case "CfgRoute:order_num": switch (route.CO_Num.Length >= fieldlength) { case true: Audit.SetTruncate(field, route.CO_Num.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, route.CO_Num); break; default: fieldlength = route.CO_Num.Length; break; } break;
                case "CfgRoute:bom_id": switch (route.BOM_ID.Length >= fieldlength) { case true: Audit.SetTruncate(field, route.BOM_ID.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, route.BOM_ID); break; default: fieldlength = route.BOM_ID.Length; break; } break;
                case "CfgRoute:description": switch (route.Description.Length >= fieldlength) { case true: Audit.SetTruncate(field, route.Description.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, route.Description); break; default: fieldlength = route.Description.Length; break; } break;
                case "CfgRoute:notes": switch (route.Notes.Length >= fieldlength) { case true: Audit.SetTruncate(field, route.Notes.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, route.Notes); break; default: fieldlength = route.Notes.Length; break; } break;
                case "CfgRoute:mach_name": switch (route.Machine_Name.Length >= fieldlength) { case true: Audit.SetTruncate(field, route.Machine_Name.Length, fieldlength, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, route.Machine_Name); break; default: fieldlength = route.Machine_Name.Length; break; } break;
            }
            return fieldlength;
        }
    }
}
