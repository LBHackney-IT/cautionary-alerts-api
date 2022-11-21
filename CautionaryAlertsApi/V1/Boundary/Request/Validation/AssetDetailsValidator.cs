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
                .NotXssString();
        }
    }
}
