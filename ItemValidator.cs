using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfigureOneFlag;
using FluentValidation;
using FluentValidation.Results;

namespace Triggers
{
    class ItemValidator : AbstractValidator<zCfgItem>
    {
        public void CO_Num()
        {
            RuleFor(x => x.CO_Num).Length(0, 15);
            ValidationResult results = this.Validate(StagingUtilities.citemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.citemValidator.CO_Num = StagingUtilities.citemValidator.CO_Num.Substring(0, 15); Audit.ValidationMessages += results.ToString(); }
        }
        public void Smartpart()
        {
            RuleFor(x => x.Smartpart).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.citemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.citemValidator.CO_Num = StagingUtilities.citemValidator.CO_Num.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void Item()
        {
            RuleFor(x => x.Item).Length(0, 40);
            ValidationResult results = this.Validate(StagingUtilities.citemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.citemValidator.Item = StagingUtilities.citemValidator.Item.Substring(0, 40); Audit.ValidationMessages += results.ToString(); }
        }
        public void Desc()
        {
            RuleFor(x => x.Desc).Length(0, 1000);
            ValidationResult results = this.Validate(StagingUtilities.citemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.citemValidator.Desc = StagingUtilities.citemValidator.Desc.Substring(0, 1000); Audit.ValidationMessages += results.ToString(); }
        }
        public void UnitOfMeasure()
        {
            RuleFor(x => x.UnitOfMeasure).Length(0, 10);
            ValidationResult results = this.Validate(StagingUtilities.citemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.citemValidator.UnitOfMeasure = StagingUtilities.citemValidator.UnitOfMeasure.Substring(0, 10); Audit.ValidationMessages += results.ToString(); }
        }
        public void IM_VAR1()
        {
            RuleFor(x => x.IM_VAR1).Length(0, 200);
            ValidationResult results = this.Validate(StagingUtilities.citemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.citemValidator.IM_VAR1 = StagingUtilities.citemValidator.IM_VAR1.Substring(0, 200); Audit.ValidationMessages += results.ToString(); }
        }
        public void IM_VAR2()
        {
            RuleFor(x => x.IM_VAR2).Length(0, 200);
            ValidationResult results = this.Validate(StagingUtilities.citemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.citemValidator.IM_VAR2 = StagingUtilities.citemValidator.IM_VAR2.Substring(0, 200); Audit.ValidationMessages += results.ToString(); }
        }
        public void IM_VAR3()
        {
            RuleFor(x => x.IM_VAR3).Length(0, 200);
            ValidationResult results = this.Validate(StagingUtilities.citemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.citemValidator.IM_VAR3 = StagingUtilities.citemValidator.IM_VAR3.Substring(0, 200); Audit.ValidationMessages += results.ToString(); }
        }
        public void IM_VAR4()
        {
            RuleFor(x => x.IM_VAR4).Length(0, 200);
            ValidationResult results = this.Validate(StagingUtilities.citemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.citemValidator.IM_VAR4 = StagingUtilities.citemValidator.IM_VAR4.Substring(0, 200); Audit.ValidationMessages += results.ToString(); }
        }
        public void IM_VAR5()
        {
            RuleFor(x => x.IM_VAR5).Length(0, 200);
            ValidationResult results = this.Validate(StagingUtilities.citemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.citemValidator.IM_VAR5 = StagingUtilities.citemValidator.IM_VAR5.Substring(0, 200); Audit.ValidationMessages += results.ToString(); }
        }
        public void IM_VAR6()
        {
            RuleFor(x => x.IM_VAR6).Length(0, 200);
            ValidationResult results = this.Validate(StagingUtilities.citemValidator);
            if (results.Errors.Count > 0) { StagingUtilities.citemValidator.IM_VAR6 = StagingUtilities.citemValidator.IM_VAR6.Substring(0, 200); Audit.ValidationMessages += results.ToString(); }
        }
        public void ValidateCitem()
        {
            CO_Num();
            Smartpart();
            Item();
            Desc();
            UnitOfMeasure();
            IM_VAR1();
            IM_VAR2();
            IM_VAR3();
            IM_VAR4();
            IM_VAR5();
            IM_VAR6();
        }
    }
}
