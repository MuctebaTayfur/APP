using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Infra.Base.Validations.FluentValidation
{
    public class NullReferenceAbstractValidator<T>:AbstractValidator<T>
    {
        protected override bool PreValidate(ValidationContext<T> context, ValidationResult result)
        {
            if (context.InstanceToValidate==null)
            {
                result.Errors.Add(new ValidationFailure("", "Model must not be null"));
                return false;
            }
            return true;
        }
    }
}
