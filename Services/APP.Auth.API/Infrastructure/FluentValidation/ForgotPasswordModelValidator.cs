using APP.Auth.Model.Model;
using APP.Infra.Base.Validations.FluentValidation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APP.Auth.API.Infrastructure.FluentValidation
{
    public class ForgotPasswordModelValidator:NullReferenceAbstractValidator<ForgotPasswordModel>
    {
        public ForgotPasswordModelValidator()
        {
            RuleFor(p => p.Email).EmailAddress().WithMessage("Geçerli bir eposta adresi değil.");
            RuleFor(p => p.ResetPasswordUrl).NotEmpty();
        }
    }
}
