using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigureOneFlag
{
    public static class C1Dictionaries
    {
        public static Dictionary<string, string> webmethods = new Dictionary<string, string>();
        public static Dictionary<string, int> fieldLengths = new Dictionary<string, int>();
        public static void LoadWMDictionary()
        {
            webmethods.Clear();
            webmethods.Add("getOrder", "http://nationaldev.conceptconfigurator.com/webservices/services/ConceptAccess?method=getOrder");
            webmethods.Add("updateOrder", "http://nationaldev.conceptconfigurator.com/webservices/services/ConceptAccess?method=updateOrder");
            webmethods.Add("getConfiguration", "http://nationaldev.conceptconfigurator.com/webservices/services/ConceptAccess?method=getConfiguration");
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
            fieldLengths.Add("CO:cust_name", 1024);
            fieldLengths.Add("CO:cust_ref_num", 15);
            fieldLengths.Add("CO:account_num", 15);
            fieldLengths.Add("CO:erp_reference_num", 15);
            fieldLengths.Add("CO:project", 1024);
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
            //COITEM Fields
            fieldLengths.Add("COItem:order_num", 15);
            fieldLengths.Add("COItem:ser_num", 15);
            fieldLengths.Add("COItem:item_num", 30);
            fieldLengths.Add("COItem:smartpart_num", 30);
            fieldLengths.Add("COItem:description", 40);
            //CfgItem Fields
            fieldLengths.Add("CfgItem:order_num", 15);
            fieldLengths.Add("CfgItem:smartpart_num", 40);
            fieldLengths.Add("CfgItem:item_num", 40);
            fieldLengths.Add("CfgItem:description", 40);
            fieldLengths.Add("CfgItem:uom", 10);
            //CfgParmVal Fields
            fieldLengths.Add("CfgParmVal:order_num", 15);
            fieldLengths.Add("CfgParmVal:name", 255);
            fieldLengths.Add("CfgParmVal:value", 1024);
            fieldLengths.Add("CfgParmVal:type", 15);
            fieldLengths.Add("CfgParmVal:label", 255);
        }
        public static int DBFieldLenCO(string field, ref zCfgCO co)
        {
            int fieldlength = (!fieldLengths.ContainsKey(field)) ? 15 : fieldLengths[field];
            switch (field)
            {
                case "CO:bill_to_phone_number": switch (co.BillToPhoneNumber.Length >= fieldlength) { case true: break; default: fieldlength = co.BillToPhoneNumber.Length; break; } break;
                case "CO:ship_via": switch (co.ShipVia.Length >= fieldlength) { case true: break; default: fieldlength = co.ShipVia.Length; break; } break;
                case "CO:shipping_terms": switch (co.ShippingTerms.Length >= fieldlength) { case true: break; default: fieldlength = co.ShippingTerms.Length; break; } break;
                case "CO:bill_to_fax_number": switch (co.BillToFaxNumber.Length >= fieldlength) { case true: break; default: fieldlength = co.BillToFaxNumber.Length; break; } break;
            }
            return fieldlength;
        }
        public static int DBFieldLenCOItem(string field, ref zCfgCOitem coitem)
        {
            int fieldlength = (!fieldLengths.ContainsKey(field)) ? 15 : fieldLengths[field];
            switch (field)
            {
                case "COItem:ser_num": switch (coitem.Serial.Length >= fieldlength) { case true: break; default: fieldlength = coitem.Serial.Length; break; } break;
                case "COItem:item_num": switch (coitem.Item.Length >= fieldlength) { case true: break; default: fieldlength = coitem.Item.Length; break; } break;
                case "COItem:smartpart_num": switch (coitem.Smartpart.Length >= fieldlength) { case true: break; default: fieldlength = coitem.Smartpart.Length; break; } break;
                case "COItem:description": switch (coitem.Desc.Length >= fieldlength) { case true: break; default: fieldlength = coitem.Desc.Length; break; } break;
            }
            return fieldlength;
        }
        public static int DBFieldLenCfgItem(string field, ref zCfgItem citem)
        {
            int fieldlength = (!fieldLengths.ContainsKey(field)) ? 15 : fieldLengths[field];
            switch (field)
            {
                case "CfgItem:smartpart_num": switch (citem.Smartpart.Length >= fieldlength) { case true: break; default: fieldlength = citem.Smartpart.Length; break; } break;
                case "CfgItem:item_num": switch (citem.Item.Length >= fieldlength) { case true: break; default: fieldlength = citem.Item.Length; break; } break;
                case "CfgItem:description": switch (citem.Desc.Length >= fieldlength) { case true: break; default: fieldlength = citem.Desc.Length; break; } break;
                case "CfgItem:uom": switch (citem.UnitOfMeasure.Length >= fieldlength) { case true: break; default: fieldlength = citem.UnitOfMeasure.Length; break; } break;
            }
            return fieldlength;
        }
        public static int DBFieldLenParmVal(string field, ref zCfgParmVal cfg)
        {
            int fieldlength = (!fieldLengths.ContainsKey(field)) ? 15 : fieldLengths[field];
            switch (field)
            {
                case "CfgParmVal:name": switch (cfg.CName.Length >= fieldlength) { case true: break; default: fieldlength = cfg.CName.Length; break; } break;
                case "CfgParmVal:value": switch (cfg.CValue.Length >= fieldlength) { case true: break; default: fieldlength = cfg.CValue.Length; break; } break;
                case "CfgParmVal:type": switch (cfg.CType.Length >= fieldlength) { case true: break; default: fieldlength = cfg.CType.Length; break; } break;
                case "CfgParmVal:label": switch (cfg.CLabel.Length >= fieldlength) { case true: break; default: fieldlength = cfg.CLabel.Length; break; } break;
            }
            return fieldlength;
        }
        public static int DBFieldLenBOM(string field, ref zCfgBOM bom)
        {
            int fieldlength = (!fieldLengths.ContainsKey(field)) ? 15 : fieldLengths[field];
            switch (field)
            {
                case "BOM:id": switch (bom.Identifier.Length >= fieldlength) { case true: break; default: fieldlength = bom.Identifier.Length; break; } break;
                case "BOM:item_num": switch (bom.Item.Length >= fieldlength) { case true: break; default: fieldlength = bom.Item.Length; break; } break;
                case "BOM:smartpart_num": switch (bom.Smartpart.Length >= fieldlength) { case true: break; default: fieldlength = bom.Smartpart.Length; break; } break;
            }
            return fieldlength;
        }
    }
}
