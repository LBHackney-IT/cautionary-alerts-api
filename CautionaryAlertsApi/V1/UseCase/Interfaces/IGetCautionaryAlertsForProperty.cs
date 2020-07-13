using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IGetCautionaryAlertsForProperty
    {
        CautionaryAlertsPropertyResponse Execute(string propertyReference);
    }
}
