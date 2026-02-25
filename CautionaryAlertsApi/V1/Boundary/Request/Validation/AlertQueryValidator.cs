using FluentValidation;

namespace CautionaryAlertsApi.V1.Boundary.Request.Validation
{
    public class AlertQueryValidator : AbstractValidator<AlertQueryObject>
    {
        public AlertQueryValidator()
        {
            RuleFor(x => x.AlertId).NotNull().NotEmpty();
        }
    }
}
