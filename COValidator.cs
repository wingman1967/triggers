using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfigureOneFlag;
using FluentValidation;
using FluentValidation.Results;

namespace Triggers
{
    public class COValidator: AbstractValidator<zCfgCO>
    {
        public void BillToAddress1()
        {
            RuleFor(x => x.BillToAddressLine1).Length(1, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.BillToAddressLine1 = StagingUtilities.coValidator.BillToAddressLine1.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void BillToAddress2()
        {
            RuleFor(x => x.BillToAddressLine2).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.BillToAddressLine2 = StagingUtilities.coValidator.BillToAddressLine2.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void BillToAddress3()
        {
            RuleFor(x => x.BillToAddressLine3).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.BillToAddressLine3 = StagingUtilities.coValidator.BillToAddressLine3.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void CustName()
        {
            RuleFor(x => x.CustName).Length(1, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.CustName = StagingUtilities.coValidator.CustName.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void ErpRefNum()
        {
            RuleFor(x => x.ErpReferenceNum).Length(0, 15);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ErpReferenceNum = StagingUtilities.coValidator.ErpReferenceNum.Substring(0, 15); Audit.ValidationMessages += results.ToString(); }
        }
        public void Project()
        {
            RuleFor(x => x.Project).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.Project = StagingUtilities.coValidator.Project.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void PaymentTerms()
        {
            RuleFor(x => x.PaymentTerms).Length(0, 15);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.PaymentTerms = StagingUtilities.coValidator.PaymentTerms.Substring(0, 15); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipVia()
        {
            RuleFor(x => x.ShipVia).Length(0, 15);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipVia = StagingUtilities.coValidator.ShipVia.Substring(0, 15); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShippingTerms()
        {
            RuleFor(x => x.ShippingTerms).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShippingTerms = StagingUtilities.coValidator.ShippingTerms.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void BillToContactName()
        {
            RuleFor(x => x.BillToContactName).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.BillToContactName = StagingUtilities.coValidator.BillToContactName.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void BillToCity()
        {
            RuleFor(x => x.BillToCity).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.BillToCity = StagingUtilities.coValidator.BillToCity.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void BillToState()
        {
            RuleFor(x => x.BillToState).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.BillToState = StagingUtilities.coValidator.BillToState.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void BillToCountry()
        {
            RuleFor(x => x.BillToCountry).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.BillToCountry = StagingUtilities.coValidator.BillToCountry.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void BillToPostalCode()
        {
            RuleFor(x => x.BillToPostalCode).Length(0, 15);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.BillToPostalCode = StagingUtilities.coValidator.BillToPostalCode.Substring(0, 15); Audit.ValidationMessages += results.ToString(); }
        }
        public void BillToPhoneNumber()
        {
            RuleFor(x => x.BillToPhoneNumber).Length(0, 25);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.BillToPhoneNumber = StagingUtilities.coValidator.BillToPhoneNumber.Substring(0, 25); Audit.ValidationMessages += results.ToString(); }
        }
        public void BillToFaxNumber()
        {
            RuleFor(x => x.BillToFaxNumber).Length(0, 25);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.BillToFaxNumber = StagingUtilities.coValidator.BillToFaxNumber.Substring(0, 25); Audit.ValidationMessages += results.ToString(); }
        }
        public void BillToEmailAddress()
        {
            RuleFor(x => x.BillToEmailAddress).Length(0, 60);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.BillToEmailAddress = StagingUtilities.coValidator.BillToEmailAddress.Substring(0, 60); Audit.ValidationMessages += results.ToString(); }
        }
        public void BillToRefNum()
        {
            RuleFor(x => x.BillToRefNum).Length(0, 15);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.BillToRefNum = StagingUtilities.coValidator.BillToRefNum.Substring(0, 15); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToContactName()
        {
            RuleFor(x => x.ShipToContactName).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToContactName = StagingUtilities.coValidator.ShipToContactName.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToAddressLine1()
        {
            RuleFor(x => x.ShipToAddressLine1).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToAddressLine1 = StagingUtilities.coValidator.ShipToAddressLine1.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToAddressLine2()
        {
            RuleFor(x => x.ShipToAddressLine2).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToAddressLine2 = StagingUtilities.coValidator.ShipToAddressLine2.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToAddressLine3()
        {
            RuleFor(x => x.ShipToAddressLine3).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToAddressLine3 = StagingUtilities.coValidator.ShipToAddressLine3.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToAddressLine4()
        {
            RuleFor(x => x.ShipToAddressLine4).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToAddressLine4 = StagingUtilities.coValidator.ShipToAddressLine4.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToCity()
        {
            RuleFor(x => x.ShipToCity).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToCity = StagingUtilities.coValidator.ShipToCity.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToState()
        {
            RuleFor(x => x.ShipToState).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToState = StagingUtilities.coValidator.ShipToState.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToCountry()
        {
            RuleFor(x => x.ShipToCountry).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToCountry = StagingUtilities.coValidator.ShipToCountry.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToPostalCode()
        {
            RuleFor(x => x.ShipToPostalCode).Length(0, 15);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToPostalCode = StagingUtilities.coValidator.ShipToPostalCode.Substring(0, 15); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToPhoneNumber()
        {
            RuleFor(x => x.ShipToPhoneNumber).Length(0, 25);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToPhoneNumber = StagingUtilities.coValidator.ShipToPhoneNumber.Substring(0, 25); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToFaxNumber()
        {
            RuleFor(x => x.ShipToFaxNumber).Length(0, 25);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToFaxNumber = StagingUtilities.coValidator.ShipToFaxNumber.Substring(0, 25); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToEmailAddress()
        {
            RuleFor(x => x.ShipToEmailAddress).Length(0, 60);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToEmailAddress = StagingUtilities.coValidator.ShipToEmailAddress.Substring(0, 60); Audit.ValidationMessages += results.ToString(); }
        }
        public void ShipToRefNum()
        {
            RuleFor(x => x.ShipToRefNum).Length(0, 15);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.ShipToRefNum = StagingUtilities.coValidator.ShipToRefNum.Substring(0, 15); Audit.ValidationMessages += results.ToString(); }
        }
        public void CustPO()
        {
            RuleFor(x => x.CustPO).Length(0, 22);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.CustPO = StagingUtilities.coValidator.CustPO.Substring(0, 22); Audit.ValidationMessages += results.ToString(); }
        }
        public void FreightTerms()
        {
            RuleFor(x => x.FreightTerms).Length(0, 100);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.FreightTerms = StagingUtilities.coValidator.FreightTerms.Substring(0, 100); Audit.ValidationMessages += results.ToString(); }
        }
        public void FreightAccount()
        {
            RuleFor(x => x.FreightAcct).Length(0, 100);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.FreightAcct = StagingUtilities.coValidator.FreightAcct.Substring(0, 100); Audit.ValidationMessages += results.ToString(); }
        }
        public void QuoteNbr()
        {
            RuleFor(x => x.QuoteNbr).Length(0, 10);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.QuoteNbr = StagingUtilities.coValidator.QuoteNbr.Substring(0, 10); Audit.ValidationMessages += results.ToString(); }
        }
        public void WebUserName()
        {
            RuleFor(x => x.WebUserName).Length(0, 30);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.WebUserName = StagingUtilities.coValidator.WebUserName.Substring(0, 30); Audit.ValidationMessages += results.ToString(); }
        }
        public void DropShipAddress1()
        {
            RuleFor(x => x.DropShipAddress1).Length(0, 50);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DropShipAddress1 = StagingUtilities.coValidator.DropShipAddress1.Substring(0, 50); Audit.ValidationMessages += results.ToString(); }
        }
        public void DropShipAddress2()
        {
            RuleFor(x => x.DropShipAddress2).Length(0, 50);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DropShipAddress2 = StagingUtilities.coValidator.DropShipAddress2.Substring(0, 50); Audit.ValidationMessages += results.ToString(); }
        }
        public void DropShipAddress3()
        {
            RuleFor(x => x.DropShipAddress3).Length(0, 50);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DropShipAddress3 = StagingUtilities.coValidator.DropShipAddress3.Substring(0, 50); Audit.ValidationMessages += results.ToString(); }
        }
        public void DropShipAddress4()
        {
            RuleFor(x => x.DropShipAddress4).Length(0, 50);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DropShipAddress4 = StagingUtilities.coValidator.DropShipAddress4.Substring(0, 50); Audit.ValidationMessages += results.ToString(); }
        }
        public void DropShipCity()
        {
            RuleFor(x => x.DropShipCity).Length(0, 30);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DropShipCity = StagingUtilities.coValidator.DropShipCity.Substring(0, 30); Audit.ValidationMessages += results.ToString(); }
        }
        public void DropShipState()
        {
            RuleFor(x => x.DropShipState).Length(0, 5);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DropShipState = StagingUtilities.coValidator.DropShipState.Substring(0, 5); Audit.ValidationMessages += results.ToString(); }
        }
        public void DropShipZip()
        {
            RuleFor(x => x.DropShipZip).Length(0, 10);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DropShipZip = StagingUtilities.coValidator.DropShipZip.Substring(0, 10); Audit.ValidationMessages += results.ToString(); }
        }
        public void DropShipName()
        {
            RuleFor(x => x.DropShipName).Length(0, 60);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DropShipName = StagingUtilities.coValidator.DropShipName.Substring(0, 60); Audit.ValidationMessages += results.ToString(); }
        }
        public void DropShipContact()
        {
            RuleFor(x => x.DropShipContact).Length(0, 30);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DropShipContact = StagingUtilities.coValidator.DropShipContact.Substring(0, 30); Audit.ValidationMessages += results.ToString(); }
        }
        public void DropShipCountry()
        {
            RuleFor(x => x.DropShipCountry).Length(0, 30);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DropShipCountry = StagingUtilities.coValidator.DropShipCountry.Substring(0, 30); Audit.ValidationMessages += results.ToString(); }
        }
        public void DropShipPhone()
        {
            RuleFor(x => x.DropShipPhone).Length(0, 25);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DropShipPhone = StagingUtilities.coValidator.DropShipPhone.Substring(0, 25); Audit.ValidationMessages += results.ToString(); }
        }
        public void DropShipEmail()
        {
            RuleFor(x => x.DropShipEmail).Length(0, 25);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DropShipEmail = StagingUtilities.coValidator.DropShipEmail.Substring(0, 25); Audit.ValidationMessages += results.ToString(); }
        }
        public void DestinationCountry()
        {
            RuleFor(x => x.DestinationCountry).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.DestinationCountry = StagingUtilities.coValidator.DestinationCountry.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void OrderHeaderNotes()
        {
            RuleFor(x => x.OrderHeaderNotes).Length(0, 1000);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.OrderHeaderNotes = StagingUtilities.coValidator.OrderHeaderNotes.Substring(0, 1000); Audit.ValidationMessages += results.ToString(); }
        }
        public void EndUser()
        {
            RuleFor(x => x.EndUser).Length(0, 100);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.EndUser = StagingUtilities.coValidator.EndUser.Substring(0, 100); Audit.ValidationMessages += results.ToString(); }
        }
        public void Engineer()
        {
            RuleFor(x => x.Engineer).Length(0, 100);
            ValidationResult results = this.Validate(StagingUtilities.coValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coValidator.Engineer = StagingUtilities.coValidator.Engineer.Substring(0, 100); Audit.ValidationMessages += results.ToString(); }
        }
        public void ValidateCO()
        {
            BillToAddress1();
            BillToAddress2();
            BillToAddress3();
            BillToCity();
            BillToContactName();
            BillToCountry();
            BillToEmailAddress();
            BillToFaxNumber();
            BillToPhoneNumber();
            BillToPostalCode();
            BillToRefNum();
            BillToState();
            ShipToAddressLine1();
            ShipToAddressLine2();
            ShipToAddressLine3();
            ShipToAddressLine4();
            ShipToCity();
            ShipToContactName();
            ShipToCountry();
            ShipToEmailAddress();
            ShipToFaxNumber();
            ShipToPhoneNumber();
            ShipToPostalCode();
            ShipToRefNum();
            ShipToState();
            DropShipAddress1();
            DropShipAddress2();
            DropShipAddress3();
            DropShipAddress4();
            DropShipCity();
            DropShipContact();
            DropShipCountry();
            DropShipEmail();
            DropShipName();
            DropShipPhone();
            DropShipState();
            DropShipZip();
            CustPO();
            Project();
            WebUserName();
            Engineer();
            EndUser();
            ShippingTerms();
            ShipVia();
            FreightAccount();
            FreightTerms();
            ErpRefNum();
            CustName();
            PaymentTerms();
            DestinationCountry();
            OrderHeaderNotes();
            QuoteNbr();
        }
    }
}
