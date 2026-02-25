using FluentValidation;
using System;

namespace CautionaryAlertsApi.V1.Boundary.Request.Validation
{
    public class EndCautionaryAlertValidator : AbstractValidator<EndCautionaryAlertRequest>
    {
        public EndCautionaryAlertValidator()
        {
            RuleFor(x => x.EndDate).NotNull().LessThanOrEqualTo(DateTime.UtcNow);
        }
    }
}
