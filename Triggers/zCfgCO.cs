using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Accessor class for CO object
    /// </summary>
    public class zCfgCO
    {
        private string id;
        private string order_num;
        private string order_ref_num;
        private string cust_name;
        private string cust_ref_num;
        private string account_num;
        private string erp_reference_num;
        private string project;
        private string payment_terms;
        private string ship_via;
        private string shipping_terms;
        private string bill_to_contact_name;
        private string bill_address_line_1;
        private string bill_address_line_2;
        private string bill_address_line_3;
        private string bill_to_city;
        private string bill_to_state;
        private string bill_to_country;
        private string bill_to_postal_code;
        private string bill_to_phone_number;
        private string bill_to_fax_number;
        private string bill_to_email_address;
        private string bill_to_ref_num;
        private string ship_to_contact_name;
        private string ship_to_address_line_1;
        private string ship_to_address_line_2;
        private string ship_to_address_line_3;
        private string ship_to_address_line_4;
        private string ship_to_city;
        private string ship_to_state;
        private string ship_to_country;
        private string ship_to_postal_code;
        private string ship_to_phone_number;
        private string ship_to_fax_number;
        private string ship_to_email_address;
        private string ship_to_ref_num;
        private int priority_level;
        private DateTime due_date;
        private string cust_po;
        private string freight_terms;
        private string freight_acct;
        private string quote_nbr;
        private string web_user_name;
        private DateTime web_order_date;
        private string dropship_address1;
        private string dropship_address2;
        private string dropship_address3;
        private string dropship_address4;
        private string dropship_city;
        private string dropship_state;
        private string dropship_zip;
        private string dropship_name;
        private string dropship_contact;
        private string dropship_country;
        private string dropship_phone;
        private string dropship_email;
        private DateTime request_date;
        private string destination_country;
        private string orderheader_notes;
        private string end_user;
        private string engineer;
        public string Identifier
        {
            get { return id; }
            set { id = value; }
        }
        public string CO_Num
        {
            get { return order_num; }
            set { order_num = value; }
        }
        public string CORefNum
        {
            get { return order_ref_num; }
            set { order_ref_num = value; }
        }
        public string CustName
        {
            get { return cust_name; }
            set { cust_name = value; }
        }
        public string CustRefNum
        {
            get { return cust_ref_num; }
            set { cust_ref_num = value; }
        }
        public string AccountNum
        {
            get { return account_num; }
            set { account_num = value; }
        }
        public string ErpReferenceNum
        {
            get { return erp_reference_num; }
            set { erp_reference_num = value; }
        }
        public string Project
        {
            get { return project; }
            set { project = value; }
        }
        public string PaymentTerms
        {
            get { return payment_terms; }
            set { payment_terms = value; }
        }
        public string ShipVia
        {
            get { return ship_via; }
            set { ship_via = value; }
        }
        public string ShippingTerms
        {
            get { return shipping_terms; }
            set { shipping_terms = value; }
        }
        public string BillToContactName
        {
            get { return bill_to_contact_name; }
            set { bill_to_contact_name = value; }
        }
        public string BillToAddressLine1
        {
            get { return bill_address_line_1; }
            set { bill_address_line_1 = value; }
        }
        public string BillToAddressLine2
        {
            get { return bill_address_line_2; }
            set { bill_address_line_2 = value; }
        }
        public string BillToAddressLine3
        {
            get { return bill_address_line_3; }
            set { bill_address_line_3 = value; }
        }
        public string BillToCity
        {
            get { return bill_to_city; }
            set { bill_to_city = value; }
        }
        public string BillToState
        {
            get { return bill_to_state; }
            set { bill_to_state = value; }
        }
        public string BillToCountry
        {
            get { return bill_to_country; }
            set { bill_to_country = value; }
        }
        public string BillToPostalCode
        {
            get { return bill_to_postal_code; }
            set { bill_to_postal_code = value; }
        }
        public string BillToPhoneNumber
        {
            get { return bill_to_phone_number; }
            set { bill_to_phone_number = value; }
        }
        public string BillToFaxNumber
        {
            get { return bill_to_fax_number; }
            set { bill_to_fax_number = value; }
        }
        public string BillToEmailAddress
        {
            get { return bill_to_email_address; }
            set { bill_to_email_address = value; }
        }
        public string BillToRefNum
        {
            get { return bill_to_ref_num; }
            set { bill_to_ref_num = value; }
        }
        public string ShipToContactName
        {
            get { return ship_to_contact_name; }
            set { ship_to_contact_name = value; }
        }
        public string ShipToAddressLine1
        {
            get { return ship_to_address_line_1; }
            set { ship_to_address_line_1 = value; }
        }
        public string ShipToAddressLine2
        {
            get { return ship_to_address_line_2; }
            set { ship_to_address_line_2 = value; }
        }
        public string ShipToAddressLine3
        {
            get { return ship_to_address_line_3; }
            set { ship_to_address_line_3 = value; }
        }
        public string ShipToAddressLine4
        {
            get { return ship_to_address_line_4; }
            set { ship_to_address_line_4 = value; }
        }
        public string ShipToCity
        {
            get { return ship_to_city; }
            set { ship_to_city = value; }
        }
        public string ShipToState
        {
            get { return ship_to_state; }
            set { ship_to_state = value; }
        }
        public string ShipToCountry
        {
            get { return ship_to_country; }
            set { ship_to_country = value; }
        }
        public string ShipToPostalCode
        {
            get { return ship_to_postal_code; }
            set { ship_to_postal_code = value; }
        }
        public string ShipToPhoneNumber
        {
            get { return ship_to_phone_number; }
            set { ship_to_phone_number = value; }
        }
        public string ShipToFaxNumber
        {
            get { return ship_to_fax_number; }
            set { ship_to_fax_number = value; }
        }
        public string ShipToEmailAddress
        {
            get { return ship_to_email_address; }
            set { ship_to_email_address = value; }
        }
        public string ShipToRefNum
        {
            get { return ship_to_ref_num; }
            set { ship_to_ref_num = value; }
        }
        public int PriorityLevel
        {
            get { return priority_level; }
            set { priority_level = value; }
        }
        public DateTime DueDate
        {
            get { return due_date; }
            set { due_date = value; }
        }
        public string CustPO
        {
            get { return cust_po; }
            set { cust_po = value; }
        }
        public string FreightTerms
        {
            get { return freight_terms; }
            set { freight_terms = value; }
        }
        public string FreightAcct
        {
            get { return freight_acct; }
            set { freight_acct = value; }
        }
        public string QuoteNbr
        {
            get { return quote_nbr; }
            set { quote_nbr = value; }
        }
        public string WebUserName
        {
            get { return web_user_name; }
            set { web_user_name = value; }
        }
        public DateTime WebOrderDate
        {
            get { return web_order_date; }
            set { web_order_date = value; }
        }
        public string DropShipAddress1
        {
            get { return dropship_address1; }
            set { dropship_address1 = value; }
        }
        public string DropShipAddress2
        {
            get { return dropship_address2; }
            set { dropship_address2 = value; }
        }
        public string DropShipAddress3
        {
            get { return dropship_address3; }
            set { dropship_address3 = value; }
        }
        public string DropShipAddress4
        {
            get { return dropship_address4; }
            set { dropship_address4 = value; }
        }
        public string DropShipCity
        {
            get { return dropship_city; }
            set { dropship_city = value; }
        }
        public string DropShipState
        {
            get { return dropship_state; }
            set { dropship_state = value; }
        }
        public string DropShipZip
        {
            get { return dropship_zip; }
            set { dropship_zip = value; }
        }
        public string DropShipName
        {
            get { return dropship_name; }
            set { dropship_name = value; }
        }
        public string DropShipContact
        {
            get { return dropship_contact; }
            set { dropship_contact = value; }
        }
        public string DropShipCountry
        {
            get { return dropship_country; }
            set { dropship_country = value; }
        }
        public string DropShipPhone
        {
            get { return dropship_phone; }
            set { dropship_phone = value; }
        }
        public string DropShipEmail
        {
            get { return dropship_email; }
            set { dropship_email = value; }
        }
        public DateTime RequestDate
        {
            get { return request_date; }
            set { request_date = value; }
        }
        public string DestinationCountry
        {
            get { return destination_country; }
            set { destination_country = value; }
        }
        public string OrderHeaderNotes
        {
            get { return orderheader_notes; }
            set { orderheader_notes = value; }
        }
        public string EndUser
        {
            get { return end_user; }
            set { end_user = value; }
        }
        public string Engineer
        {
            get { return engineer; }
            set { engineer = value; }
        }
        public static void ClearCO(ref zCfgCO co)
        {
            co.BillToAddressLine1 = "";
            co.BillToAddressLine2 = "";
            co.BillToAddressLine3 = "";
            co.BillToCity = "";
            co.BillToContactName = "";
            co.BillToCountry = "";
            co.BillToEmailAddress = "";
            co.BillToFaxNumber = "";
            co.BillToPhoneNumber = "";
            co.BillToPostalCode = "";
            co.BillToRefNum = "";
            co.BillToState = "";
            co.ShipToAddressLine1 = "";
            co.ShipToAddressLine2 = "";
            co.ShipToAddressLine3 = "";
            co.ShipToAddressLine4 = "";
            co.ShipToCity = "";
            co.ShipToContactName = "";
            co.ShipToCountry = "";
            co.ShipToEmailAddress = "";
            co.ShipToFaxNumber = "";
            co.ShipToPhoneNumber = "";
            co.ShipToPostalCode = "";
            co.ShipToRefNum = "";
            co.ShipToState = "";
            co.DropShipAddress1 = "";
            co.DropShipAddress2 = "";
            co.DropShipAddress3 = "";
            co.DropShipAddress4 = "";
            co.DropShipCity = "";
            co.DropShipContact = "";
            co.DropShipCountry = "";
            co.DropShipEmail = "";
            co.DropShipName = "";
            co.DropShipPhone = "";
            co.DropShipState = "";
            co.DropShipZip = "";
            co.CustPO = "";
            co.Project = "";
            co.WebUserName = "";
            co.Engineer = "";
            co.EndUser = "";
            co.ShippingTerms = "";
            co.ShipVia = "";
            co.FreightAcct = "";
            co.FreightTerms = "";
            co.ErpReferenceNum = "";
            co.CustName = "";
            co.PaymentTerms = "";
            co.DestinationCountry = "";
            co.OrderHeaderNotes = "";
            co.QuoteNbr = "";
        }
    }
}
