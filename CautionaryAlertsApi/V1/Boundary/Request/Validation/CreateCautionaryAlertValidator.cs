using CautionaryAlertsApi.V1.Infrastructure;
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
            RuleFor(x => x.IncidentDescription)
                .NotEmpty().NotNull()
                .NotXssString()
                .When(x => !string.IsNullOrEmpty(x.IncidentDescription))
                .Must(x => x.Length <= CreateCautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH);

            RuleFor(x => x.IncidentDate).NotEmpty().NotNull()
                .Must(date => date < DateTime.UtcNow)
                .NotEqual(DateTime.MinValue);

            RuleFor(x => x.AssureReference)
                .NotEmpty().NotNull()
                .NotXssString()
                .Must(x => x.Length <= CreateCautionaryAlertConstants.ASSUREREFERENCELENGTH);

            RuleFor(x => x.Alert).SetValidator(new AlertValidator());

            RuleFor(x => x.AssetDetails).SetValidator(new AssetDetailsValidator());

            RuleFor(x => x.PersonDetails).SetValidator(new PersonDetailsValidator());
        }
    }
}
