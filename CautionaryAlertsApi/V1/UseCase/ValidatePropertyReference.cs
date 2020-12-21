using CautionaryAlertsApi.V1.UseCase.Interfaces;
using System.Text.RegularExpressions;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class ValidatePropertyReference : IValidatePropertyReference
    {
        public bool Execute(string propertyRef)
        {
            const string pattern = @"^([A-Za-z]{0,2}\d{7,12})$";

            return Regex.IsMatch(propertyRef, pattern);
        }
    }
}
