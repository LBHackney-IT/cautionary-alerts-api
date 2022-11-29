using CautionaryAlertsApi.V1.Infrastructure;
using FluentValidation;

namespace CautionaryAlertsApi.V1.Boundary.Request.Validation
{
    public class AlertValidator : AbstractValidator<Alert>
    {
        public AlertValidator()
        {
            RuleFor(x => x.Code).NotNull().NotEmpty()
                .Must(x => x.Length <= CreateCautionaryAlertConstants.ALERTCODELENGTH);
            RuleFor(x => x.Code).NotNull().NotEmpty()
                .Must(x => x.Length <= CreateCautionaryAlertConstants.ALERTDESCRIPTION);
        }
    }
}
