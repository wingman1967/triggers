using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation;
using FluentValidation.Results;
using ConfigureOneFlag;

namespace Triggers
{
    class COItemValidator : AbstractValidator<zCfgCOitem>
    {
        public void ConfigType()
        {
            RuleFor(x => x.ConfigType).Length(1);
            ValidationResult results = this.Validate(StagingUtilities.coitemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coitemValidator.ConfigType = StagingUtilities.coitemValidator.ConfigType.Substring(0, 1); Audit.ValidationMessages += results.ToString(); }
        }
        public void CO_Num()
        {
            RuleFor(x => x.CO_Num).Length(0, 15);
            ValidationResult results = this.Validate(StagingUtilities.coitemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coitemValidator.CO_Num = StagingUtilities.coitemValidator.CO_Num.Substring(0, 15); Audit.ValidationMessages += results.ToString(); }
        }
        public void Serial()
        {
            RuleFor(x => x.Serial).Length(0, 15);
            ValidationResult results = this.Validate(StagingUtilities.coitemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coitemValidator.Serial = StagingUtilities.coitemValidator.Serial.Substring(0, 15); Audit.ValidationMessages += results.ToString(); }
        }
        public void Item()
        {
            RuleFor(x => x.Item).Length(0, 30);
            ValidationResult results = this.Validate(StagingUtilities.coitemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coitemValidator.Item = StagingUtilities.coitemValidator.Item.Substring(0, 30); Audit.ValidationMessages += results.ToString(); }
        }
        public void Smartpart()
        {
            RuleFor(x => x.Smartpart).Length(0, 30);
            ValidationResult results = this.Validate(StagingUtilities.coitemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coitemValidator.Smartpart = StagingUtilities.coitemValidator.Smartpart.Substring(0, 30); Audit.ValidationMessages += results.ToString(); }
        }
        public void Desc()
        {
            RuleFor(x => x.Desc).Length(0, 1000);
            ValidationResult results = this.Validate(StagingUtilities.coitemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coitemValidator.Desc = StagingUtilities.coitemValidator.Desc.Substring(0, 1000); Audit.ValidationMessages += results.ToString(); }
        }
        public void CustPO()
        {
            RuleFor(x => x.CustPO).Length(0, 22);
            ValidationResult results = this.Validate(StagingUtilities.coitemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coitemValidator.CustPO = StagingUtilities.coitemValidator.CustPO.Substring(0, 22); Audit.ValidationMessages += results.ToString(); }
        }
        public void OrderLineNotes()
        {
            RuleFor(x => x.OrderLineNotes).Length(0, 1000);
            ValidationResult results = this.Validate(StagingUtilities.coitemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.coitemValidator.OrderLineNotes = StagingUtilities.coitemValidator.OrderLineNotes.Substring(0, 1000); Audit.ValidationMessages += results.ToString(); }
        }
        public void ValidateCOItem()
        {
            ConfigType();
            CO_Num();
            Serial();
            Item();
            Smartpart();
            Desc();
            CustPO();
            OrderLineNotes();
        }
    }
}
