using CautionaryAlertsApi.V1.Infrastructure;
using FluentValidation;
using Hackney.Core.Validation;
using System;

namespace CautionaryAlertsApi.V1.Boundary.Request.Validation
{
    public class AssetDetailsValidator : AbstractValidator<AssetDetails>
    {
        public AssetDetailsValidator()
        {
            RuleFor(x => x.Id).NotNull()
                .NotEqual(Guid.Empty);

            RuleFor(x => x.FullAddress)
                .NotEmpty().NotNull()
                .NotXssString()
                .Must(x => x.Length <= CreateCautionaryAlertConstants.FULLADDRESSLENGTH);

            RuleFor(x => x.PropertyReference)
                .NotNull().NotEmpty()
                .NotXssString()
                .Must(x => x.Length <= CreateCautionaryAlertConstants.PROPERTYREFERENCELENGTH);

            RuleFor(x => x.UPRN)
                .NotNull().NotEmpty()
                .NotXssString()
                .Must(x => x.Length <= CreateCautionaryAlertConstants.UPRNLENGTH);
        }
    }
}
