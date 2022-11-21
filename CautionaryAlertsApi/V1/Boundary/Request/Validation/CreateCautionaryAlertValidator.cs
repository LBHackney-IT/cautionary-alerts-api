using FluentValidation;
using Hackney.Core.Validation;
using Microsoft.AspNetCore.Http;
using System;

namespace CautionaryAlertsApi.V1.Boundary.Request.Validation
{
    public class CreateCautionaryAlertValidator : AbstractValidator<CreateCautionaryAlert>
    {
        public CreateCautionaryAlertValidator()
        {
            RuleFor(x => x.IncidentDescription).NotXssString()
                .When(x => !string.IsNullOrEmpty(x.IncidentDescription));

            RuleFor(x => x.IncidentDescription).NotEmpty().NotNull();

            RuleFor(x => x.IncidentDate).NotEmpty().NotNull()
                .Must(date => date < DateTime.UtcNow)
                .NotEqual(DateTime.MinValue);

            RuleFor(x => x.Alert).SetValidator(new AlertValidator());

            RuleFor(x => x.AssetDetails).SetValidator(new AssetDetailsValidator());

            RuleFor(x => x.PersonDetails).SetValidator(new PersonDetailsValidator());
        }
    }
}
