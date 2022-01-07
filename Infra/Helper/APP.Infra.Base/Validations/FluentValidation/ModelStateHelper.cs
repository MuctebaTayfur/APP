using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Infra.Base.Validations.FluentValidation
{
    public static class ModelStateHelper
    {
        public static List<string> GetErrors(this ModelStateDictionary modelState)
        {
            return modelState.Values.Where(v => v.Errors.Count > 0)
             .SelectMany(v => v.Errors)
             .Select(v => v.ErrorMessage)
             .ToList();
        }
    }
}
