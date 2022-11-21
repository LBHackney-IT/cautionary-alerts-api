using FluentValidation;

namespace CautionaryAlertsApi.V1.Boundary.Request.Validation
{
    public class AlertValidator : AbstractValidator<Alert>
    {
        public AlertValidator()
        {
            RuleFor(x => x.Code).NotNull().NotEmpty();
        }
    }
}
